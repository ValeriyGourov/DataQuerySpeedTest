using System.Net.WebSockets;
using System.Runtime.Serialization;
using System.Text.Json;

namespace Protocols.WebSocketProtocol;

public sealed class JsonWebSocketSubprotocol : IWebSocketSubProtocol
{
	public string SubProtocol { get; } = "json";

	public async Task<long> SendAsync<T>(
		T data,
		WebSocket webSocket,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(data);
		ArgumentNullException.ThrowIfNull(webSocket);

		byte[] bytes = Serialize(data, cancellationToken);

		await webSocket
			.SendAsync(
				bytes,
				WebSocketMessageType.Text,
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

		return JsonSerializer.SerializeToUtf8Bytes(request);
	}

	public T Deserialize<T>(
		Stream stream,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(stream);

		T? message;
		try
		{
			message = JsonSerializer.Deserialize<T>(stream);
		}
		catch (JsonException exception)
		{
			throw new SerializationException(
				"Не удалось прочитать полученные данные как JSON.",
				exception);
		}

		if (message is null
			|| EqualityComparer<T>.Default.Equals(message, default))
		{
			throw new SerializationException("В сообщении не обнаружены полезные данные.");
		}

		return message;
	}
}
