namespace Protocols.WebSocketProtocol.DataSerializers;

internal interface IWebSocketDataSerializer
{
	Task SerializeAsync<T>(T data, Stream stream, CancellationToken cancellationToken);
	byte[] Serialize<T>(T data, CancellationToken cancellationToken);
	ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken);
}
