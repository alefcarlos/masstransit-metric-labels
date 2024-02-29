using Microsoft.Extensions.Options;

public class ConfigureRabbitMqOptions : IConfigureOptions<RabbitMqOptions>
{
    private readonly IConfiguration _configuration;

    public ConfigureRabbitMqOptions(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(RabbitMqOptions options)
    {
        options.ConnectionString = new Uri(_configuration.GetConnectionString("RabbitMQ")!);
    }
}
