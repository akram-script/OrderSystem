using Microsoft.EntityFrameworkCore;

namespace OrderService.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
    {
        public DbSet<Order> Orders => Set<Order>();
    }

}
