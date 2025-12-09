using Protocols.WebSocketProtocol;

using Tester.Core.Extensions;

namespace Tester.Load.TestSuites;

internal sealed class WebSocketSuite(string name, IConfiguration configuration, IWebSocketSubProtocol subProtocol)
	: TestSuiteBase(configuration)
{
	private readonly Uri _host = configuration.GetWebSocketHost();
	private readonly IWebSocketSubProtocol _subProtocol = subProtocol;

	public override string Name { get; } = name;

	protected override async ValueTask<IModule> CreateModuleAsync(
		string scenarioName,
		CancellationToken cancellationToken)
		=> await WebSocketModule
			.CreateAsync(_host, scenarioName, _subProtocol, cancellationToken)
			.ConfigureAwait(false);
}
