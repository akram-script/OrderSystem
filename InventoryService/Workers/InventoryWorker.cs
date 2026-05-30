using Shared.Messages;

namespace InventoryService.Workers
{
    public class InventoryWorker(ILogger<InventoryWorker> logger,
                                 RabbitMqPublisher publisher) : RabbitMqConsumer<OrderCreatedEvent>(logger)
    {
        protected override string QueueName => "inventory.queue";
        protected override string RoutingKey => "orders.created";
        protected override async Task HandleMessage(OrderCreatedEvent evt)
        {
            logger.LogInformation(
            "Reserving stock for order {Id} — {Count} item(s)",
            evt.OrderId, evt.Items.Count);
            // Simulate stock check 
            await Task.Delay(200);
            bool inStock = true; // 
            if (inStock)
            {
                var confirmed = new OrderConfirmedEvent(
                OrderId: evt.OrderId,
                CustomerEmail: evt.CustomerEmail,
                ConfirmedAt: DateTime.UtcNow
                );
                await publisher.PublishAsync(confirmed, "orders.confirmed");
                logger.LogInformation("Order {Id} confirmed", evt.OrderId);
            }
            else
            {
                logger.LogWarning("Out of stock for order {Id}", evt.OrderId);
                // In production: publish OrderFailedEvent
            }
        }
    }

}