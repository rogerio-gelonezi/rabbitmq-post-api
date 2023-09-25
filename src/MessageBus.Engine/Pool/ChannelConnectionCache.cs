using MessageBus.Engine.Pool.Abstractions;
using Microsoft.Extensions.ObjectPool;

namespace MessageBus.Engine.Pool;

internal class ChannelConnectionCache : IChannelConnectionCache
{
    private readonly DefaultObjectPool<ChannelPooledObject> _pool;

    public ChannelConnectionCache(DefaultObjectPool<ChannelPooledObject> pool, ChannelPooledObject channelPooledObject)
    {
        _pool = pool;
        Value = channelPooledObject;
    }

    public ChannelPooledObject Value { get; }
    
    public void Dispose()
    {
        _pool.Return(Value);
    }
}