#pragma warning disable VSTHRD200

using BenchmarkDotNet.Attributes;

using DataQuerySpeedTest.ServiceDefaults;

using Protocols.WebSocketProtocol;

namespace Tester.Benchmark;

public partial class Benchmarks
{
	#region Get

	[GlobalSetup(Target = nameof(JsonWebSocketClassicGet))]
	public ValueTask JsonWebSocketClassicGetGlobalSetup() => CreateWebSocketModuleAsync(ScenarioNames.Get, JsonWebSocketSubProtocol.WithClassicDataProtocol);

	[Benchmark(Description = RequestTypeNames.WebSocketClassicJson, Baseline = true)]
	[BenchmarkCategory(ScenarioNames.Get)]
	public ValueTask<long?> JsonWebSocketClassicGet() => ExecuteGetAsync();

	#endregion

	#region GetAll

	[GlobalSetup(Target = nameof(JsonWebSocketClassicGetAll))]
	public ValueTask JsonWebSocketClassicGetAllGlobalSetup() => CreateWebSocketModuleAsync(ScenarioNames.GetAll, JsonWebSocketSubProtocol.WithClassicDataProtocol);

	[Benchmark(Description = RequestTypeNames.WebSocketClassicJson, Baseline = true)]
	[BenchmarkCategory(ScenarioNames.GetAll)]
	public ValueTask<long?> JsonWebSocketClassicGetAll() => ExecuteGetAllAsync();

	#endregion

	#region Create

	[GlobalSetup(Target = nameof(JsonWebSocketClassicCreate))]
	public ValueTask JsonWebSocketClassicCreateGlobalSetup() => CreateWebSocketModuleAsync(ScenarioNames.Create, JsonWebSocketSubProtocol.WithClassicDataProtocol);

	[Benchmark(Description = RequestTypeNames.WebSocketClassicJson, Baseline = true)]
	[BenchmarkCategory(ScenarioNames.Create)]
	public ValueTask<long?> JsonWebSocketClassicCreate() => ExecuteCreateAsync();

	#endregion
}
