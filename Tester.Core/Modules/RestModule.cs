using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;

using DataQuerySpeedTest.ServiceDefaults.Models;

using Microsoft.AspNetCore.Http;

namespace Tester.Core.Modules;

public sealed class RestModule(HttpClient httpClient) : IModule
{
	private const string _resourceName = "rest";

	private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

	public ValueTask<long?> ExecuteGetAsync(CancellationToken cancellationToken)
		=> ExecuteAsync<Order>(
			static () =>
			{
				HttpRequestMessage request = new(
					HttpMethod.Get,
					new Uri(
						$"{_resourceName}/{DataFactory.GetDataId()}",
						UriKind.Relative));
				request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

				return request;
			},
			cancellationToken);

	public ValueTask<long?> ExecuteGetAllAsync(CancellationToken cancellationToken)
		=> ExecuteAsync<IEnumerable<Order>>(
			static () =>
			{
				QueryString queryString = QueryString.Create(
					nameof(GetAllQuery.PageSize),
					DataFactory.DefaultPageSize.ToString(CultureInfo.InvariantCulture));

				HttpRequestMessage request = new(
					HttpMethod.Get,
					new Uri(
						_resourceName + queryString,
						UriKind.Relative));
				request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

				return request;
			},
			cancellationToken);

	public ValueTask<long?> ExecuteCreateAsync(CancellationToken cancellationToken)
		=> ExecuteAsync(
			static () => new HttpRequestMessage(
				HttpMethod.Post,
				new Uri(_resourceName, UriKind.Relative))
			{
				Content = JsonContent.Create(DataFactory.NewCreateCommand())
			},
			cancellationToken);

	private async Task<HttpResponseMessage> GetResponseMessageAsync(
		Func<HttpRequestMessage> requestFactory,
		CancellationToken cancellationToken)
	{
		using HttpRequestMessage requestMessage = requestFactory();

		HttpResponseMessage responseMessage = await _httpClient
			.SendAsync(requestMessage, cancellationToken)
			.ConfigureAwait(false);
		responseMessage.EnsureSuccessStatusCode();

		return responseMessage;
	}

	private async ValueTask<long?> ExecuteAsync(
		Func<HttpRequestMessage> requestFactory,
		CancellationToken cancellationToken)
	{
		using HttpResponseMessage responseMessage = await GetResponseMessageAsync(requestFactory, cancellationToken)
			.ConfigureAwait(false);

		return null;
	}

	private async ValueTask<long?> ExecuteAsync<T>(
		Func<HttpRequestMessage> requestFactory,
		CancellationToken cancellationToken)
	{
		using HttpResponseMessage responseMessage = await GetResponseMessageAsync(requestFactory, cancellationToken)
			.ConfigureAwait(false);

#pragma warning disable S1481
		T? response = await responseMessage.Content
			.ReadFromJsonAsync<T>(cancellationToken)
			.ConfigureAwait(false);
#pragma warning restore S1481

		return null;
	}
}
