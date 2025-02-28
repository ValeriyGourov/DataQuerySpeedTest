using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;

using Microsoft.Extensions.Configuration;

using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace Tester.TestSuites;

internal sealed class HttpTestSuite(
	IConfiguration configuration,
	HttpClient httpClient)
	: TestSuiteBase(configuration)
{
	private const string _resourceName = "rest";

	private readonly HttpClient _httpClient = httpClient;

	public override string Name { get; } = "HTTP";

	protected override ScenarioProps Get => Scenario.Create(
		GetScenarioName(),
		async _ =>
		{
			HttpRequestMessage request = new(
				HttpMethod.Get,
				new Uri($"{_resourceName}/1", UriKind.Relative));
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

			return await Http
				.Send(_httpClient, request)
				.ConfigureAwait(false);
		});

	protected override ScenarioProps GetAll => Scenario.Create(
		GetScenarioName(),
		async _ =>
		{
			HttpRequestMessage request = new(
				HttpMethod.Get,
				new Uri(_resourceName, UriKind.Relative));
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

			return await Http
				.Send(_httpClient, request)
				.ConfigureAwait(false);
		});

	protected override ScenarioProps Create => Scenario.Create(
		GetScenarioName(),
		async _ =>
		{
			HttpRequestMessage request = new(
				HttpMethod.Post,
				new Uri(_resourceName, UriKind.Relative))
			{
				Content = JsonContent.Create(new
				{
					ProductName = "Товар 123",
					Quantity = 12.3m
				})
			};

			return await Http
				.Send(_httpClient, request)
				.ConfigureAwait(false);
		});
}
