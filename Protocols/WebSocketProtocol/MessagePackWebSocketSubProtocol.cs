using System.Net.WebSockets;

using MessagePack;

namespace Protocols.WebSocketProtocol;

public sealed class MessagePackWebSocketSubProtocol : IWebSocketSubProtocol
{
	public string SubProtocol { get; } = "message-pack";

	public async Task<long> SendAsync<T>(T data, WebSocket webSocket, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(data);
		ArgumentNullException.ThrowIfNull(webSocket);

		byte[] bytes = Serialize(data, cancellationToken);

		await webSocket
			.SendAsync(
				bytes,
				WebSocketMessageType.Binary,
				true,
				cancellationToken)
			.ConfigureAwait(false);

		return bytes.Length;
	}

	public byte[] Serialize<T>(
		T request,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(request);

		return MessagePackSerializer.Serialize(request, cancellationToken: cancellationToken);
	}

	public T Deserialize<T>(
		Stream stream,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(stream);

		return MessagePackSerializer.Deserialize<T>(stream, cancellationToken: cancellationToken);
	}
}
