namespace MessageBus.Engine.Publishers;

public interface IMessageBusPublisher
{
    void Publish(string queue, string message);
}