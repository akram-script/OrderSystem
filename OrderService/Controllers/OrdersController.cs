using Microsoft.AspNetCore.Mvc;
using OrderService.Data;
using Shared.Messages;
namespace OrderService.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController(
 AppDbContext db,
 RabbitMqPublisher publisher,
 ILogger<OrdersController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        // Save to database
        var order = new Order { CustomerEmail = request.CustomerEmail };
        db.Orders.Add(order);
        await db.SaveChangesAsync();
        // Publish event to RabbitMQ
        var evt = new OrderCreatedEvent(
        OrderId: order.Id,
        CustomerEmail: order.CustomerEmail,
        Items: request.Items,
        CreatedAt: order.CreatedAt
        );
        await publisher.PublishAsync(evt, routingKey: "orders.created");
        logger.LogInformation("Order {Id} created and published", order.Id);
        return Ok(new { order.Id, order.Status });
    }
}
public record CreateOrderRequest(
 string CustomerEmail,
 List<OrderItem> Items
);
