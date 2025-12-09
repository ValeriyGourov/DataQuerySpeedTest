using System.Buffers;
using System.Net.WebSockets;

using Microsoft.IO;

using Protocols.WebSocketProtocol.DataSerializers;

namespace Protocols.WebSocketProtocol.DataProtocols;

internal sealed class ClassicWebSocketDataProtocol(IWebSocketDataSerializer serializer)
	: WebSocketDataProtocol(serializer)
{
	private const int _bufferSize = 1024 * 4;

	private static readonly RecyclableMemoryStreamManager _memoryStreamManager = new();

	public override string Name { get; } = "Classic";

	public override async Task<T> ReceiveAsync<T>(WebSocket webSocket, CancellationToken cancellationToken)
	{
		CheckWebSocket(webSocket);

#pragma warning disable CA2007
		await using Stream stream = await ReceiveAllAsync(webSocket, cancellationToken).ConfigureAwait(false);
#pragma warning restore CA2007

		return await DeserializeAsync<T>(stream, cancellationToken).ConfigureAwait(false);
	}

	public override Task SendAsync<T>(T data, WebSocket webSocket, CancellationToken cancellationToken)
	{
		byte[] bytes = Serialize(data, cancellationToken);

		return webSocket.SendAsync(
			bytes,
			WebSocketMessageType.Binary,
			true,
			cancellationToken);
	}

	private static async Task<Stream> ReceiveAllAsync(WebSocket webSocket, CancellationToken cancellationToken)
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
