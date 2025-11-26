#pragma warning disable CA1034 // TODO: Удалить после исправления ошибки анализатора: https://github.com/dotnet/roslyn-analyzers/issues/7765

using System.Buffers;
using System.Net.WebSockets;

using Microsoft.IO;

namespace Protocols.WebSocketProtocol;

public static class WebSocketExtensions
{
	private const int _bufferSize = 1024 * 4;

	private static readonly RecyclableMemoryStreamManager _memoryStreamManager = new();

	extension(WebSocket webSocket)
	{
		public async ValueTask<Stream> ReceiveAllAsync(CancellationToken cancellationToken = default)
		{
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
}
