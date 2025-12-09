#pragma warning disable VSTHRD200

using BenchmarkDotNet.Attributes;

using DataQuerySpeedTest.ServiceDefaults;

namespace Tester.Benchmark;

public partial class Benchmarks
{
	[GlobalSetup(Targets = [nameof(GrpcGet), nameof(GrpcGetAll), nameof(GrpcCreate)])]
	public void GrpcGlobalSetup() => CreateGrpcModule();

	[Benchmark(Description = RequestTypeNames.Grpc)]
	[BenchmarkCategory(ScenarioNames.Get)]
	public ValueTask<long?> GrpcGet() => ExecuteGetAsync();

	[Benchmark(Description = RequestTypeNames.Grpc)]
	[BenchmarkCategory(ScenarioNames.GetAll)]
	public ValueTask<long?> GrpcGetAll() => ExecuteGetAllAsync();

	[Benchmark(Description = RequestTypeNames.Grpc)]
	[BenchmarkCategory(ScenarioNames.Create)]
	public ValueTask<long?> GrpcCreate() => ExecuteCreateAsync();
}
