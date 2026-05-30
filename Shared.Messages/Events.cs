using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Messages
{
    public record OrderCreatedEvent(Guid OrderId,
                                    string CustomerEmail,
                                    List<OrderItem> Items,
                                    DateTime CreatedAt
    );
    public record OrderConfirmedEvent(
     Guid OrderId,
     string CustomerEmail,
     DateTime ConfirmedAt
    );
    public record OrderItem(string ProductId, int Quantity);
}
