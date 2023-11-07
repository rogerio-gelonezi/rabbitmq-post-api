using RabbitMQ.Client;

namespace MessageBus.Engine.MessageBus;

internal interface IMessageBus
{
    void CreateQueue(IModel model, string queue);
    IModel CreateModel();
}