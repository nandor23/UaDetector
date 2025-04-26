using Scalar.AspNetCore;
using UaDetector;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddUaDetector(x =>
{
    // Custom configuration options
    // e.g., x.VersionTruncation = VersionTruncation.Major;
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(cfg =>
{
    cfg.Title = "UaDetector";
    cfg.Theme = ScalarTheme.BluePlanet;
    cfg.ShowSidebar = true;
});

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
