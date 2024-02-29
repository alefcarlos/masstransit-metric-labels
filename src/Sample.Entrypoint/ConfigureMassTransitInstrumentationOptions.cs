using MassTransit.Monitoring;
using Microsoft.Extensions.Options;

public class ConfigureMassTransitInstrumentationOptions : IConfigureOptions<InstrumentationOptions>
{
    public void Configure(InstrumentationOptions options)
    {
        options.ServiceName = "sample-app";
    }
}
