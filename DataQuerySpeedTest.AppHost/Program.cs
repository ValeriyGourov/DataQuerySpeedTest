IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Server>("Server");

await builder.Build().RunAsync().ConfigureAwait(false);
