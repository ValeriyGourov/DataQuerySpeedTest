using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Configuration;

using NBomber;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.WebSockets;

using WebSocket = NBomber.WebSockets.WebSocket;

namespace Tester.TestSuites;

#pragma warning disable CA1812
internal sealed class WebSocketSuite : TestSuiteBase
{
	private const string _resourceName = "WebSocket";
	private readonly Uri _host;

	public WebSocketSuite(IConfiguration configuration)
		: base(configuration)
	{
		Uri httpsServerEndpont = configuration.GetValue<Uri>("HttpsServerEndpont")
			?? throw new InvalidOperationException("В конфигурации не указан адрес сервера.");

		_host = new UriBuilder(httpsServerEndpont)
		{
			Scheme = "wss"
		}.Uri;
	}

	public override string Name { get; } = "WebSocket";

	private void RunScenario<T>(string scenarioName, T request, bool receiveResponse)
	{
		Uri address = new(_host, $"{_resourceName}/{scenarioName}");

		using ClientPool<WebSocket> clientPool = new();

		ScenarioProps scenario = Scenario
			.Create(
				scenarioName,
				async context =>
				{
					WebSocket webSocket = clientPool.GetClient(context.ScenarioInfo);

					string requestJson = JsonSerializer.Serialize(request);

					await webSocket
						.Send(requestJson)
						.ConfigureAwait(false);

					if (receiveResponse)
					{
						WebSocketResponse response = await webSocket
							.Receive()
							.ConfigureAwait(false);
						string responceJson = Encoding.UTF8.GetString(response.Data.Span);

						return Response.Ok(
							payload: responceJson,
							sizeBytes: response.Data.Length);
					}
					else
					{
						return Response.Ok();
					}
				})
			.WithInit(async _ =>
			{
				WebSocket webSocket = new(new());

				await webSocket
					.Connect(address)
					.ConfigureAwait(false);

				clientPool.AddClient(webSocket);
			})
			.WithClean(_ =>
			{
				clientPool.DisposeClients();
				return Task.CompletedTask;
			});

		RunNBomberRunner(scenario);
	}

	protected override void RunGetScenario() => RunScenario(
		ScenarioNames.Get,
		new { Id = GetDataId() },
		true);

	protected override void RunGetAllScenario() => RunScenario(
		ScenarioNames.GetAll,
		new { PageSize = DefaultPageSize },
		true);

	protected override void RunCreateScenario() => RunScenario(
		ScenarioNames.Create,
		CreateOrder(),
		false);
}
