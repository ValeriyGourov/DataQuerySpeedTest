namespace Tester.TestSuites;

#pragma warning disable CA1812
internal sealed class MessagePackWebSocketSuite(IConfiguration configuration)
	: WebSocketSuiteBase<MessagePackWebSocketModule>(configuration)
{
	public override string Name { get; } = RequestTypeNames.WebSocketMessagePack;
}
