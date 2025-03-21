using Server.QueryBuilders.HttpQueries;
using Server.QueryBuilders.WebSocketQueries;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMediator();

WebApplication app = builder.Build();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.UseWebSockets();

app
	.MapRestEndpoints()
	.MapWebSocketEndpoints();

await app.RunAsync().ConfigureAwait(false);
