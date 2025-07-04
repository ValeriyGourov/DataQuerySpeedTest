﻿#pragma warning disable VSTHRD200, CA1515, CA1001

using System.Diagnostics.CodeAnalysis;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

using DataQuerySpeedTest.ServiceDefaults;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Tester.Core.Extensions;
using Tester.Core.Modules;

namespace Tester.Benchmark;

[MemoryDiagnoser]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class Benchmarks
{
	private Uri? _host;
	private Uri? _webSocketHost;

	private HttpClient? _httpClient;
	private RestModule? _restModule;

	private WebSocketModuleBase? _getJsonWebSocketModule;
	private WebSocketModuleBase? _getAllJsonWebSocketModule;
	private WebSocketModuleBase? _createJsonWebSocketModule;

	private WebSocketModuleBase? _getMessagePackWebSocketModule;
	private WebSocketModuleBase? _getAllMessagePackWebSocketModule;
	private WebSocketModuleBase? _createMessagePackWebSocketModule;

	private GrpcModule? _grpcModule;

	[GlobalSetup]
	public async Task GlobalSetup()
	{
		SetOptions();

		_restModule = new RestModule(_httpClient);

		ValueTask<WebSocketModuleBase> CreateWebSocketModule<T>(string endpointName)
			where T : IWebSocketModule
			=> T.CreateAsync(
				_webSocketHost,
				endpointName,
				CancellationToken.None);

		_getJsonWebSocketModule = await CreateWebSocketModule<JsonWebSocketModule>(ScenarioNames.Get)
			.ConfigureAwait(false);
		_getAllJsonWebSocketModule = await CreateWebSocketModule<JsonWebSocketModule>(ScenarioNames.GetAll)
			.ConfigureAwait(false);
		_createJsonWebSocketModule = await CreateWebSocketModule<JsonWebSocketModule>(ScenarioNames.Create)
			.ConfigureAwait(false);

		_getMessagePackWebSocketModule = await CreateWebSocketModule<MessagePackWebSocketModule>(ScenarioNames.Get)
			.ConfigureAwait(false);
		_getAllMessagePackWebSocketModule = await CreateWebSocketModule<MessagePackWebSocketModule>(ScenarioNames.GetAll)
			.ConfigureAwait(false);
		_createMessagePackWebSocketModule = await CreateWebSocketModule<MessagePackWebSocketModule>(ScenarioNames.Create)
			.ConfigureAwait(false);

		_grpcModule = new GrpcModule(_host, _httpClient);
	}

	[GlobalCleanup]
	public async Task GlobalCleanup()
	{
		_httpClient?.Dispose();

		static ValueTask DisposeAsync(WebSocketModuleBase? webSocketModule)
			=> webSocketModule is not null
				? webSocketModule.DisposeAsync()
				: ValueTask.CompletedTask;

		await DisposeAsync(_getJsonWebSocketModule).ConfigureAwait(false);
		await DisposeAsync(_getAllJsonWebSocketModule).ConfigureAwait(false);
		await DisposeAsync(_createJsonWebSocketModule).ConfigureAwait(false);

		await DisposeAsync(_getMessagePackWebSocketModule).ConfigureAwait(false);
		await DisposeAsync(_getAllMessagePackWebSocketModule).ConfigureAwait(false);
		await DisposeAsync(_createMessagePackWebSocketModule).ConfigureAwait(false);

		_grpcModule?.Dispose();
	}

	[MemberNotNull(
		nameof(_httpClient),
		nameof(_host),
		nameof(_webSocketHost))]
	private void SetOptions()
	{
		HostApplicationBuilder builder = new();

		builder.AddServiceDefaults();

		builder.Services.ConfigureHttpClient();

		IHost app = builder.Build();

		_httpClient = app.Services
			.GetRequiredService<IHttpClientFactory>()
			.CreateClient();

		IConfiguration configuration = app.Services.GetRequiredService<IConfiguration>();
		_host = configuration.GetHttpsServerEndpont();
		_webSocketHost = configuration.GetWebSocketHost();
	}

	private static async Task ExecuteAsync(Func<CancellationToken, ValueTask<long?>> method)
		=> _ = await method(CancellationToken.None).ConfigureAwait(false);

	#region Get

	private static Task ExecuteGetAsync(IModule? module)
	{
#pragma warning disable RCS1256 // Invalid argument null check
		ArgumentNullException.ThrowIfNull(module);
#pragma warning restore RCS1256 // Invalid argument null check
		return ExecuteAsync(module.ExecuteGetAsync);
	}

	[Benchmark(Description = RequestTypeNames.WebSocketJson, Baseline = true)]
	[BenchmarkCategory(ScenarioNames.Get)]
	public Task JsonWebSocketGet() => ExecuteGetAsync(_getJsonWebSocketModule);

	[Benchmark(Description = RequestTypeNames.WebSocketMessagePack)]
	[BenchmarkCategory(ScenarioNames.Get)]
	public Task MessagePackWebSocketGet() => ExecuteGetAsync(_getMessagePackWebSocketModule);

	[Benchmark(Description = RequestTypeNames.Rest)]
	[BenchmarkCategory(ScenarioNames.Get)]
	public Task RestGet() => ExecuteGetAsync(_restModule);

	[Benchmark(Description = RequestTypeNames.Grpc)]
	[BenchmarkCategory(ScenarioNames.Get)]
	public Task GrpcGet() => ExecuteGetAsync(_grpcModule);

	#endregion

	#region GetAll

	private static Task ExecuteGetAllAsync(IModule? module)
	{
#pragma warning disable RCS1256 // Invalid argument null check
		ArgumentNullException.ThrowIfNull(module);
#pragma warning restore RCS1256 // Invalid argument null check
		return ExecuteAsync(module.ExecuteGetAllAsync);
	}

	[Benchmark(Description = RequestTypeNames.WebSocketJson, Baseline = true)]
	[BenchmarkCategory(ScenarioNames.GetAll)]
	public Task JsonWebSocketGetAll() => ExecuteGetAllAsync(_getAllJsonWebSocketModule);

	[Benchmark(Description = RequestTypeNames.WebSocketMessagePack)]
	[BenchmarkCategory(ScenarioNames.GetAll)]
	public Task MessagePackWebSocketGetAll() => ExecuteGetAllAsync(_getAllMessagePackWebSocketModule);

	[Benchmark(Description = RequestTypeNames.Rest)]
	[BenchmarkCategory(ScenarioNames.GetAll)]
	public Task RestGetAll() => ExecuteGetAllAsync(_restModule);

	[Benchmark(Description = RequestTypeNames.Grpc)]
	[BenchmarkCategory(ScenarioNames.GetAll)]
	public Task GrpcGetAll() => ExecuteGetAllAsync(_grpcModule);

	#endregion

	#region Create

	private static Task ExecuteCreateAsync(IModule? module)
	{
#pragma warning disable RCS1256 // Invalid argument null check
		ArgumentNullException.ThrowIfNull(module);
#pragma warning restore RCS1256 // Invalid argument null check
		return ExecuteAsync(module.ExecuteCreateAsync);
	}

	[Benchmark(Description = RequestTypeNames.WebSocketJson, Baseline = true)]
	[BenchmarkCategory(ScenarioNames.Create)]
	public Task JsonWebSocketCreate() => ExecuteCreateAsync(_createJsonWebSocketModule);

	[Benchmark(Description = RequestTypeNames.WebSocketMessagePack)]
	[BenchmarkCategory(ScenarioNames.Create)]
	public Task MessagePackWebSocketCreate() => ExecuteCreateAsync(_createMessagePackWebSocketModule);

	[Benchmark(Description = RequestTypeNames.Rest)]
	[BenchmarkCategory(ScenarioNames.Create)]
	public Task RestCreate() => ExecuteCreateAsync(_restModule);

	[Benchmark(Description = RequestTypeNames.Grpc)]
	[BenchmarkCategory(ScenarioNames.Create)]
	public Task GrpcCreate() => ExecuteCreateAsync(_grpcModule);

	#endregion
}
