using System.Net.WebSockets;

using Protocols.WebSocketProtocol.DataSerializers;

namespace Protocols.WebSocketProtocol.DataProtocols;

internal sealed class ModernWebSocketDataProtocol(IWebSocketDataSerializer serializer)
	: WebSocketDataProtocol(serializer)
{
	public override string Name { get; } = "Modern";

	public override async Task<T> ReceiveAsync<T>(WebSocket webSocket, CancellationToken cancellationToken)
	{
		CheckWebSocket(webSocket);

#pragma warning disable CA2007
		await using WebSocketStream stream = WebSocketStream.CreateReadableMessageStream(webSocket);
#pragma warning restore CA2007

		return await DeserializeAsync<T>(stream, cancellationToken).ConfigureAwait(false);
	}

	public override async Task SendAsync<T>(T data, WebSocket webSocket, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(data);
		ArgumentNullException.ThrowIfNull(webSocket);

#pragma warning disable CA2007
		await using WebSocketStream stream = WebSocketStream.CreateWritableMessageStream(
			webSocket,
			WebSocketMessageType.Binary);
#pragma warning restore CA2007

		await SerializeAsync(data, stream, cancellationToken).ConfigureAwait(false);
	}
}
