using System.Diagnostics;

using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Sinks.Timescale;

using Serilog.Events;

namespace Tester.TestSuites;

internal abstract class TestSuiteBase(IConfiguration configuration)
{
	private readonly string _timescaleDbConnectionString = configuration.GetConnectionString("TimescaleDB")
		?? throw new ArgumentException("Не указана строка подключения к базе данных результатов.");

	public abstract string Name { get; }

	protected Task RunGetScenarioAsync() => RunScenarioAsync(
		ScenarioNames.Get,
		static (module, cancellationToken) => module.ExecuteGetAsync(cancellationToken));
	protected Task RunGetAllScenarioAsync() => RunScenarioAsync(
		ScenarioNames.GetAll,
		static (module, cancellationToken) => module.ExecuteGetAllAsync(cancellationToken));
	protected Task RunCreateScenarioAsync() => RunScenarioAsync(
		ScenarioNames.Create,
		static (module, cancellationToken) => module.ExecuteCreateAsync(cancellationToken));

	public async Task RunAsync(CancellationToken _ = default)
	{
		await RunGetScenarioAsync().ConfigureAwait(false);
		await RunGetAllScenarioAsync().ConfigureAwait(false);
		await RunCreateScenarioAsync().ConfigureAwait(false);
	}

	protected abstract ValueTask<IModule> CreateModuleAsync(
		string scenarioName,
		CancellationToken cancellationToken);

	private async Task RunScenarioAsync(
		string scenarioName,
		Func<IModule, CancellationToken, ValueTask<long?>> handler,
		CancellationToken cancellationToken = default)
	{
		IModule module = await CreateModuleAsync(scenarioName, cancellationToken)
			.ConfigureAwait(false);

		ScenarioProps scenario = Scenario.Create(
			scenarioName,
			async _ =>
			{
				long? size = await handler(module, CancellationToken.None).ConfigureAwait(false);
				return Response.Ok(sizeBytes: size ?? default);
			});

		RunNBomberRunner(scenario);

		switch (module)
		{
			case IAsyncDisposable disposableModule:
				await disposableModule.DisposeAsync().ConfigureAwait(false);
				break;
			case IDisposable disposableModule:
				disposableModule.Dispose();
				break;
		}
	}

	private void RunNBomberRunner(ScenarioProps scenario)
	{
		using TimescaleDbSink timescaleDbSink = new(new(_timescaleDbConnectionString));

		// TODO: Уровень журналирования установить в Error как для NBomber, так и для приложения в целом.

		NBomberRunner
			.RegisterScenarios(scenario)
			.WithReportingSinks(timescaleDbSink)
			.WithTestSuite(Name)
			.WithTestName(scenario.ScenarioName)
			.WithMinimumLogLevel(Debugger.IsAttached
				? LogEventLevel.Debug
				: LogEventLevel.Information)
			.Run();
	}
}
