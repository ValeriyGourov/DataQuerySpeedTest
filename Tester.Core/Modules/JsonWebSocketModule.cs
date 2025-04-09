using Protocols.WebSocketProtocol;

namespace Tester.Core.Modules;

public sealed class JsonWebSocketModule : WebSocketModuleBase, IWebSocketModule
{
	private JsonWebSocketModule()
		: base(new JsonWebSocketSubprotocol())
	{ }

	public static async ValueTask<WebSocketModuleBase> CreateAsync(
		Uri host,
		string endpointName,
		CancellationToken cancellationToken)
	{
		JsonWebSocketModule module = new();
		await InitializeAsync(module, host, endpointName, cancellationToken)
			.ConfigureAwait(false);

		return module;
	}
}
