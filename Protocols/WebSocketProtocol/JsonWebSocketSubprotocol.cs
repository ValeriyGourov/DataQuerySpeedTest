using Protocols.WebSocketProtocol.DataProtocols;
using Protocols.WebSocketProtocol.DataSerializers;

namespace Protocols.WebSocketProtocol;

public sealed class JsonWebSocketSubProtocol : WebSocketSubProtocolBase
{
	private static readonly JsonWebSocketDataSerializer _serializer = new();

	internal JsonWebSocketSubProtocol(WebSocketDataProtocol dataProtocol)
		: base(dataProtocol)
	{ }

	public override string SubProtocol { get; } = "json";

	public static IWebSocketSubProtocol WithClassicDataProtocol { get; } = new JsonWebSocketSubProtocol(new ClassicWebSocketDataProtocol(_serializer));
	public static IWebSocketSubProtocol WithModernDataProtocol { get; } = new JsonWebSocketSubProtocol(new ModernWebSocketDataProtocol(_serializer));
}
