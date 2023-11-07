using MessageBus.Engine.MessageBus;
using MessageBus.Engine.Pool;
using MessageBus.Engine.Pool.Abstractions;
using MessageBus.Engine.Publishers;
using MessageBus.Engine.Validators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MessageBus.Engine.IoC;

public static class ServiceCollectionExtentions
{
    private const string MessageBusHostname = "MESSAGEBUS_HOSTNAME";
    private static readonly (string Key, ushort Value) MessageBusPort = ("MESSAGEBUS_PORT", 5672);
    private const string MessageBusUseSsl = "MESSAGEBUS_USESSL";
    private static readonly (string Key, string Value) MessageBusVirtualHost = ("MESSAGEBUS_VIRTUALHOST", "/");
    private const string MessageBusUsername = "MESSAGEBUS_USERNAME";
    private const string MessageBusPassword = "MESSAGEBUS_PASSWORD";
    private static readonly (string Key, ushort Value) MessageBusPrefetchCount = ("MESSAGEBUS_PREFETCHCOUNT", 1);
    
    public static void AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        var prefetchCount = ResolvePrefetchCount(configuration);
        var port = ResolvePort(configuration);
        var virtualHost = ResolveVirtualHost(configuration);
        
        services.AddMessageBus(p =>
        {
            p.HostName = configuration[MessageBusHostname];
            p.Username = configuration[MessageBusUsername];
            p.Password = configuration[MessageBusPassword];
            p.VirtualHost = virtualHost;
            p.PrefetchCount = prefetchCount;
            p.ConsumerDispatchConcurrency = prefetchCount;
            p.Port = port;
            p.UseSsl = Convert.ToBoolean(configuration[MessageBusUseSsl]);
        });
    }

    private static void AddMessageBus(this IServiceCollection services, Action<MessageBusOptions> options)
    {
        services.Configure(options);
        services.TryAddSingleton<IMessageBusPublisher, MessageBusPublisher>();
        services.TryAddSingleton<IMessageBus, MessageBus.MessageBus>();
        services.TryAddSingleton<IChannelPool, ChannelPool>();
        services.TryAddSingleton(CreateRabbitMqConnectionFactory);
        services.TryAddSingleton(sp => sp.GetRequiredService<IConnectionFactory>().CreateConnection());
    }
    
    private static IConnectionFactory CreateRabbitMqConnectionFactory(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<IOptions<MessageBusOptions>>().Value;
        MessageBusOptionsValidator.Validate(options);
        var connectionFactory = new ConnectionFactory
        {
            DispatchConsumersAsync = true,
            ConsumerDispatchConcurrency = options.ConsumerDispatchConcurrency,
            HostName = options.HostName,
            UserName = options.Username,
            Password = options.Password,
            VirtualHost = options.VirtualHost,
            Ssl =
            {
                ServerName = options.HostName,
                Enabled = options.UseSsl
            }
        };
        if (options.Port != default)
            connectionFactory.Port = options.Port;

        return connectionFactory;
    }

    private static ushort ResolvePrefetchCount(IConfiguration configuration)
    {
        return ushort.TryParse(configuration[MessageBusPrefetchCount.Key], out var prefetchCount)
            ? prefetchCount
            : MessageBusPrefetchCount.Value;
    }
    
    private static ushort ResolvePort(IConfiguration configuration)
    {
        return ushort.TryParse(configuration[MessageBusPort.Key], out var prefetchCount)
            ? prefetchCount
            : MessageBusPort.Value;
    }
    
    private static string? ResolveVirtualHost(IConfiguration configuration)
    {
        return string.IsNullOrWhiteSpace(configuration[MessageBusVirtualHost.Key])
            ? MessageBusVirtualHost.Value
            : configuration[MessageBusVirtualHost.Key];
    }

}