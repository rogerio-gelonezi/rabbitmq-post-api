using RabbitMQ.Client;

namespace MessageBus.Engine.Pool;

internal record ChannelPooledObject(IModel Channel, IBasicProperties Properties);