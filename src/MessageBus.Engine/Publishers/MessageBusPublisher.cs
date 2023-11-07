using System.Text;
using MessageBus.Engine.MessageBus;
using MessageBus.Engine.Pool.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace MessageBus.Engine.Publishers;

internal class MessageBusPublisher : IMessageBusPublisher
{
    private readonly ILogger<MessageBusPublisher> _logger;
    private readonly IChannelPool _channelPool;
    private readonly IMessageBus _messageBus;

    public MessageBusPublisher(ILogger<MessageBusPublisher> logger, IMessageBus messageBus, IChannelPool channelPool)
    {
        _logger = logger;
        _messageBus = messageBus;
        _channelPool = channelPool;
    }

    public void Publish(string queue, string message)
    {
        try
        {
            using var channelCache = _channelPool.Get(queue);
            _messageBus.CreateQueue(channelCache.Value.Channel, queue);
            
            channelCache.Value.Channel.BasicPublish(exchange: string.Empty,
                                                    routingKey: queue, 
                                                    body: Encoding.UTF8.GetBytes(message),
                                                    basicProperties: channelCache.Value.Properties);
            
            _logger.LogInformation("Publishing message to queue {queue}.", queue);
            
            channelCache.Value.Channel.WaitForConfirmsOrDie();
        }
        catch (Exception e)
        {
            _logger.LogError("Error publishing message to queue {queue}. Details: {exception}", queue, e);
            
            throw;
        }
    }
}