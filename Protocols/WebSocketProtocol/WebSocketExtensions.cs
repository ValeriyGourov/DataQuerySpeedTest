using System.Buffers;
using System.Net.WebSockets;

using Microsoft.IO;

namespace Protocols.WebSocketProtocol;

public static class WebSocketExtensions
{
	private const int _bufferSize = 1024 * 4;

	private static readonly RecyclableMemoryStreamManager _memoryStreamManager = new();

	public static async ValueTask<Stream> ReceiveAllAsync(
		this WebSocket webSocket,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(webSocket);
		byte[] buffer = ArrayPool<byte>.Shared.Rent(_bufferSize);

		RecyclableMemoryStream stream = _memoryStreamManager.GetStream();
		WebSocketReceiveResult result;

		try
		{
			do
			{
				result = await webSocket
					.ReceiveAsync(buffer, cancellationToken)
					.ConfigureAwait(false);

				await stream
					.WriteAsync(
						buffer.AsMemory(0, result.Count),
						cancellationToken)
					.ConfigureAwait(false);
			} while (!result.EndOfMessage);
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(buffer);
		}

		stream.Position = 0;

		return stream;
	}
}
