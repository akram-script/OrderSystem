using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace Shared.Messages;

public abstract class RabbitMqConsumer<T> : BackgroundService where T : class
{
    protected ILogger Logger { get; }
    private IConnection? _connection;
    private IChannel? _channel;

    protected RabbitMqConsumer(ILogger logger)
    {
        Logger = logger;
    }

    protected abstract string QueueName { get; }
    protected abstract string RoutingKey { get; }
    protected abstract Task HandleMessage(T message);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq" };
            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.ExchangeDeclareAsync("orders", "topic", durable: true, cancellationToken: stoppingToken);
            await _channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
            await _channel.QueueBindAsync(QueueName, "orders", RoutingKey, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = System.Text.Encoding.UTF8.GetString(body);
                    var message = JsonSerializer.Deserialize<T>(json);

                    if (message != null)
                    {
                        await HandleMessage(message);
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error processing message");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
                }
            };

            await _channel.BasicConsumeAsync(QueueName, false, consumer, cancellationToken: stoppingToken);

            // Keep the consumer running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            Logger.LogInformation("RabbitMQ consumer cancelled");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Fatal error in RabbitMQ consumer");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        _channel?.Dispose();
        _connection?.Dispose();
    }
}