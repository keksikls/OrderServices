using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Data.Configuration;
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");
        
            builder.HasKey(oi => oi.Id);

            // Связь с Order
            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItemsInternal)
                .HasForeignKey(oi => oi.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // ProductId (Value Object)
            builder.OwnsOne(oi => oi.ProductId, pid =>
            {
                pid.Property(p => p.Value)
                    .HasColumnName("ProductId")
                    .IsRequired();
            }).Navigation(oi => oi.ProductId).IsRequired();

            builder.OwnsOne(oi => oi.Quantity, qty =>
            {
                qty.Property(q => q.Value)
                    .HasColumnName("Quantity")
                    .IsRequired();
            }).Navigation(oi => oi.Quantity).IsRequired();

            builder.OwnsOne(oi => oi.UnitPrice, price =>
            {
                price.Property(p => p.Amount)
                    .HasColumnName("UnitPriceAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                price.Property(p => p.Currency)
                    .HasColumnName("UnitPriceCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            }).Navigation(oi => oi.UnitPrice).IsRequired(); 

            builder.Ignore(oi => oi.TotalPrice);

            // Индексы
            builder.HasIndex(oi => oi.OrderId); // Для OrderId (Guid)
        }
    }
