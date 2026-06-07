using E_Commerce_Web_API.Enums;
using E_Commerce_Web_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Web_API.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.ID);
            builder.Property(o => o.OrderTime)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");
            builder.Property(o => o.TotalPrice)
                .IsRequired().HasComputedColumnSql("[dbo].[CalculateOrderTotal]([ID])");
            builder.Property(o => o.ShippingAddress)
                .IsRequired();
            builder.Property(o => o.Status)
                .IsRequired().HasDefaultValue(eOrderStatus.Pending);
        }
    }
}
