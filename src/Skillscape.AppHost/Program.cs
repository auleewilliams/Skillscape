var builder = DistributedApplication.CreateBuilder(args);

var apiservice = builder.AddProject<Projects.Skillscape_ApiService>("apiservice");

builder.AddProject<Projects.Skillscape_Web>("webfrontend")
    .WithReference(apiservice);

builder.Build().Run();
