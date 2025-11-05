using DataQuerySpeedTest.ServiceDefaults.Serialization;

using ProtoBuf.Grpc.Server;

using Server.QueryBuilders;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMediator();

builder.Services.AddCodeFirstGrpc(options
	=> options.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal);

if (builder.Environment.IsDevelopment())
{
	builder.Services.AddCodeFirstGrpcReflection();
}

builder.Services.ConfigureHttpJsonOptions(options =>
{
	options.SerializerOptions.TypeInfoResolverChain.Insert(0, ModelsJsonSerializerContext.Default);
	options.SerializerOptions.PropertyNamingPolicy = ModelsJsonSerializerContext.Default.Options.PropertyNamingPolicy;
});

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

app.UseWebSockets();

app
	.MapRestEndpoints()
	.MapWebSocketEndpoints();

if (app.Environment.IsDevelopment())
{
	app.MapCodeFirstGrpcReflectionService();
}

app.MapGrpcService<GrpcService>();

await app.RunAsync().ConfigureAwait(false);
