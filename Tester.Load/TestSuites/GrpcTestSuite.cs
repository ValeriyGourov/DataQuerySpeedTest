using Tester.Core.Extensions;

namespace Tester.Load.TestSuites;

#pragma warning disable CA1812
internal sealed class GrpcTestSuite(
	IConfiguration configuration,
	HttpClient httpClient)
	: TestSuiteBase(configuration)
{
	private readonly Uri _host = configuration.GetHttpsServerEndpont();
	private readonly HttpClient _httpClient = httpClient;

	public override string Name { get; } = RequestTypeNames.Grpc;

	protected override ValueTask<IModule> CreateModuleAsync(
		string scenarioName,
		CancellationToken cancellationToken)
	{
#pragma warning disable CA2000
		IModule module = new GrpcModule(_host, _httpClient);
#pragma warning restore CA2000
		return ValueTask.FromResult(module);
	}
}
