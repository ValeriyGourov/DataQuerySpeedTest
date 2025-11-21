#pragma warning disable IDE0130

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

// Adds common Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class ServiceExtensions
{
	private const string _healthEndpointPath = "/health";
	private const string _alivenessEndpointPath = "/alive";

	public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder)
		where TBuilder : IHostApplicationBuilder
	{
		_ = builder.ConfigureOpenTelemetry();

		_ = builder.AddDefaultHealthChecks();

		_ = builder.Services.AddServiceDiscovery();

		_ = builder.Services.ConfigureHttpClientDefaults(static http =>
		{
			_ = http.UseSocketsHttpHandler(static (handler, _)
				=> handler.EnableMultipleHttp2Connections = true);

			// Turn on resilience by default
			_ = http.AddStandardResilienceHandler();

			// Turn on service discovery by default
			_ = http.AddServiceDiscovery();
		});

		return builder;
	}

	public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder)
		where TBuilder : IHostApplicationBuilder
	{
		_ = builder.Logging
			.AddOpenTelemetry(static logging =>
			{
				logging.IncludeFormattedMessage = true;
				logging.IncludeScopes = true;
			})
			.SetMinimumLevel(LogLevel.Error);

		_ = builder.Services
			.AddOpenTelemetry()
			.WithMetrics(static metrics =>
			{
				_ = metrics
					.AddAspNetCoreInstrumentation()
					.AddHttpClientInstrumentation()
					.AddRuntimeInstrumentation();
			})
			.WithTracing(tracing =>
			{
				_ = tracing
					.AddSource(builder.Environment.ApplicationName)
					.AddAspNetCoreInstrumentation(static tracing =>
						// Exclude health check requests from tracing
						tracing.Filter = context =>
							!context.Request.Path.StartsWithSegments(_healthEndpointPath, StringComparison.InvariantCultureIgnoreCase)
							&& !context.Request.Path.StartsWithSegments(_alivenessEndpointPath, StringComparison.InvariantCultureIgnoreCase)
					)
					.AddGrpcClientInstrumentation()
					.AddHttpClientInstrumentation();
			});

		_ = builder.AddOpenTelemetryExporters();

		return builder;
	}

	private static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder)
		where TBuilder : IHostApplicationBuilder
	{
		bool useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

		if (useOtlpExporter)
		{
			_ = builder.Services.AddOpenTelemetry().UseOtlpExporter();
		}

		return builder;
	}

	public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder)
		where TBuilder : IHostApplicationBuilder
	{
		_ = builder.Services.AddHealthChecks()
			// Add a default liveness check to ensure app is responsive
			.AddCheck(
				"self",
				static () => HealthCheckResult.Healthy(),
				["live"]);

		return builder;
	}

	public static WebApplication MapDefaultEndpoints(this WebApplication app)
	{
		ArgumentNullException.ThrowIfNull(app);

		// Adding health checks endpoints to applications in non-development environments has security implications.
		// See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
		if (app.Environment.IsDevelopment())
		{
			// All health checks must pass for app to be considered ready to accept traffic after starting
			_ = app.MapHealthChecks(_healthEndpointPath);

			// Only health checks tagged with the "live" tag must pass for app to be considered alive
			_ = app.MapHealthChecks(
				_alivenessEndpointPath,
				new HealthCheckOptions
				{
					Predicate = static registration => registration.Tags.Contains("live")
				});
		}

		return app;
	}
}
