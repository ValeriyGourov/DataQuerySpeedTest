using System.Net.WebSockets;

using DataQuerySpeedTest.ServiceDefaults.Models;

using Protocols.WebSocketProtocol;

using Tester.Core;

namespace Tester.Core.Modules;

public abstract class WebSocketModuleBase(IWebSocketSubProtocol subProtocol)
	: IModule, IAsyncDisposable
{
	private bool _disposedValue;

	private const string _resourceName = "WebSocket";
#pragma warning disable CA2213
	private readonly ClientWebSocket _client = new();
#pragma warning restore CA2213
	private readonly IWebSocketSubProtocol _subProtocol = subProtocol ?? throw new ArgumentNullException(nameof(subProtocol));

	protected static async ValueTask<WebSocketModuleBase> InitializeAsync(
		WebSocketModuleBase module,
		Uri host,
		string endpointName,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(module);
		ArgumentNullException.ThrowIfNull(host);

		Uri address = new(host, $"{_resourceName}/{endpointName}");
		module._client.Options.AddSubProtocol(module._subProtocol.SubProtocol);

		await module._client
			.ConnectAsync(address, cancellationToken)
			.ConfigureAwait(false);

		return module;
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
		=> await _subProtocol
			.SendAsync(request, _client, cancellationToken)
			.ConfigureAwait(false);

	private async ValueTask<long?> ExecuteAsync<TRequest, TResponse>(
		TRequest request,
		CancellationToken cancellationToken)
	{
		long requestSize = await ExecuteAsync(request, cancellationToken)
			.ConfigureAwait(false) ?? default;

		WebSocketResponse<TResponse> response = await _subProtocol
			.ReceiveAsync<TResponse>(_client, cancellationToken)
			.ConfigureAwait(false);
		long responseSize = response.Size;

		return requestSize + responseSize;
	}

	protected virtual async ValueTask DisposeAsync(bool disposing)
	{
		if (!_disposedValue)
		{
			if (disposing)
			{
				// Освобождение управляемого состояния (управляемые объекты).

				if (_client.State is WebSocketState.Open)
				{
					await _client
						.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None)
						.ConfigureAwait(false);
				}

				_client.Dispose();
			}

			// Освобождение неуправляемыъ ресурсов (неуправляемые объекты).
			_disposedValue = true;
		}
	}

	public async ValueTask DisposeAsync()
	{
		await DisposeAsync(disposing: true).ConfigureAwait(false);
		GC.SuppressFinalize(this);
	}
}
