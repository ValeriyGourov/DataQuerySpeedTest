namespace Tester.TestSuites;

internal abstract class WebSocketSuiteBase<TModule> : TestSuiteBase
	where TModule : IWebSocketModule
{
	private readonly Uri _host;

	protected WebSocketSuiteBase(IConfiguration configuration)
		: base(configuration)
	{
		Uri httpsServerEndpont = configuration.GetValue<Uri>("HttpsServerEndpont")
			?? throw new InvalidOperationException("В конфигурации не указан адрес сервера.");

		_host = new UriBuilder(httpsServerEndpont)
		{
			Scheme = "wss"
		}.Uri;
	}

	protected override async ValueTask<IModule> CreateModuleAsync(
		string scenarioName,
		CancellationToken cancellationToken)
		=> await TModule
			.CreateAsync(_host, scenarioName, cancellationToken)
			.ConfigureAwait(false);
}
