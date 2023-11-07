using MessageBus.Engine.Properties;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace MessageBus.Engine.MessageBus;

internal class MessageBus : IMessageBus
{
    private const string ArgumentMaxPriority = "x-max-priority";

    private static readonly IDictionary<string, object> Arguments = new Dictionary<string, object> { [ArgumentMaxPriority] = 5 };

    private IConnection? _connection;
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
                _logger.LogInformation("Connection successfully made to queue {queue}.", queue);
            }
        }
        catch (Exception e)
        {
            lock (_lock)
            {
                _logger.LogWarning("Error connecting to queue {queue}. Details: {exception}.", queue, e);
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
            if (_connection == null)
                throw new ApplicationException(Resources.Error_MessageBus_Connection);
            
            var model = _connection.CreateModel();
            model.ConfirmSelect();
            model.BasicQos(prefetchSize: 0,
                           prefetchCount: _options.PrefetchCount,
                           global: false);
            
            lock (_lock)
            {
                _logger.LogInformation("MessageBus Model successfully created.");
            }
            
            return model;
        }
        catch (Exception e)
        {
            lock (_lock)
            {
                _logger.LogWarning("Error creating the MessageBus Model. Details: {exception}.", e);
            }

            throw;
        }
        
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

    private void Connect(ISyncPolicy policy)
    {
        policy.Execute(() =>
        {
            _connection = _connectionFactory.CreateConnection();
            _connection.ConnectionShutdown += OnDisconnect;
            _logger.LogInformation("Connection to MessageBus completed successfully.");
        });
    }

    private void OnDisconnect(object? sender, ShutdownEventArgs e)
    {
        _logger.LogError("Connection to MessageBus was lost.");
        var policy = Policy.Handle<BrokerUnreachableException>().RetryForever();
        policy.Execute(TryConnect);
    }

    private void RetryConnect(int retryCount, Exception exception)
    {
        _logger.LogWarning("MessageBus connection attempt number {retryCount}. Error message:{exception.Message}.", retryCount, exception.Message);        
        if (retryCount < 3) return;
        _logger.LogError("Error connecting to MessageBus, retry attempts {retryCount}. Error message: {exception.Message} ", retryCount, exception.Message);
    }
}