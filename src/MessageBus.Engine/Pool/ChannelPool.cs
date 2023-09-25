using System.Globalization;
using MessageBus.Engine.MessageBus;
using MessageBus.Engine.Pool.Abstractions;
using MessageBus.Engine.Properties;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace MessageBus.Engine.Pool;

internal class ChannelPool : IChannelPool
{
    private readonly object _lock = new();
    private readonly IDictionary<string, DefaultObjectPool<ChannelPooledObject>> _pools;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<ChannelPool> _logger;
    
    public ChannelPool(IMessageBus messageBus, ILogger<ChannelPool> logger)
    {
        _pools = new Dictionary<string, DefaultObjectPool<ChannelPooledObject>>();
        _messageBus = messageBus;
        _logger = logger;
    }

    public IChannelConnectionCache Get(string queue)
    {
        var pool = GetPool(queue);
        var channel = pool.Get();
        return new ChannelConnectionCache(pool, channel);
    }

    private DefaultObjectPool<ChannelPooledObject> GetPool(string queue)
    {
        lock (_lock)
        {
            if (_pools.TryGetValue(queue, out var pool))
            {
                _logger.LogDebug("MessageBus retrieved from cache to queue {queue}.", queue);
                return pool;
            }

            pool = new DefaultObjectPool<ChannelPooledObject>(new ChannelPooledObjectPolicy(_messageBus));
            _pools[queue] = pool;
            _logger.LogDebug("MessageBus inserted into cache for queue {queue}", queue);
            
            return pool;
        }
    }
}