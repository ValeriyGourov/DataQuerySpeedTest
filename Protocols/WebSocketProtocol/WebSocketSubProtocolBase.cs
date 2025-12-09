using System.Net.WebSockets;

using Protocols.WebSocketProtocol.DataProtocols;

namespace Protocols.WebSocketProtocol;

public abstract class WebSocketSubProtocolBase : IWebSocketSubProtocol
{
	private readonly WebSocketDataProtocol _dataProtocol;

	public abstract string SubProtocol { get; }

	public string Name { get; }
	public string DataProtocol => _dataProtocol.Name;

	private protected WebSocketSubProtocolBase(WebSocketDataProtocol dataProtocol)
	{
		_dataProtocol = dataProtocol;

		Name = $"{DataProtocol} ({SubProtocol})";
	}

	public Task SendAsync<T>(T data, WebSocket webSocket, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(data);
		ArgumentNullException.ThrowIfNull(webSocket);

		return _dataProtocol.SendAsync(data, webSocket, cancellationToken);
	}

	public Task<T> ReceiveAsync<T>(WebSocket webSocket, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(webSocket);

		return _dataProtocol.ReceiveAsync<T>(webSocket, cancellationToken);
	}
}
