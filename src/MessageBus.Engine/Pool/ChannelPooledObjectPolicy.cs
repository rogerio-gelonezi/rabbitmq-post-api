using MessageBus.Engine.MessageBus;
using Microsoft.Extensions.ObjectPool;

namespace MessageBus.Engine.Pool;

internal class ChannelPooledObjectPolicy : IPooledObjectPolicy<ChannelPooledObject>
{
    private readonly IMessageBus _messageBus;

    public ChannelPooledObjectPolicy(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public ChannelPooledObject Create()
    {
        var channel = _messageBus.CreateModel();
        var properties = channel.CreateBasicProperties();

        properties.Persistent = true;

        return new ChannelPooledObject(channel, properties);
    }

    public bool Return(ChannelPooledObject channelPooledObject)
    {
        if (channelPooledObject.Channel.IsOpen)
            return true;

        channelPooledObject.Channel.Dispose();
        return false;
    }
}