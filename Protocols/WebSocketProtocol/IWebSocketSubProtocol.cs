/*
 * WebSocket subprotocol negotiation in ASP.NET Core: https://www.tpeczek.com/2017/06/websocket-subprotocol-negotiation-in.html
 */

using System.Net.WebSockets;

namespace Protocols.WebSocketProtocol;

public interface IWebSocketSubProtocol
{
	string SubProtocol { get; }

	Task<long> SendAsync<T>(T data, WebSocket webSocket, CancellationToken cancellationToken);

	async Task<WebSocketResponse<T>> ReceiveAsync<T>(WebSocket webSocket, CancellationToken cancellationToken)
	{
		byte[] bytes = await webSocket
			.ReceiveAllAsync(cancellationToken)
			.ConfigureAwait(false);

		if (webSocket.State == WebSocketState.Open)
		{
			T data = await DeserializeAsync<T>(bytes, cancellationToken)
				.ConfigureAwait(false);
			return new(data, bytes.Length);
		}
		else
		{
			return default;
		}
	}

	ValueTask<byte[]> SerializeAsync<T>(T request, CancellationToken cancellationToken = default);

	ValueTask<T> DeserializeAsync<T>(byte[] buffer, CancellationToken cancellationToken = default);
}
