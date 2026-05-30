using Shared.Messages;

namespace NotificationService.Workers
{
    public class NotificationWorker(ILogger<NotificationWorker> logger)
  : RabbitMqConsumer<OrderConfirmedEvent>(logger)
    {
        protected override string QueueName => "notification.queue";
        protected override string RoutingKey => "orders.confirmed";
        protected override async Task HandleMessage(OrderConfirmedEvent evt)
        {
            //  simulate send email 
            Logger.LogInformation(
            "[EMAIL] Sending confirmation to {Email} for order {Id}",
            evt.CustomerEmail, evt.OrderId);
            await Task.Delay(50); 
            Logger.LogInformation("Notification sent for order {Id}", evt.OrderId);
        }
    }

}
