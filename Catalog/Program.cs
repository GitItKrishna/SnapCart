using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<ProductDbContext>(connectionName:"catalogdb");
builder.Services.AddScoped<ProductService>();
builder.Services.AddFastEndpoints();
var app = builder.Build();
// Configure the HTTP request pipeline.
 app.MapDefaultEndpoints();
app.UseFastEndpoints();
app.UseMigration();
app.UseHttpsRedirection();

app.Run();
