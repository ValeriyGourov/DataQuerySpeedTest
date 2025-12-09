//using DataQuerySpeedTest.ServiceDefaults;

//using Microsoft.Extensions.Configuration;

//using Protocols.WebSocketProtocol;

//using Tester.Core.Extensions;
//using Tester.Core.Modules;

//namespace Tester.Benchmark;

//internal static class ModuleCollection
//{
//	private static readonly IReadOnlyDictionary<string, List<IModule>> _modules = new Dictionary<string, List<IModule>>
//	{
//		[nameof(ScenarioNames.Get)] = [],
//		[nameof(ScenarioNames.GetAll)] = [],
//		[nameof(ScenarioNames.Create)] = [],
//	};

//	public static async Task InitializeAsync(IConfiguration configuration, HttpClient httpClient)
//	{
//		Uri host = configuration.GetHttpsServerEndpont();
//		Uri webSocketHost = configuration.GetWebSocketHost();

//		await CreateWebSocketModulesAsync(
//			webSocketHost,
//			ScenarioNames.Get,
//			JsonWebSocketSubProtocol.WithClassicDataProtocol,
//			MessagePackWebSocketSubProtocol.WithClassicDataProtocol)
//			.ConfigureAwait(false);

//		await CreateWebSocketModulesAsync(
//			webSocketHost,
//			ScenarioNames.GetAll,
//			JsonWebSocketSubProtocol.WithClassicDataProtocol,
//			MessagePackWebSocketSubProtocol.WithClassicDataProtocol)
//			.ConfigureAwait(false);

//		await CreateWebSocketModulesAsync(
//			webSocketHost,
//			ScenarioNames.Create,
//			JsonWebSocketSubProtocol.WithClassicDataProtocol,
//			MessagePackWebSocketSubProtocol.WithClassicDataProtocol)
//			.ConfigureAwait(false);

//		CreateModule(
//			() => new RestModule(httpClient),
//			ScenarioNames.Get,
//			ScenarioNames.GetAll,
//			ScenarioNames.Create);

//		CreateModule(
//			() => new GrpcModule(host, httpClient),
//			ScenarioNames.Get,
//			ScenarioNames.GetAll,
//			ScenarioNames.Create);
//	}

//	public static List<IModule> GetModules(string scenarioName)
//		=> _modules
//			.First(item => item.Key == scenarioName)
//			.Value;

//	private static void AddModule(IModule module, params IEnumerable<string> scenarioNames)
//	{
//		foreach (string scenarioName in scenarioNames)
//		{
//			_modules[scenarioName].Add(module);
//		}
//	}

//	private static void CreateModule(Func<IModule> createFunc, params IEnumerable<string> scenarioNames)
//	{
//		IModule module = createFunc();
//		AddModule(module, scenarioNames);
//	}

//	private static async ValueTask CreateModuleAsync(Func<ValueTask<IModule>> createFunc, params IEnumerable<string> scenarioNames)
//	{
//		IModule module = await createFunc().ConfigureAwait(false);
//		AddModule(module, scenarioNames);
//	}

//	//private static ValueTask CreateWebSocketModuleAsync(
//	//	Uri webSocketHost,
//	//	string endpointName,
//	//	IWebSocketSubProtocol subProtocol)
//	//{
//	//	return CreateModuleAsync(
//	//		async () => await WebSocketModule
//	//			.CreateAsync(
//	//				webSocketHost,
//	//				endpointName,
//	//				subProtocol,
//	//				CancellationToken.None)
//	//			.ConfigureAwait(false),
//	//		endpointName);
//	//}

//	private static async ValueTask CreateWebSocketModulesAsync(
//		Uri webSocketHost,
//		string endpointName,
//		params IEnumerable<IWebSocketSubProtocol> subProtocols)
//	{
//		foreach (IWebSocketSubProtocol subProtocol in subProtocols)
//		{
//			await CreateModuleAsync(
//				createFunc: async () => await WebSocketModule
//					.CreateAsync(
//						webSocketHost,
//						endpointName,
//						subProtocol,
//						CancellationToken.None)
//					.ConfigureAwait(false),
//				scenarioNames: endpointName)
//				.ConfigureAwait(false);
//		}
//	}

//	public static async ValueTask DisposeAsync()
//	{
//		IEnumerable<IModule> modules = _modules.Values
//			.SelectMany(list => list)
//			.Distinct();

//		foreach (IModule module in modules)
//		{
//			switch (module)
//			{
//				case IAsyncDisposable asyncDisposable:
//					await asyncDisposable.DisposeAsync().ConfigureAwait(false);
//					break;

//				case IDisposable disposable:
//					disposable.Dispose();
//					break;
//			}
//		}
//	}
//}
