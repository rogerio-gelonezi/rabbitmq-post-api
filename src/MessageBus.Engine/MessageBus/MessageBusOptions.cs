namespace MessageBus.Engine.MessageBus;

internal class MessageBusOptions
{
    public string? HostName { get; set; } = "localhost";
    public ushort PrefetchCount { get; set; } = 1;
    public ushort ConsumerDispatchConcurrency { get; set; }
    public string? Username { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public string? VirtualHost { get; set; } = string.Empty;
    public ushort Port { get; set; }
}