namespace MessageBus.Engine.Publishers;

public interface IMessageBusPublisher
{
    void Publish(string queueName, string message);
}