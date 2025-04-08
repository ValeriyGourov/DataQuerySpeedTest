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

		byte[] bytes = await SerializeAsync(data, cancellationToken)
			.ConfigureAwait(false);

		await webSocket
			.SendAsync(
				bytes,
				WebSocketMessageType.Text,
				true,
				cancellationToken)
			.ConfigureAwait(false);

		return bytes.Length;
	}

	public async ValueTask<byte[]> SerializeAsync<T>(
		T request,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(request);

#pragma warning disable CA2007
		await using MemoryStream memoryStream = new();
#pragma warning restore CA2007
		await JsonSerializer
			.SerializeAsync(memoryStream, request, cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return memoryStream.ToArray();
	}

	public async ValueTask<T> DeserializeAsync<T>(
		byte[] buffer,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(buffer);

#pragma warning disable CA2007
		await using MemoryStream memoryStream = new(buffer);
#pragma warning restore CA2007

		// Для простоты просто выбросим исключение при любой ошибке десериализации.

		T? message;
		try
		{
			message = await JsonSerializer
				.DeserializeAsync<T>(memoryStream, cancellationToken: cancellationToken)
				.ConfigureAwait(false);
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
