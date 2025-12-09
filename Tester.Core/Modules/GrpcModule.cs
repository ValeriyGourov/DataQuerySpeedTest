using DataQuerySpeedTest.ServiceDefaults.ServiceAbstractions;

using Grpc.Net.Client;

using ProtoBuf.Grpc.Client;

namespace Tester.Core.Modules;

public sealed class GrpcModule : IModule, IDisposable
{
	private bool disposedValue;
	private readonly GrpcChannel _channel;
	private readonly IOrderService _client;

	public GrpcModule(Uri host, HttpClient httpClient)
	{
		ArgumentNullException.ThrowIfNull(host);
		ArgumentNullException.ThrowIfNull(httpClient);

		_channel = GrpcChannel.ForAddress(
			host,
			new() { HttpClient = httpClient });

		_client = _channel.CreateGrpcService<IOrderService>();
	}

	public override string ToString() => "gRPC";

	public async ValueTask<long?> ExecuteGetAsync(CancellationToken cancellationToken)
	{
		_ = await _client
			.GetAsync(DataFactory.NewGetQuery())
			.ConfigureAwait(false);

		return null;
	}

	public async ValueTask<long?> ExecuteGetAllAsync(CancellationToken cancellationToken)
	{
		_ = await _client
			.GetAllAsync(DataFactory.NewGetAllQuery())
			.ConfigureAwait(false);

		return null;
	}

	public async ValueTask<long?> ExecuteCreateAsync(CancellationToken cancellationToken)
	{
		await _client
			.CreateAsync(DataFactory.NewCreateCommand())
			.ConfigureAwait(false);

		return null;
	}

	private void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				// Освобождение управляемого состояния (управляемые объекты).

				_channel.Dispose();
			}

			// Освобождение неуправляемыъ ресурсов (неуправляемые объекты).
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
