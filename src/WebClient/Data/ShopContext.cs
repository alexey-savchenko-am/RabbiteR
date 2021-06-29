namespace WebClient.Data
{
    using Microsoft.EntityFrameworkCore;
    using WebClient.Data.Config;

    public class ShopContext
        : DbContext
    {
        public ShopContext(DbContextOptions<ShopContext> options)
            : base(options)
        {}

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new OrdersConfiguration());
        }
    }
}
