using Scalar.AspNetCore;

using UaDetector;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddSingleton<IUaDetector>(new UaDetector.UaDetector());

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
