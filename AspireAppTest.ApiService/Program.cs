using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; // Add this using directive for EF Core extensions
//using OpenTelemetry.Instrumentation.EntityFrameworkCore;
using OpenTelemetry.Logs;
using AspireAppTest.ApiService.Data; 
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;


var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContextCoffee>(options => options.UseInMemoryDatabase("CoffeeShop"));
builder.Services.AddSwaggerGen();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("CoffeeShop"))
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
        //.AddRuntimeInstrumentation()
        //metrics.AddOtlpExporter();
    })
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
        //.AddEntityFrameworkCoreInstrumentation(); // Ensure the package is installed
        //tracing.AddOtlpExporter();
    });
    //.WithLogging(log => log.AddConsoleExporter());

//builder.Logging.AddOpenTelemetry(log => log.AddConsoleExporter());

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapPost("coffee", (CoffeeType _coffeeType, AppDbContextCoffee _dbContext, ILogger<Program> _logger) =>
{
    if (!Enum.IsDefined(_coffeeType))
    {
        _logger.LogWarning("Invalid {CoffeeType}", _coffeeType);
        return Results.BadRequest("Invalid coffee type");
    }
    var _entry = _dbContext.Sales.Add(new Sale
    {
        Id = Guid.NewGuid(),
        CoffeeType = _coffeeType,
        CreatedAt = DateTime.UtcNow
    });
    _dbContext.SaveChanges();
    _logger.LogInformation("Successfully created {@Sale}", _entry.Entity);
    return Results.Ok(_entry.Entity.Id);
});

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
}).WithName("GetWeatherForecast");
app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
