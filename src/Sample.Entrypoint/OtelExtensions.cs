using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Sample.Entrypoint;

public static class OtelExtensions
{
    public static WebApplicationBuilder AddOpenTelemetryDefaults(this WebApplicationBuilder appBuilder)
    {
        void ConfigureResource(ResourceBuilder builder)
        {
            builder
                .AddAttributes(new Dictionary<string, object>
                {
                    ["host.name"] = Environment.MachineName,
                    ["environment.name"] = appBuilder.Environment.EnvironmentName,
                })
                .AddService(serviceName: "sample-app",
                            serviceVersion: "1.0.0",
                            autoGenerateServiceInstanceId: false);
        }

        appBuilder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        appBuilder.Services.AddOpenTelemetry()
            .ConfigureResource(ConfigureResource)
            .WithTracing(ConfigureTracing)
            .WithMetrics(ConfigureMetrics);

        appBuilder.Services.ConfigureOpenTelemetryMeterProvider(builder => builder.AddPrometheusExporter());

        return appBuilder;
    }

    private static void ConfigureTracing(TracerProviderBuilder builder)
    {
        builder.SetSampler(new AlwaysOnSampler());
    }

    private static void ConfigureMetrics(MeterProviderBuilder builder)
    {
    }
}
