namespace MessageBus.Engine.Pool.Abstractions;

internal interface IChannelPool
{
    IChannelConnectionCache Get(string queue);
}
