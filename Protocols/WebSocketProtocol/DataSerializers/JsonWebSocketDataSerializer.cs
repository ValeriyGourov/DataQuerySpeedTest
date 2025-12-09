using System.Text.Json;

using DataQuerySpeedTest.ServiceDefaults.Serialization;

namespace Protocols.WebSocketProtocol.DataSerializers;

internal sealed class JsonWebSocketDataSerializer : IWebSocketDataSerializer
{
	public byte[] Serialize<T>(T data, CancellationToken cancellationToken) => JsonSerializer.SerializeToUtf8Bytes<T>(data);

	public Task SerializeAsync<T>(T data, Stream stream, CancellationToken cancellationToken)
		=> JsonSerializer.SerializeAsync(
			stream,
			data,
			ModelsJsonSerializerContext.Default.Options,
			cancellationToken);

	public ValueTask<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken)
	{
		/*
		 * TODO: В .NET 9 использование JsonSerializerContext для преобразования в классы, у которых
		 * свойства объявлены с ключевым словом required, не оптимизировано и приводит к более медленному
		 * выполнению преобразования и дополнительному выделению памяти. Поэтому временно используется
		 * стандартный метод преобразования.
		 * https://github.com/dotnet/runtime/issues/97612
		 *
		 * В идеале метод должен вызываться так: JsonSerializer.DeserializeAsync<T>(stream, ModelsJsonSerializerContext.Default.Options, cancellationToken)
		 */
		return JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
	}
}
