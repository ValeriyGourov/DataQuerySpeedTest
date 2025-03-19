using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Server.QueryBuilders;

internal static class WebSocketExtensions
{
	public static async Task<byte[]> ReceiveToTheEndAsync(
		this WebSocket webSocket,
		CancellationToken cancellationToken = default)
	{
		byte[] buffer = new byte[1024 * 4];

#pragma warning disable CA2007 // Попробуйте вызвать ConfigureAwait для ожидаемой задачи
		await using MemoryStream memoryStream = new();
#pragma warning restore CA2007 // Попробуйте вызвать ConfigureAwait для ожидаемой задачи
		WebSocketReceiveResult result;

		do
		{
			result = await webSocket
				.ReceiveAsync(buffer, cancellationToken)
				.ConfigureAwait(false);

			await memoryStream
				.WriteAsync(
					buffer.AsMemory(0, result.Count),
					cancellationToken)
				.ConfigureAwait(false);
		} while (!result.EndOfMessage);

		return memoryStream.ToArray();
	}

	public static Task SendAsJsonAsync<T>(
		this WebSocket webSocket,
		T data,
		CancellationToken cancellationToken = default)
	{
		string json = JsonSerializer.Serialize(data);

		return webSocket.SendAsync(
			Encoding.UTF8.GetBytes(json),
			WebSocketMessageType.Text,
			true,
			cancellationToken);
	}
}
