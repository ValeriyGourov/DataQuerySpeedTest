#pragma warning disable VSTHRD200, CA1515, CA1001

using System.Diagnostics.CodeAnalysis;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Protocols.WebSocketProtocol;

using Tester.Core.Extensions;
using Tester.Core.Modules;

namespace Tester.Benchmark;

[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public partial class Benchmarks
{
	private Uri? _host;
	private Uri? _webSocketHost;

	private IModule? _module;

	private HttpClient? _httpClient;

	[GlobalCleanup]
	public async Task GlobalCleanup()
	{
		switch (_module)
		{
			case IAsyncDisposable asyncDisposable:
				await asyncDisposable.DisposeAsync().ConfigureAwait(false);
				break;

			case IDisposable disposable:
				disposable.Dispose();
				break;
		}

		_httpClient?.Dispose();
	}

	[MemberNotNull(
		nameof(_httpClient),
		nameof(_host),
		nameof(_webSocketHost))]
	private void SetOptions()
	{
		HostApplicationBuilder builder = new();

		builder.AddServiceDefaults();

		builder.Services.ConfigureHttpClient();

		IHost app = builder.Build();

		_httpClient = app.Services
			.GetRequiredService<IHttpClientFactory>()
			.CreateClient();

		IConfiguration configuration = app.Services.GetRequiredService<IConfiguration>();
		_host = configuration.GetHttpsServerEndpont();
		_webSocketHost = configuration.GetWebSocketHost();
	}

	private async ValueTask CreateWebSocketModuleAsync(string endpointName, IWebSocketSubProtocol subProtocol)
	{
		SetOptions();

		_module = await WebSocketModule
			.CreateAsync(
				_webSocketHost,
				endpointName,
				subProtocol,
				CancellationToken.None)
			.ConfigureAwait(false);
	}

	private void CreateRestModule()
	{
		SetOptions();
		_module = new RestModule(_httpClient);
	}

	private void CreateGrpcModule()
	{
		SetOptions();
		_module = new GrpcModule(_host, _httpClient);
	}

	private static ValueTask<long?> ExecuteAsync(Func<CancellationToken, ValueTask<long?>> method) => method(CancellationToken.None);

	private ValueTask<long?> ExecuteGetAsync()
	{
		ArgumentNullException.ThrowIfNull(_module);
		return ExecuteAsync(_module.ExecuteGetAsync);
	}

	private ValueTask<long?> ExecuteGetAllAsync()
	{
		ArgumentNullException.ThrowIfNull(_module);
		return ExecuteAsync(_module.ExecuteGetAllAsync);
	}

	private ValueTask<long?> ExecuteCreateAsync()
	{
		ArgumentNullException.ThrowIfNull(_module);
		return ExecuteAsync(_module.ExecuteCreateAsync);
	}
}
