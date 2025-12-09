using Protocols.WebSocketProtocol.DataProtocols;
using Protocols.WebSocketProtocol.DataSerializers;

namespace Protocols.WebSocketProtocol;

public sealed class MessagePackWebSocketSubProtocol : WebSocketSubProtocolBase
{
	private static readonly MessagePackWebSocketDataSerializer _serializer = new();

	internal MessagePackWebSocketSubProtocol(WebSocketDataProtocol dataProtocol)
		: base(dataProtocol)
	{ }

	public override string SubProtocol { get; } = "message-pack";

	public static IWebSocketSubProtocol WithClassicDataProtocol { get; } = new MessagePackWebSocketSubProtocol(new ClassicWebSocketDataProtocol(_serializer));
	public static IWebSocketSubProtocol WithModernDataProtocol { get; } = new MessagePackWebSocketSubProtocol(new ModernWebSocketDataProtocol(_serializer));
}
