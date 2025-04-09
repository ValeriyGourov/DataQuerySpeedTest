IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> timescale = builder
	.AddPostgres("Timescale")
	.WithImage("timescale/timescaledb")
	.WithImageTag("latest-pg17")
	.WithDataVolume("NBomber-Studio-data")
	.WithPgAdmin()
	.WithPgWeb();

IResourceBuilder<PostgresDatabaseResource> timescaleDB = timescale.AddDatabase("TimescaleDB");

timescale.WithEnvironment(
	"POSTGRES_DB",
	timescaleDB.Resource.DatabaseName);

builder
	.AddContainer("NBomberStudio", "nbomberdocker/nbomber-studio")
	.WithHttpEndpoint(targetPort: 8080)
	.WithEnvironment("DBSETTINGS:CONNECTIONSTRING", timescaleDB)
	.WaitFor(timescaleDB);

IResourceBuilder<ProjectResource> server = builder.AddProject<Projects.Server>(nameof(Projects.Server));

EndpointReference httpsServerEndpont = server.GetEndpoint("https");

builder
	.AddProject<Projects.Tester_Load>("Tester-Load")
	.WaitFor(server)
	.WithReference(server)
	.WithReference(timescaleDB)
	.WithEnvironment("HttpsServerEndpont", httpsServerEndpont)
	.WithExplicitStart();

builder
	.AddProject<Projects.Tester_Benchmark>("Tester-Benchmark")
	.WaitFor(server)
	.WithReference(server)
	.WithEnvironment("HttpsServerEndpont", httpsServerEndpont)
	.WithExplicitStart();

await builder.Build().RunAsync().ConfigureAwait(false);
