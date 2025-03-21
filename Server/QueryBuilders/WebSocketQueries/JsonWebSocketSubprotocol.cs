using System.Net.WebSockets;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;

namespace Server.QueryBuilders.WebSocketQueries;

internal sealed class JsonWebSocketSubprotocol : IWebSocketSubProtocol
{
	public string SubProtocol { get; } = "json";

	public Task SendAsync<T>(
		T data,
		WebSocket webSocket,
		CancellationToken cancellationToken)
	{
		string json = JsonSerializer.Serialize(data);

		return webSocket.SendAsync(
			Encoding.UTF8.GetBytes(json),
			WebSocketMessageType.Text,
			true,
			cancellationToken);
	}

	public T Deserialize<T>(byte[] buffer)
	{
		string json = Encoding.UTF8.GetString(buffer);

		// Для простоты просто выбросим исключение при любой ошибке десериализации.

		T? message;
		try
		{
			message = JsonSerializer.Deserialize<T>(json);
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
