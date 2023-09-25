namespace MessageBus.Engine.Pool.Abstractions;

internal interface IChannelConnectionCache : IDisposable
{
    public ChannelPooledObject Value { get; }
}