using UADetector.Parsers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IOsParser, OsParser>();
builder.Services.AddSingleton<IBrowserParser, BrowserParser>();


builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
