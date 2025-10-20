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
#pragma warning disable CA2007
		await using Stream stream = await webSocket
			.ReceiveAllAsync(cancellationToken)
			.ConfigureAwait(false);
#pragma warning restore CA2007

		if (webSocket.State == WebSocketState.Open)
		{
			T data = Deserialize<T>(stream, cancellationToken);
			return new(data, stream.Length);
		}
		else
		{
			return default;
		}
	}

	byte[] Serialize<T>(T request, CancellationToken cancellationToken = default);

	T Deserialize<T>(Stream stream, CancellationToken cancellationToken = default);
}
