namespace Tester.Load.TestSuites;

#pragma warning disable CA1812
internal sealed class JsonWebSocketSuite(IConfiguration configuration)
	: WebSocketSuiteBase<JsonWebSocketModule>(configuration)
{
	public override string Name { get; } = RequestTypeNames.WebSocketJson;
}
