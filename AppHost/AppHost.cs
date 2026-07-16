var builder = DistributedApplication.CreateBuilder(args);
// add backing services below.
// var sqlserver = builder
//     .AddSqlServer("sqlserver")
//     .WithDataVolume()
//     .WithLifetime(ContainerLifetime.Persistent);
//
// var catalogDb = sqlserver.AddDatabase("catalogdb");


var postgres = builder
    .AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = postgres.AddDatabase("catalogdb");

//projects
builder.AddProject<Projects.Catalog>("catalog")
    .WithReference(catalogDb)
    .WaitFor(catalogDb);

builder.AddProject<Projects.Cart>("cart");

builder.Build().Run();

// add backing services below.

