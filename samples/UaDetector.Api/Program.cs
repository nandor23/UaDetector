using Scalar.AspNetCore;
using UaDetector;
using UaDetector.MemoryCache;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddUaDetector(options =>
{
    options.UseMemoryCache();
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(cfg =>
{
    cfg.Title = "UaDetector";
    cfg.Theme = ScalarTheme.Moon;
    cfg.ShowSidebar = true;
    cfg.HideDarkModeToggle = true;
});

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
