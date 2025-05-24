using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Data.Configuration;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
 public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Таблица и ключ
        builder.ToTable("Orders")
               .HasKey(o => o.Id);

        builder.Property(o => o.Id)
               .ValueGeneratedNever();

        // Статус (enum → string)
        builder.Property(o => o.Status)
               .HasConversion<string>()
               .HasMaxLength(32)
               .IsRequired();

        // Даты
        builder.Property(o => o.CreatedAt)
               .IsRequired()
               .HasDefaultValueSql("NOW()"); // или "GETUTCDATE()" для SQL Server

        builder.Property(o => o.PaidDate)
               .IsRequired(false);

        builder.Property(o => o.ShippedDate)
               .IsRequired(false);

        builder.Property(o => o.CancelledDate)
               .IsRequired(false);

        builder.HasMany(o => o.OrderItemsInternal)
               .WithOne(oi => oi.Order)
               .HasForeignKey(oi => oi.OrderId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
               

        builder.Metadata
               .FindNavigation(nameof(Order.OrderItemsInternal))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(o => o.OrderItemsInternal)
               .AutoInclude(); // Автоподгрузка (если нужно)

        // Value Object: OrderName
        builder.OwnsOne(o => o.OrderName, name =>
        {
            name.Property(n => n.Value)
                .HasColumnName("OrderName")
                .HasMaxLength(100)
                .IsRequired();
        });

        // Value Object: TotalAmount (Money)
        builder.OwnsOne(o => o.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Value Object: ShippingAddress
        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Country)
                  .HasColumnName("ShippingCountry")
                  .HasMaxLength(100)
                  .IsRequired();

            address.Property(a => a.City)
                  .HasColumnName("ShippingCity")
                  .HasMaxLength(100)
                  .IsRequired();

            address.Property(a => a.Street)
                  .HasColumnName("ShippingStreet")
                  .HasMaxLength(200)
                  .IsRequired();

            address.Property(a => a.State)
                  .HasColumnName("ShippingState")
                  .HasMaxLength(50)
                  .IsRequired();

            address.Property(a => a.PostalCode)
                  .HasColumnName("ShippingPostalCode")
                  .HasMaxLength(20)
                  .IsRequired();
        });
    }
}