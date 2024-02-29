using MassTransit;

namespace Sample.Entrypoint.Consumers;

public record CreateUser(string Name);

public class SampleCreateUserConsumer : IConsumer<CreateUser>
{
    public Task Consume(ConsumeContext<CreateUser> context)
    {
        throw new NotImplementedException();
    }
}
