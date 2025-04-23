using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

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
