namespace Tester.TestSuites;

#pragma warning disable CA1812
internal sealed class RestTestSuite(
	IConfiguration configuration,
	HttpClient httpClient)
	: TestSuiteBase(configuration)
{
	private readonly HttpClient _httpClient = httpClient;

	public override string Name { get; } = "REST";

	protected override ValueTask<IModule> CreateModuleAsync(
		string scenarioName,
		CancellationToken cancellationToken)
	{
		IModule module = new RestModule(_httpClient);
		return ValueTask.FromResult(module);
	}
}
