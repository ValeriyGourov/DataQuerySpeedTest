using Protocols.WebSocketProtocol;

namespace Tester.Core.Modules;

public sealed class MessagePackWebSocketModule : WebSocketModuleBase, IWebSocketModule
{
	private MessagePackWebSocketModule()
		: base(new MessagePackWebSocketSubProtocol())
	{ }

	public static async ValueTask<WebSocketModuleBase> CreateAsync(
		Uri host,
		string endpointName,
		CancellationToken cancellationToken)
	{
		MessagePackWebSocketModule module = new();
		await InitializeAsync(module, host, endpointName, cancellationToken)
			.ConfigureAwait(false);

		return module;
	}
}
