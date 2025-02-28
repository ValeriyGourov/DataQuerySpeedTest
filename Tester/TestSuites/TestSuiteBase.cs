using System.Diagnostics;
using System.Runtime.CompilerServices;

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

	protected abstract ScenarioProps Get { get; }
	protected abstract ScenarioProps GetAll { get; }
	protected abstract ScenarioProps Create { get; }

	public abstract string Name { get; }

	public void Run()
	{
		RunScenario(Get);
		RunScenario(GetAll);
		RunScenario(Create);

		void RunScenario(ScenarioProps scenario)
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

	protected static string GetScenarioName([CallerMemberName] string name = "") => name;
}
