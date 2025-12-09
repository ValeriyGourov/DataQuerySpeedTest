#pragma warning disable VSTHRD200

using BenchmarkDotNet.Attributes;

using DataQuerySpeedTest.ServiceDefaults;

using Protocols.WebSocketProtocol;

namespace Tester.Benchmark;

public partial class Benchmarks
{
	#region Get

	[GlobalSetup(Target = nameof(MessagePackWebSocketModernGet))]
	public ValueTask MessagePackWebSocketModernGetGlobalSetup() => CreateWebSocketModuleAsync(ScenarioNames.Get, MessagePackWebSocketSubProtocol.WithModernDataProtocol);

	[Benchmark(Description = RequestTypeNames.WebSocketModernMessagePack)]
	[BenchmarkCategory(ScenarioNames.Get)]
	public ValueTask<long?> MessagePackWebSocketModernGet() => ExecuteGetAsync();

	#endregion

	#region GetAll

	[GlobalSetup(Target = nameof(MessagePackWebSocketModernGetAll))]
	public ValueTask MessagePackWebSocketModernGetAllGlobalSetup() => CreateWebSocketModuleAsync(ScenarioNames.GetAll, MessagePackWebSocketSubProtocol.WithModernDataProtocol);

	[Benchmark(Description = RequestTypeNames.WebSocketModernMessagePack)]
	[BenchmarkCategory(ScenarioNames.GetAll)]
	public ValueTask<long?> MessagePackWebSocketModernGetAll() => ExecuteGetAllAsync();

	#endregion

	#region Create

	[GlobalSetup(Target = nameof(MessagePackWebSocketModernCreate))]
	public ValueTask MessagePackWebSocketModernCreateGlobalSetup() => CreateWebSocketModuleAsync(ScenarioNames.Create, MessagePackWebSocketSubProtocol.WithModernDataProtocol);

	[Benchmark(Description = RequestTypeNames.WebSocketModernMessagePack)]
	[BenchmarkCategory(ScenarioNames.Create)]
	public ValueTask<long?> MessagePackWebSocketModernCreate() => ExecuteCreateAsync();

	#endregion
}
