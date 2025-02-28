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

IResourceBuilder<ContainerResource> nBomberStudio = builder
	.AddContainer("NBomberStudio", "nbomberdocker/nbomber-studio")
	.WithHttpEndpoint(targetPort: 8080)
	.WithEnvironment("DBSETTINGS:CONNECTIONSTRING", timescaleDB)
	.WaitFor(timescaleDB);

IResourceBuilder<ProjectResource> server = builder.AddProject<Projects.Server>(nameof(Projects.Server));

builder
	.AddProject<Projects.Tester>(nameof(Projects.Tester))
	.WaitFor(server)
	.WaitFor(nBomberStudio)
	.WithReference(server)
	.WithReference(timescaleDB);

await builder.Build().RunAsync().ConfigureAwait(false);
