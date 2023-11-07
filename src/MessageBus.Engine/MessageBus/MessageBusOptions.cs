namespace MessageBus.Engine.MessageBus;

internal class MessageBusOptions
{
    public string? HostName { get; set; }
    public ushort PrefetchCount { get; set; }
    public ushort ConsumerDispatchConcurrency { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? VirtualHost { get; set; }
    public ushort Port { get; set; }
    public bool UseSsl { get; set; }
}