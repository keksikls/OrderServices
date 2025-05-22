namespace OrderService.Infrastructure.Data.DbContext;

public class OrderDbContext
{
    public class OrderDbContext : IdentityDbContext<UserEntity, IdentityRoleEntity, long>
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }
    
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<MerchantEntity> Merchants { get; set; }
        public DbSet<CustomerEntity> Customers { get; set; } 
        public DbSet<CartEntity> Carts { get; set; }
        public DbSet<CartItemEntity> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        
            //auto mapping all configuration
            builder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
        }
    }
}