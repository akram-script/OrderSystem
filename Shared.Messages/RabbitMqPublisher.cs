using RabbitMQ.Client;
using System.Text.Json;

namespace Shared.Messages;

public class RabbitMqPublisher : IAsyncDisposable
{
    private IConnection? _connection;
    private IChannel? _channel;

    public async Task InitializeAsync()
    {
        var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        // Déclarer l'exchange
        await _channel.ExchangeDeclareAsync("orders", "topic", durable: true);

        await _channel.QueueDeclareAsync(
            queue: "notification.queue",
            durable: true,
            exclusive: false,
            autoDelete: false
        );
        await _channel.QueueBindAsync(
            queue: "notification.queue",
            exchange: "orders",
            routingKey: "orders.confirmed"
        );
    }

    public async Task PublishAsync<T>(T message, string routingKey) where T : class
    {
        if (_channel == null)
            throw new InvalidOperationException("RabbitMqPublisher not initialized. Call InitializeAsync() first.");

        var json = JsonSerializer.Serialize(message);
        var body = System.Text.Encoding.UTF8.GetBytes(json);

        await _channel.BasicPublishAsync(
            exchange: "orders",
            routingKey: routingKey,
            body: body);
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
            await _channel.CloseAsync();
        if (_connection != null)
            await _connection.CloseAsync();

        _channel?.Dispose();
        _connection?.Dispose();
    }
}