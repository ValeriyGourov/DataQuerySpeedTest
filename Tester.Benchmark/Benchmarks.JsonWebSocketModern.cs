#pragma warning disable VSTHRD200

using BenchmarkDotNet.Attributes;

using DataQuerySpeedTest.ServiceDefaults;

using Protocols.WebSocketProtocol;

namespace Tester.Benchmark;

public partial class Benchmarks
{
	#region Get

	[GlobalSetup(Target = nameof(JsonWebSocketModernGet))]
	public ValueTask JsonWebSocketModernGetGlobalSetup() => CreateWebSocketModuleAsync(ScenarioNames.Get, JsonWebSocketSubProtocol.WithModernDataProtocol);

	[Benchmark(Description = RequestTypeNames.WebSocketModernJson)]
	[BenchmarkCategory(ScenarioNames.Get)]
	public ValueTask<long?> JsonWebSocketModernGet() => ExecuteGetAsync();

	#endregion

	#region GetAll

	[GlobalSetup(Target = nameof(JsonWebSocketModernGetAll))]
	public ValueTask JsonWebSocketModernGetAllGlobalSetup() => CreateWebSocketModuleAsync(ScenarioNames.GetAll, JsonWebSocketSubProtocol.WithModernDataProtocol);

	[Benchmark(Description = RequestTypeNames.WebSocketModernJson)]
	[BenchmarkCategory(ScenarioNames.GetAll)]
	public ValueTask<long?> JsonWebSocketModernGetAll() => ExecuteGetAllAsync();

	#endregion

	#region Create

	[GlobalSetup(Target = nameof(JsonWebSocketModernCreate))]
	public ValueTask JsonWebSocketModernCreateGlobalSetup() => CreateWebSocketModuleAsync(ScenarioNames.Create, JsonWebSocketSubProtocol.WithModernDataProtocol);

	[Benchmark(Description = RequestTypeNames.WebSocketModernJson)]
	[BenchmarkCategory(ScenarioNames.Create)]
	public ValueTask<long?> JsonWebSocketModernCreate() => ExecuteCreateAsync();

	#endregion
}
