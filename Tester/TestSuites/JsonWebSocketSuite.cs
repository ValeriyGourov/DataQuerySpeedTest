using System.Text.Json;

using Microsoft.Extensions.Configuration;

namespace Tester.TestSuites;

#pragma warning disable CA1812
internal sealed class JsonWebSocketSuite(IConfiguration configuration)
	: WebSocketSuiteBase(configuration, "json")
{
	public override string Name { get; } = "WebSocket-JSON";

	protected override byte[] Serialize<T>(T request)
	{
		using MemoryStream memoryStream = new();
		JsonSerializer.Serialize(memoryStream, request);

		return memoryStream.ToArray();
	}
}
