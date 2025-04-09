using Tester.Core.Extensions;

namespace Tester.TestSuites;

internal abstract class WebSocketSuiteBase<TModule>(IConfiguration configuration)
	: TestSuiteBase(configuration)
	where TModule : IWebSocketModule
{
	private readonly Uri _host = configuration.GetWebSocketHost();

	protected override async ValueTask<IModule> CreateModuleAsync(
		string scenarioName,
		CancellationToken cancellationToken)
		=> await TModule
			.CreateAsync(_host, scenarioName, cancellationToken)
			.ConfigureAwait(false);
}
