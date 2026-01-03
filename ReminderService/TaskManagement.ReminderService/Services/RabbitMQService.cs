using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TaskManagement.ReminderService.Configuration;
using TaskManagement.ReminderService.Models;

namespace TaskManagement.ReminderService.Services;

public interface IRabbitMQService : IDisposable
{
    void PublishReminder(TaskReminderMessage message);
    void StartConsuming(Action<TaskReminderMessage> messageHandler);
    void StopConsuming();
}

public class RabbitMQService : IRabbitMQService
{
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQService> _logger;
    private IConnection? _connection;
    private IModel? _channel;
    private string? _consumerTag;

    public RabbitMQService(IOptions<RabbitMQSettings> settings, ILogger<RabbitMQService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        InitializeConnection();
    }

    private void InitializeConnection()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            _logger.LogInformation("Connecting to RabbitMQ at {Host}:{Port}...", _settings.HostName, _settings.Port);

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _logger.LogInformation("Connected to RabbitMQ. Queue '{Queue}' is ready.", _settings.QueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to RabbitMQ");
            throw;
        }
    }

    public void PublishReminder(TaskReminderMessage message)
    {
        if (_channel == null)
            throw new InvalidOperationException("RabbitMQ channel is not initialized");

        var jsonMessage = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        _channel.BasicPublish(
            exchange: "",
            routingKey: _settings.QueueName,
            basicProperties: properties,
            body: body
        );

        _logger.LogDebug("Published reminder for Task {TaskId}: {Title}", message.TaskId, message.TaskTitle);
    }

    public void StartConsuming(Action<TaskReminderMessage> messageHandler)
    {
        if (_channel == null)
            throw new InvalidOperationException("RabbitMQ channel is not initialized");

        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (sender, eventArgs) =>
        {
            try
            {
                var body = eventArgs.Body.ToArray();
                var jsonMessage = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<TaskReminderMessage>(jsonMessage);

                if (message != null)
                {
                    messageHandler(message);
                    _channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                _channel.BasicNack(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _consumerTag = _channel.BasicConsume(
            queue: _settings.QueueName,
            autoAck: false,
            consumer: consumer
        );

        _logger.LogInformation("Started consuming from queue '{Queue}'", _settings.QueueName);
    }

    public void StopConsuming()
    {
        if (_channel != null && !string.IsNullOrEmpty(_consumerTag))
        {
            _channel.BasicCancel(_consumerTag);
            _logger.LogInformation("Stopped consuming from queue");
        }
    }

    public void Dispose()
    {
        StopConsuming();
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
        _logger.LogInformation("RabbitMQ connection closed");
    }
}
