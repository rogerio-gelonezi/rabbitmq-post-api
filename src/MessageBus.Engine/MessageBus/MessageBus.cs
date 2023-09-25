using System.Globalization;
using MessageBus.Engine.Properties;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace MessageBus.Engine.MessageBus;

internal class MessageBus : IMessageBus
{
    private const string ArgumentMaxPriority = "x-max-priority";

    private static readonly IDictionary<string, object> Arguments = new Dictionary<string, object> { [ArgumentMaxPriority] = 5 };

    private IConnection _connection;
    private readonly object _lock = new();
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<MessageBus> _logger;
    private readonly MessageBusOptions _options;

    public MessageBus(IOptions<MessageBusOptions> options, ILogger<MessageBus> logger, IConnectionFactory connectionFactory)
    {
        _options = options.Value;
        _logger = logger;
        _connectionFactory = connectionFactory;
    }

    private bool IsConnected => _connection?.IsOpen ?? false;
    
    public IModel ConnectQueue(string queue)
    {
        TryConnect();
        var model = CreateModel();
        CreateQueue(model, queue);
        return model;
    }

    public void CreateQueue(IModel model, string queue)
    {
        try
        {
            model.QueueDeclare(queue: queue,
                               durable: true,
                               exclusive: false,
                               autoDelete: false,
                               arguments: Arguments);

            lock (_lock)
            {
                _logger.LogInformation(string.Format(CultureInfo.InvariantCulture, Resources.Queue_Connection_With_Success, queue));
            }
        }
        catch (Exception e)
        {
            lock (_lock)
            {
                _logger.LogWarning(string.Format(CultureInfo.InvariantCulture, Resources.Queue_Connection_With_Error, queue, e));
            }

            throw;
        }
    }

    public IModel CreateModel()
    {
        try
        {
            if (!IsConnected)
                TryConnect();
            
            var model = _connection.CreateModel();
            model.ConfirmSelect();
            model.BasicQos(prefetchSize: 0,
                           prefetchCount: _options.PrefetchCount,
                           global: false);
            
            
            lock (_lock)
            {
                _logger.LogInformation(Resources.Model_Created_With_Success);
            }
            
            return model;
        }
        catch (Exception e)
        {
            lock (_lock)
            {
                _logger.LogWarning(string.Format(CultureInfo.InvariantCulture, Resources.Model_Created_With_Error, e));
            }

            throw;
        }
        
    }

    public void ConnectConsumer(IModel channel, string queue, IBasicConsumer consumer)
    {
        channel.BasicConsume(queue: queue,
                             autoAck: false,
                             consumer: consumer);
    }
    
    private void TryConnect()
    {
        lock (_lock)
        {
            if (IsConnected) return;

            var policy = Policy.Handle<BrokerUnreachableException>()
                               .WaitAndRetry(retryCount: 3,
                                             sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                             onRetry: (exception, _, retryCount, _) => { RetryConnect(retryCount, exception); });

            Connect(policy);
        }
    }

    private void Connect(RetryPolicy policy)
    {
        policy.Execute(() =>
        {
            _connection = _connectionFactory.CreateConnection();
            _connection.ConnectionShutdown += OnDisconnect;
            _logger.LogInformation(Resources.MessageBus_Connection_With_Success);
        });
    }

    private void OnDisconnect(object? sender, ShutdownEventArgs e)
    {
        _logger.LogError(Resources.MessageBus_Connection_Lost);
        var policy = Policy.Handle<BrokerUnreachableException>().RetryForever();
        policy.Execute(TryConnect);
    }

    private void RetryConnect(int retryCount, Exception exception)
    {
        _logger.LogWarning(string.Format(CultureInfo.InvariantCulture, Resources.MessageBus_Connection_Attempt, retryCount));
        if (retryCount < 3) return;
        _logger.LogError(string.Format(CultureInfo.InvariantCulture, Resources.MessageBus_Connection_Error, retryCount));
    }
}