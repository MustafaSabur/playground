var builder = DistributedApplication.CreateBuilder(args);

// Add password parameter for SQL Server
var password = builder.AddParameter("sql-password", secret: true);

// Build and use custom SQL Server 2025 image with full-text search
var sqlServer = builder.AddContainer("sqlserver", "sqlserver-2025-fts")
    .WithDockerfile("Dockerfile.sqlserver")
    .WithEnvironment("SA_PASSWORD", password)
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("MSSQL_AGENT_ENABLED", "true")
    .WithEndpoint(1433, 1433, "sql")
    .WithLifetime(ContainerLifetime.Persistent);

var apiService = builder.AddProject<Projects.AIPlayground_ApiService>("apiservice");

builder.AddProject<Projects.AIPlayground_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
