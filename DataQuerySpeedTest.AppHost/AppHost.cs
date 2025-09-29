IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

const string
	databaseIcon = "Database",
	databaseWindowIcon = "DatabaseWindow";

IResourceBuilder<PostgresServerResource> timescale = builder
	.AddPostgres("Timescale")
	.WithIconName(databaseIcon)
	.WithImage("timescale/timescaledb")
	.WithImageTag("latest-pg17")
	.WithDataVolume("NBomber-Studio-data")
	.WithPgAdmin(ConfigureDbAdminConsole)
	.WithPgWeb(ConfigureDbAdminConsole);

IResourceBuilder<PostgresDatabaseResource> timescaleDB = timescale.AddDatabase("TimescaleDB");

timescale.WithEnvironment(
	"POSTGRES_DB",
	timescaleDB.Resource.DatabaseName);

builder
	.AddContainer("NBomberStudio", "nbomberdocker/nbomber-studio", "0.3.0")
	.WithHttpEndpoint(targetPort: 8080)
	.WithEnvironment("DBSETTINGS:CONNECTIONSTRING", timescaleDB)
	.WithImagePullPolicy(ImagePullPolicy.Always)
	.WaitFor(timescaleDB);

IResourceBuilder<ProjectResource> server = builder
	.AddProject<Projects.Server>(nameof(Projects.Server))
	.WithUrls(static context =>
	{
		IEnumerable<(ResourceUrlAnnotation ResourceUrl, string Scheme, int Index)> urls = (
			from resourceUrl in context.Urls
			let uri = new Uri(resourceUrl.Url)
			orderby uri.Scheme is "https"
			select (ResourceUrl: resourceUrl, uri.Scheme)
			)
			.Select(static (item, index)
				=> (item.ResourceUrl, item.Scheme, Index: index));
		foreach ((ResourceUrlAnnotation resourceUrl, string scheme, int index) in urls)
		{
			resourceUrl.DisplayText = scheme.ToUpperInvariant();
			resourceUrl.DisplayOrder = index;
		}
	});

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

static void ConfigureDbAdminConsole(IResourceBuilder<ContainerResource> container) => container
	.WithImageTag("latest")
	.WithLifetime(ContainerLifetime.Persistent)
	.WithIconName(databaseWindowIcon);
