using UaDetector;

var builder = WebApplication.CreateBuilder(args);

GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

var memoryBefore = GC.GetTotalMemory(true);

builder.Services.AddUaDetector();

builder.Build();

// Wait until regular expressions are loaded into memory
await Task.Delay(TimeSpan.FromSeconds(20));

GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

var memoryAfter = GC.GetTotalMemory(true);
var memoryUsed = (memoryAfter - memoryBefore) / 1024.0 / 1024.0;

Console.WriteLine($"Memory used: {memoryUsed:F2} MB");
