using System.ComponentModel.DataAnnotations;

namespace OrderService.Data
{
    public class Order
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        [Required] public string CustomerEmail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";

    }
}
