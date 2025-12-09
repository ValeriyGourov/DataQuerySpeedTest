using System.Net.WebSockets;
using System.Runtime.Serialization;

using Protocols.WebSocketProtocol.DataSerializers;

namespace Protocols.WebSocketProtocol.DataProtocols;

internal abstract class WebSocketDataProtocol(IWebSocketDataSerializer serializer)
{
	protected IWebSocketDataSerializer Serializer { get; } = serializer;

	public abstract string Name { get; }

	public abstract Task<T> ReceiveAsync<T>(WebSocket webSocket, CancellationToken cancellationToken);
	public abstract Task SendAsync<T>(T data, WebSocket webSocket, CancellationToken cancellationToken);

	protected static void CheckWebSocket(WebSocket webSocket)
	{
		ArgumentNullException.ThrowIfNull(webSocket);

		if (webSocket.State != WebSocketState.Open)
		{
			throw new InvalidOperationException("WebSocket не открыт.");
		}
	}

	protected async ValueTask<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken)
	{
		T? data;

		try
		{
			data = await Serializer.DeserializeAsync<T>(stream, cancellationToken)
				.ConfigureAwait(false);
		}
		catch (Exception exception)
		{
			throw new SerializationException(
				$"Не удалось прочитать полученные данные в тип '{typeof(T).FullName}'.",
				exception);
		}

		return data is null
			|| EqualityComparer<T>.Default.Equals(data, default)
			? throw new SerializationException("В сообщении не обнаружены полезные данные.")
			: data;
	}

	protected Task SerializeAsync<T>(
		T data,
		Stream stream,
		CancellationToken cancellationToken)
		=> Serializer.SerializeAsync(data, stream, cancellationToken);

	protected byte[] Serialize<T>(T data, CancellationToken cancellationToken)
		=> Serializer.Serialize(data, cancellationToken);
}
