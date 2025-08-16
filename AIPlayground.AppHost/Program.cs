var builder = DistributedApplication.CreateBuilder(args);

// Build and use custom SQL Server 2025 image with full-text search
var sqlServer = builder.AddSqlServer("sqlserver", port: 1434)
    .WithDockerfile(".", "Dockerfile.sqlserver")
    .WithEndpoint(1433, 1435, name: "sqlserver")

    .WithLifetime(ContainerLifetime.Persistent);

var database = sqlServer.AddDatabase("AIPlayground");

var apiService = builder.AddProject<Projects.AIPlayground_ApiService>("apiservice")
  .WithReference(database)
  .WaitFor(database);

builder.AddProject<Projects.AIPlayground_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
