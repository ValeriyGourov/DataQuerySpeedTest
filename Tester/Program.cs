using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Tester.Core.Extensions;
using Tester.TestSuites;

HostApplicationBuilder builder = new(args);

builder.AddServiceDefaults();

builder.Services.ConfigureHttpClient();

builder.Services
	.AddSingleton<TestSuiteBase, RestTestSuite>()
	.AddHttpClient<RestTestSuite>();

builder.Services.AddSingleton<TestSuiteBase, JsonWebSocketSuite>();
builder.Services.AddSingleton<TestSuiteBase, MessagePackWebSocketSuite>();

IHost app = builder.Build();

foreach (TestSuiteBase testSuite in app.Services.GetServices<TestSuiteBase>())
{
	await testSuite.RunAsync().ConfigureAwait(false);
}
