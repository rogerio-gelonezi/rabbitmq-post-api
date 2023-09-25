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
    private const string MessageBusUsername = "MESSAGEBUS_USERNAME";
    private const string MessageBusPassword = "MESSAGEBUS_PASSWORD";
    private const string MessageBusVirtualHost = "MESSAGEBUS_VIRTUALHOST";
    private static readonly (string Key, ushort Value) MessageBusPrefetchCount = ("MESSAGEBUS_PREFETCHCOUNT", 1);
    
    public static IServiceCollection AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        var prefetchCount = ResolvePrefetchCount(configuration);
        services.AddMessageBus(p =>
        {
            p.HostName = configuration[MessageBusHostname];
            p.Username = configuration[MessageBusUsername];
            p.Password = configuration[MessageBusPassword];
            p.VirtualHost = configuration[MessageBusVirtualHost];
            p.PrefetchCount = prefetchCount;
            p.ConsumerDispatchConcurrency = prefetchCount;
        });
        return services;
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
            ConsumerDispatchConcurrency = options.PrefetchCount,
            HostName = options.HostName,
            UserName = options.Username,
            Password = options.Password
        };
        if (options.Port != default)
            connectionFactory.Port = options.Port;
        if (!string.IsNullOrWhiteSpace(options.HostName))
            connectionFactory.HostName = options.HostName;

        return connectionFactory;
    }

    private static ushort ResolvePrefetchCount(IConfiguration configuration)
    {
        return ushort.TryParse(configuration[MessageBusPrefetchCount.Key], out var prefetchCount)
            ? prefetchCount
            : MessageBusPrefetchCount.Value;
    }
}