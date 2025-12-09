/*
 * WebSocket subprotocol negotiation in ASP.NET Core: https://www.tpeczek.com/2017/06/websocket-subprotocol-negotiation-in.html
 */

using System.Net.WebSockets;

namespace Protocols.WebSocketProtocol;

public interface IWebSocketSubProtocol
{
	string Name { get; }
	string SubProtocol { get; }
	string DataProtocol { get; }

	Task<T> ReceiveAsync<T>(WebSocket webSocket, CancellationToken cancellationToken);
	Task SendAsync<T>(T data, WebSocket webSocket, CancellationToken cancellationToken);
}
