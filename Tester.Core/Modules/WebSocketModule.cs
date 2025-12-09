using System.Net.WebSockets;

using DataQuerySpeedTest.ServiceDefaults.Models;

using Protocols.WebSocketProtocol;

namespace Tester.Core.Modules;

public sealed class WebSocketModule : IModule, IAsyncDisposable
{
	private const string _resourceName = "WebSocket";
	private readonly ClientWebSocket _client;
	private readonly IWebSocketSubProtocol _subProtocol;

	private WebSocketModule(ClientWebSocket client, IWebSocketSubProtocol subProtocol)
	{
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_subProtocol = subProtocol ?? throw new ArgumentNullException(nameof(subProtocol));
	}

	public override string ToString() => $"WebSocket {_subProtocol.Name}";

	public static async ValueTask<WebSocketModule> CreateAsync(
		Uri host,
		string endpointName,
		IWebSocketSubProtocol subProtocol,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(host);
		ArgumentNullException.ThrowIfNull(subProtocol);
		ArgumentException.ThrowIfNullOrWhiteSpace(endpointName);

		Uri address = new(host, $"{_resourceName}/{endpointName}");

		ClientWebSocket client = new();
		client.Options.AddSubProtocol(subProtocol.SubProtocol);

		client.Options.SetRequestHeader(HeaderNames.DataProtocol, subProtocol.DataProtocol);

		await client
			.ConnectAsync(address, cancellationToken)
			.ConfigureAwait(false);

		return new(client, subProtocol);
	}

	public ValueTask<long?> ExecuteGetAsync(CancellationToken cancellationToken)
		=> ExecuteAsync<GetQuery, Order>(DataFactory.NewGetQuery(), cancellationToken);

	public ValueTask<long?> ExecuteGetAllAsync(CancellationToken cancellationToken)
		=> ExecuteAsync<GetAllQuery, IEnumerable<Order>>(DataFactory.NewGetAllQuery(), cancellationToken);

	public ValueTask<long?> ExecuteCreateAsync(CancellationToken cancellationToken)
		=> ExecuteAsync(DataFactory.NewCreateCommand(), cancellationToken);

	private async ValueTask<long?> ExecuteAsync<TRequest>(
		TRequest request,
		CancellationToken cancellationToken)
	{
		await _subProtocol
			.SendAsync(request, _client, cancellationToken)
			.ConfigureAwait(false);

		return null;
	}

	private async ValueTask<long?> ExecuteAsync<TRequest, TResponse>(
		TRequest request,
		CancellationToken cancellationToken)
	{
		_ = await ExecuteAsync(request, cancellationToken)
			.ConfigureAwait(false);

		_ = await _subProtocol
			.ReceiveAsync<TResponse>(_client, cancellationToken)
			.ConfigureAwait(false);

		return null;
	}

	public async ValueTask DisposeAsync()
	{
		if (_client.State is WebSocketState.Open)
		{
			await _client
				.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None)
				.ConfigureAwait(false);
		}

		_client.Dispose();
	}
}
