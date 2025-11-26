#pragma warning disable CA1034 // TODO: Удалить после исправления ошибки анализатора: https://github.com/dotnet/roslyn-analyzers/issues/7765
#pragma warning disable CA1708 // TODO: Удалить после исправления ошибки анализатора: https://github.com/dotnet/roslyn-analyzers/issues/7771

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tester.Core.Extensions;

public static class ConfigurationExtensions
{
	extension(IConfiguration configuration)
	{
		public Uri GetHttpsServerEndpont()
			=> configuration.GetValue<Uri>("HttpsServerEndpont")
				?? throw new InvalidOperationException("В конфигурации не указан адрес сервера.");

		public Uri GetWebSocketHost()
		{
			Uri httpsServerEndpont = configuration.GetHttpsServerEndpont();

			return new UriBuilder(httpsServerEndpont)
			{
				Scheme = "wss"
			}.Uri;
		}
	}

	extension(IServiceCollection services)
	{
		public IServiceCollection ConfigureHttpClient()
			=> services.ConfigureHttpClientDefaults(static httpClientBuilder
				=> httpClientBuilder.ConfigureHttpClient(static client => client.BaseAddress = new("https+http://Server")));
	}
}
