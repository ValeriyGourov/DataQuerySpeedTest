using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Tester.TestSuites;

HostApplicationBuilder builder = new(args);

builder.AddServiceDefaults();

builder.Services.ConfigureHttpClientDefaults(static httpClientBuilder
	=> httpClientBuilder.ConfigureHttpClient(static client
		=> client.BaseAddress = new("https+http://Server")));

builder.Services
	.AddSingleton<TestSuiteBase, HttpTestSuite>()
	.AddHttpClient<HttpTestSuite>();

IHost app = builder.Build();

foreach (TestSuiteBase testSuite in app.Services.GetServices<TestSuiteBase>())
{
	testSuite.Run();
}
