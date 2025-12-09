IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> timescale = builder
	.AddPostgres("Timescale")
	.WithIconName("Database")
	.WithImage("timescale/timescaledb")
	.WithImageTag("latest-pg17")
	.WithDataVolume("NBomber-Studio-data");
timescale
	.WithPgAdmin(container => container.WithDbAdminConsoleSettings(timescale))
	.WithPgWeb(container => container.WithDbAdminConsoleSettings(timescale));

IResourceBuilder<PostgresDatabaseResource> timescaleDB = timescale.AddDatabase("TimescaleDB");

timescale.WithEnvironment(
	"POSTGRES_DB",
	timescaleDB.Resource.DatabaseName);

builder
	.AddContainer("NBomberStudio", "nbomberdocker/nbomber-studio", "0.5.2")
	.WithHttpEndpoint(targetPort: 8080)
	.WithEnvironment("POSTGRESQL__CONNECTIONSTRING", timescaleDB)
	.WaitFor(timescaleDB)
	.WithParentRelationship(timescale);

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

IResourceBuilder<ProjectResource> testerLoad = builder
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

timescale.WithParentRelationship(testerLoad);

await builder.Build().RunAsync().ConfigureAwait(false);

#pragma warning disable S3903, RCS1110
static file class DbResourceExtensions
{
	extension<T>(IResourceBuilder<T> container)
		where T : ContainerResource
	{
		public IResourceBuilder<T> WithDbAdminConsoleSettings(
			IResourceBuilder<IResource> parent)
			=> container
				.WithImageTag("latest")
				.WithLifetime(ContainerLifetime.Persistent)
				.WithIconName("DatabaseWindow")
				.WithParentRelationship(parent);
	}
}
#pragma warning restore S3903, RCS1110
