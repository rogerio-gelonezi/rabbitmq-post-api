using System.Globalization;
using System.Text;
using MessageBus.Engine.MessageBus;
using MessageBus.Engine.Pool.Abstractions;
using MessageBus.Engine.Properties;
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

    public void Publish(string queueName, string message)
    {
        try
        {
            using var channelCache = _channelPool.Get(queueName);
            _messageBus.CreateQueue(channelCache.Value.Channel, queueName);
            
            channelCache.Value.Channel.BasicPublish(exchange: string.Empty,
                                                    routingKey: queueName, 
                                                    body: Encoding.UTF8.GetBytes(message),
                                                    basicProperties: channelCache.Value.Properties);
            
            _logger.LogInformation(string.Format(CultureInfo.InvariantCulture, Resources.Publishing_To_Queue, queueName));
            
            channelCache.Value.Channel.WaitForConfirmsOrDie();
        }
        catch (Exception e)
        {
            _logger.LogError(string.Format(CultureInfo.InvariantCulture, Resources.Publishing_To_Queue_With_Error, queueName, e));
            
            throw;
        }
    }
}