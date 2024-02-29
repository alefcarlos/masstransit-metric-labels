using MassTransit;
using Microsoft.Extensions.Options;

public static class RabbitMqBusFactoryConfiguratorExtensions
{
    public static void UseHostDefaults(this IRabbitMqBusFactoryConfigurator configurator, IBusRegistrationContext context)
    {
        var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

        configurator.Host(rabbitMqOptions.ConnectionString, "sample-app");

        configurator.ConfigurePublish(pc => pc.UseExecute((context) =>
        {
            context.Headers.Set("x-hbsa-appname", "sample-app");
        }));

        configurator.ConfigureSend(pc => pc.UseExecute((context) =>
        {
            context.Headers.Set("x-hbsa-appname", "sample-app");
        }));
    }
}
