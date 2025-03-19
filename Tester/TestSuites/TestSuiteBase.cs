using System.Diagnostics;
using System.Security.Cryptography;

using Microsoft.Extensions.Configuration;

using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Sinks.Timescale;

using Serilog.Events;

namespace Tester.TestSuites;

internal abstract class TestSuiteBase(IConfiguration configuration)
{
	private readonly string _timescaleDbConnectionString = configuration.GetConnectionString("TimescaleDB")
		?? throw new ArgumentException("Не указана строка подключения к базе данных результатов.");

	protected ushort DefaultPageSize { get; } = 20;

	public abstract string Name { get; }

	protected abstract void RunGetScenario();
	protected abstract void RunGetAllScenario();
	protected abstract void RunCreateScenario();

	public void Run()
	{
		RunGetScenario();
		RunGetAllScenario();
		RunCreateScenario();
	}

	protected void RunNBomberRunner(ScenarioProps scenario)
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

	protected static int GetDataId() => RandomNumberGenerator.GetInt32(int.MaxValue);

	protected static object CreateOrder() => new
	{
		ProductName = "Товар 123",
		Quantity = 12.34m
	};
}
