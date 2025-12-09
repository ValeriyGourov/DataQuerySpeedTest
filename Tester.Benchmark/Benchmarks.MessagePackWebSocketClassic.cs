#pragma warning disable VSTHRD200

using BenchmarkDotNet.Attributes;

using DataQuerySpeedTest.ServiceDefaults;

using Protocols.WebSocketProtocol;

namespace Tester.Benchmark;

public partial class Benchmarks
{
	#region Get

	[GlobalSetup(Target = nameof(MessagePackWebSocketClassicGet))]
	public ValueTask MessagePackWebSocketClassicGetGlobalSetup() => CreateWebSocketModuleAsync(ScenarioNames.Get, MessagePackWebSocketSubProtocol.WithClassicDataProtocol);

	[Benchmark(Description = RequestTypeNames.WebSocketClassicMessagePack)]
	[BenchmarkCategory(ScenarioNames.Get)]
	public ValueTask<long?> MessagePackWebSocketClassicGet() => ExecuteGetAsync();

	#endregion

	#region GetAll

	[GlobalSetup(Target = nameof(MessagePackWebSocketClassicGetAll))]
	public ValueTask MessagePackWebSocketClassicGetAllGlobalSetup() => CreateWebSocketModuleAsync(ScenarioNames.GetAll, MessagePackWebSocketSubProtocol.WithClassicDataProtocol);

	[Benchmark(Description = RequestTypeNames.WebSocketClassicMessagePack)]
	[BenchmarkCategory(ScenarioNames.GetAll)]
	public ValueTask<long?> MessagePackWebSocketClassicGetAll() => ExecuteGetAllAsync();

	#endregion

	#region Create

	[GlobalSetup(Target = nameof(MessagePackWebSocketClassicCreate))]
	public ValueTask MessagePackWebSocketClassicCreateGlobalSetup() => CreateWebSocketModuleAsync(ScenarioNames.Create, MessagePackWebSocketSubProtocol.WithClassicDataProtocol);

	[Benchmark(Description = RequestTypeNames.WebSocketClassicMessagePack)]
	[BenchmarkCategory(ScenarioNames.Create)]
	public ValueTask<long?> MessagePackWebSocketClassicCreate() => ExecuteCreateAsync();

	#endregion
}
