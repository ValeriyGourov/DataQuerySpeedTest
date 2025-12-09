using MessagePack;

namespace Protocols.WebSocketProtocol.DataSerializers;

internal sealed class MessagePackWebSocketDataSerializer : IWebSocketDataSerializer
{
	public byte[] Serialize<T>(T data, CancellationToken cancellationToken)
		=> MessagePackSerializer.Serialize(data, cancellationToken: cancellationToken);

	public Task SerializeAsync<T>(T data, Stream stream, CancellationToken cancellationToken)
		=> MessagePackSerializer.SerializeAsync(stream, data, cancellationToken: cancellationToken);

	public ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken)
		=> MessagePackSerializer.DeserializeAsync<T?>(stream, cancellationToken: cancellationToken);
}
