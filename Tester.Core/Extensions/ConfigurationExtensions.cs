using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tester.Core.Extensions;

public static class ConfigurationExtensions
{
	public static Uri GetHttpsServerEndpont(this IConfiguration configuration)
	{
		ArgumentNullException.ThrowIfNull(configuration);

		return configuration.GetValue<Uri>("HttpsServerEndpont")
			?? throw new InvalidOperationException("В конфигурации не указан адрес сервера.");
	}

	public static Uri GetWebSocketHost(this IConfiguration configuration)
	{
		ArgumentNullException.ThrowIfNull(configuration);

		Uri httpsServerEndpont = configuration.GetHttpsServerEndpont();

		return new UriBuilder(httpsServerEndpont)
		{
			Scheme = "wss"
		}.Uri;
	}

	public static IServiceCollection ConfigureHttpClient(this IServiceCollection services)
	{
		ArgumentNullException.ThrowIfNull(services);

		return services.ConfigureHttpClientDefaults(static httpClientBuilder
			=> httpClientBuilder.ConfigureHttpClient(static client
			=> client.BaseAddress = new("https+http://Server")));
	}
}
