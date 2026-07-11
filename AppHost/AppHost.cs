var builder = DistributedApplication.CreateBuilder(args);


// add cloud native backing services below.
builder.AddProject<Projects.Catalog>("catalog");

builder.Build().Run();