using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;

using DataQuerySpeedTest.ServiceDefaults.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace Tester.TestSuites;

#pragma warning disable CA1812
internal sealed class HttpTestSuite(
	IConfiguration configuration,
	HttpClient httpClient)
	: TestSuiteBase(configuration)
{
	private const string _resourceName = "rest";

	private readonly HttpClient _httpClient = httpClient;

	public override string Name { get; } = "HTTP";

	private void RunScenario(
		string scenarioName,
		Func<HttpRequestMessage> requestFactory)
	{
		ScenarioProps scenario = Scenario.Create(
			scenarioName,
			async _ =>
			{
				using HttpRequestMessage request = requestFactory();

				return await Http
					.Send(_httpClient, request)
					.ConfigureAwait(false);
			});

		RunNBomberRunner(scenario);
	}

	protected override void RunGetScenario() => RunScenario(
		ScenarioNames.Get,
		() =>
		{
			HttpRequestMessage request = new(
				HttpMethod.Get,
				new Uri(
					$"{_resourceName}/{GetDataId()}",
					UriKind.Relative));
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

			return request;
		});

	protected override void RunGetAllScenario() => RunScenario(
		ScenarioNames.GetAll,
		() =>
		{
			QueryString queryString = QueryString.Create(
				nameof(GetAllQuery.PageSize),
				DefaultPageSize.ToString(CultureInfo.InvariantCulture));

			HttpRequestMessage request = new(
				HttpMethod.Get,
				new Uri(
					_resourceName + queryString,
					UriKind.Relative));
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

			return request;
		});

	protected override void RunCreateScenario() => RunScenario(
		ScenarioNames.Create,
		() => new HttpRequestMessage(
			HttpMethod.Post,
			new Uri(_resourceName, UriKind.Relative))
		{
			Content = JsonContent.Create(NewCreateCommand())
		});
}
