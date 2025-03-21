using MessagePack;

using Microsoft.Extensions.Configuration;

namespace Tester.TestSuites;

#pragma warning disable CA1812
internal sealed class MessagePackWebSocketSuite(IConfiguration configuration)
	: WebSocketSuiteBase(configuration, "message-pack")
{
	public override string Name { get; } = "WebSocket-MessagePack";

	protected override byte[] Serialize<T>(T request) => MessagePackSerializer.Serialize(request);
}
