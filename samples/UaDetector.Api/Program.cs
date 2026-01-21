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
app.MapScalarApiReference(
    "scalar",
    cfg =>
    {
        cfg.Title = "UaDetector";
        cfg.Theme = ScalarTheme.Moon;
        cfg.ShowSidebar = true;
        cfg.HideDarkModeToggle = true;
    }
);

app.MapGet("/", () => Results.Redirect("/scalar")).ExcludeFromDescription();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
