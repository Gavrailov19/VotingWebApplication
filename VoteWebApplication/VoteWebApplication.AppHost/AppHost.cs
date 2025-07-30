var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.VoteWebApplication_ApiService>("Testing-Api")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.VoteWebApplication_Web>("Voting-Web-Page")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
