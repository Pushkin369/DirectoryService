using DirectoryService.Infrastructure.Postgres;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddHealthChecks();

builder.Services.AddPersistenceLayer(builder.Configuration);
    
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.MapHealthChecks("/health");

if (!app.Environment.IsProduction())
{
    app.MapOpenApi();              
    app.MapScalarApiReference();   
}

await app.RunAsync();
