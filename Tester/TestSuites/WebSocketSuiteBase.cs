using DataQuerySpeedTest.ServiceDefaults.Models;

using Microsoft.Extensions.Configuration;

using NBomber;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.WebSockets;

using WebSocket = NBomber.WebSockets.WebSocket;

namespace Tester.TestSuites;

internal abstract class WebSocketSuiteBase : TestSuiteBase
{
	private const string _resourceName = "WebSocket";
	private readonly Uri _host;
	private readonly string _subProtocol;

	protected WebSocketSuiteBase(IConfiguration configuration, string subProtocol)
		: base(configuration)
	{
		Uri httpsServerEndpont = configuration.GetValue<Uri>("HttpsServerEndpont")
			?? throw new InvalidOperationException("В конфигурации не указан адрес сервера.");

		_host = new UriBuilder(httpsServerEndpont)
		{
			Scheme = "wss"
		}.Uri;

		_subProtocol = subProtocol;
	}

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

					byte[] payload = Serialize(request);

					await webSocket
						.Send(payload)
						.ConfigureAwait(false);

					if (receiveResponse)
					{
						WebSocketResponse response = await webSocket
							.Receive()
							.ConfigureAwait(false);

						return Response.Ok(
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
				webSocket.Client.Options.AddSubProtocol(_subProtocol);

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

	protected abstract byte[] Serialize<T>(T request);

	protected override void RunGetScenario() => RunScenario(
		ScenarioNames.Get,
		new GetQuery(GetDataId()),
		true);

	protected override void RunGetAllScenario() => RunScenario(
		ScenarioNames.GetAll,
		new GetAllQuery(DefaultPageSize),
		true);

	protected override void RunCreateScenario() => RunScenario(
		ScenarioNames.Create,
		CreateOrder(),
		false);
}
