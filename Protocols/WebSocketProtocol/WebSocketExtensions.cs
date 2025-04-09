using System.Buffers;
using System.Net.WebSockets;

namespace Protocols.WebSocketProtocol;

public static class WebSocketExtensions
{
	private const int _bufferSize = 1024 * 4;

	public static async ValueTask<byte[]> ReceiveAllAsync(
		this WebSocket webSocket,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(webSocket);

		byte[] buffer = ArrayPool<byte>.Shared.Rent(_bufferSize);

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

		ArrayPool<byte>.Shared.Return(buffer);

		return memoryStream.ToArray();
	}
}
