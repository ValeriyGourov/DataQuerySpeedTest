using System.Net.WebSockets;

namespace Server.QueryBuilders.WebSocketQueries;

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
}
