using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Protocols.WebSocketProtocol;

using Tester.Core.Extensions;
using Tester.Load.TestSuites;

HostApplicationBuilder builder = new(args);

builder.AddServiceDefaults();

builder.Services.ConfigureHttpClient();

builder.Services
	.AddSingleton<TestSuiteBase, RestTestSuite>()
	.AddHttpClient<RestTestSuite>();

builder.Services
	.AddWebSocketTestSuite(RequestTypeNames.WebSocketClassicJson, JsonWebSocketSubProtocol.WithClassicDataProtocol)
	.AddWebSocketTestSuite(RequestTypeNames.WebSocketModernJson, JsonWebSocketSubProtocol.WithModernDataProtocol)
	.AddWebSocketTestSuite(RequestTypeNames.WebSocketClassicMessagePack, MessagePackWebSocketSubProtocol.WithClassicDataProtocol)
	.AddWebSocketTestSuite(RequestTypeNames.WebSocketModernMessagePack, MessagePackWebSocketSubProtocol.WithModernDataProtocol);

builder.Services
	.AddSingleton<TestSuiteBase, GrpcTestSuite>()
	.AddHttpClient<GrpcTestSuite>();

IHost app = builder.Build();

foreach (TestSuiteBase testSuite in app.Services.GetServices<TestSuiteBase>())
{
	await testSuite.RunAsync().ConfigureAwait(false);
}

#pragma warning disable S3903, RCS1110
static file class ServiceCollectionExtensions
{
	extension(IServiceCollection services)
	{
		public IServiceCollection AddWebSocketTestSuite(string name, IWebSocketSubProtocol subProtocol)
			=> services.AddSingleton<TestSuiteBase>(services => new WebSocketSuite(
				name,
				services.GetRequiredService<IConfiguration>(),
				subProtocol));
	}
}
#pragma warning restore S3903, RCS1110
