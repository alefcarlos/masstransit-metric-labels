using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Sample.Entrypoint.Consumers;
using Sample.Entrypoint;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediator();


builder.Services.ConfigureOptions<ConfigureRabbitMqOptions>();
builder.Services.ConfigureOptions<ConfigureMassTransitInstrumentationOptions>();

builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<SampleCreateUserConsumer>();

    configure.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseInstrumentation(options => options.ServiceName = "name");
        cfg.UseHostDefaults(context);

        cfg.ConfigureEndpoints(context);
    });
});

builder.AddOpenTelemetryDefaults();

builder.Services.AddOpenTelemetry()
    .WithMetrics(x => x.AddMeter(InstrumentationOptions.MeterName))
    .WithTracing(x => x.AddSource(DiagnosticHeaders.DefaultListenerName));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async (IPublishEndpoint publish) =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    await publish.Publish(new CreateUser("alef"));

    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapPrometheusScrapingEndpoint();

await app.RunAsync();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
