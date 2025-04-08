using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;

using DataQuerySpeedTest.ServiceDefaults.Models;

using Microsoft.AspNetCore.Http;

using Tester.Core;

namespace Tester.Core.Modules;

public class RestModule(HttpClient httpClient) : IModule
{
	private const string _resourceName = "rest";

	private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

	public ValueTask<long?> ExecuteGetAsync(CancellationToken cancellationToken)
		=> ExecuteAsync(
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
		=> ExecuteAsync(
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

	private async ValueTask<long?> ExecuteAsync(
		Func<HttpRequestMessage> requestFactory,
		CancellationToken cancellationToken)
	{
		using HttpRequestMessage request = requestFactory();

		long requestSize = 0L;
		if (request.Content is not null)
		{
			Stream requestStream = await request.Content
				.ReadAsStreamAsync(cancellationToken)
				.ConfigureAwait(false);
			requestSize = requestStream.Length;
		}

		HttpResponseMessage response = await _httpClient
			.SendAsync(request, cancellationToken)
			.ConfigureAwait(false);

		string json = await response.Content
			.ReadAsStringAsync(cancellationToken)
			.ConfigureAwait(false);

		long responseSize = Encoding.UTF8.GetByteCount(json);

		return requestSize + responseSize;
	}
}
