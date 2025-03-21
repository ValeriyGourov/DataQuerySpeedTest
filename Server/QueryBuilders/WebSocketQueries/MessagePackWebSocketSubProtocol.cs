using System.Net.WebSockets;

using MessagePack;

namespace Server.QueryBuilders.WebSocketQueries;

internal sealed class MessagePackWebSocketSubProtocol : IWebSocketSubProtocol
{
	public string SubProtocol { get; } = "message-pack";

	public Task SendAsync<T>(T data, WebSocket webSocket, CancellationToken cancellationToken)
	{
		byte[] bytes = MessagePackSerializer.Serialize(data, cancellationToken: cancellationToken);

		return webSocket.SendAsync(
			bytes,
			WebSocketMessageType.Binary,
			true,
			cancellationToken);
	}

	public T Deserialize<T>(byte[] buffer)
	{
		return MessagePackSerializer.Deserialize<T>(buffer);
	}
}
