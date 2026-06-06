using E_Commerce_Web_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Web_API.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");
            builder.HasKey(oi => oi.ID);
            builder.Property(oi => oi.Quantity).IsRequired();
            builder.Property(oi => oi.PriceAtPurchase).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(oi => oi.OrderID).IsRequired();
            builder.Property(oi => oi.ProductID).IsRequired();
            builder.HasOne(oi => oi.Order)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(oi => oi.OrderID)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(oi => oi.Product)
                   .WithMany()
                   .HasForeignKey(oi => oi.ProductID)
                   .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
