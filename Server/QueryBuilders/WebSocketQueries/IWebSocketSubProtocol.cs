/*
 * WebSocket subprotocol negotiation in ASP.NET Core: https://www.tpeczek.com/2017/06/websocket-subprotocol-negotiation-in.html
 */

using System.Net.WebSockets;

namespace Server.QueryBuilders.WebSocketQueries;

internal interface IWebSocketSubProtocol
{
	string SubProtocol { get; }

	Task SendAsync<T>(T data, WebSocket webSocket, CancellationToken cancellationToken);

	T Deserialize<T>(byte[] buffer);
}
