#pragma warning disable VSTHRD200

using BenchmarkDotNet.Attributes;

using DataQuerySpeedTest.ServiceDefaults;

namespace Tester.Benchmark;

public partial class Benchmarks
{
	[GlobalSetup(Targets = [nameof(RestGet), nameof(RestGetAll), nameof(RestCreate)])]
	public void RestGlobalSetup() => CreateRestModule();

	[Benchmark(Description = RequestTypeNames.Rest)]
	[BenchmarkCategory(ScenarioNames.Get)]
	public ValueTask<long?> RestGet() => ExecuteGetAsync();

	[Benchmark(Description = RequestTypeNames.Rest)]
	[BenchmarkCategory(ScenarioNames.GetAll)]
	public ValueTask<long?> RestGetAll() => ExecuteGetAllAsync();

	[Benchmark(Description = RequestTypeNames.Rest)]
	[BenchmarkCategory(ScenarioNames.Create)]
	public ValueTask<long?> RestCreate() => ExecuteCreateAsync();
}
