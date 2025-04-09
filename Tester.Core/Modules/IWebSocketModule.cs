namespace Tester.Core.Modules;

public interface IWebSocketModule : IModule
{
	static abstract ValueTask<WebSocketModuleBase> CreateAsync(
		Uri host,
		string endpointName,
		CancellationToken cancellationToken);
}
