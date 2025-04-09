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

		byte[] bytes = await SerializeAsync(data, cancellationToken)
			.ConfigureAwait(false);

		await webSocket
			.SendAsync(
				bytes,
				WebSocketMessageType.Binary,
				true,
				cancellationToken)
			.ConfigureAwait(false);

		return bytes.Length;
	}

	public ValueTask<byte[]> SerializeAsync<T>(
		T request,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(request);

		return ValueTask.FromResult(MessagePackSerializer.Serialize(request, cancellationToken: cancellationToken));
	}

	public ValueTask<T> DeserializeAsync<T>(
		byte[] buffer,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(buffer);

		return ValueTask.FromResult(MessagePackSerializer.Deserialize<T>(buffer, cancellationToken: cancellationToken));
	}
}
