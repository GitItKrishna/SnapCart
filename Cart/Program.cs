var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();
builder.AddRedisDistributedCache(connectionName: "cache"); //this looks for a connection string with the name "connectionstrings_cache"
builder.Services.AddScoped<CartService>();
builder.Services.AddHttpClient<CatalogApiClient>(client =>
{
    client.BaseAddress = new Uri("https+http://catalog/");
});
builder.Services.AddFastEndpoints(config =>
{
    config.Assemblies = new[] { typeof(Program).Assembly };
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseFastEndpoints();

app.Run();
