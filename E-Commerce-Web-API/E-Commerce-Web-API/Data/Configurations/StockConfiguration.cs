using E_Commerce_Web_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Web_API.Data.Configurations
{
    public class StockConfiguration : IEntityTypeConfiguration<Stock>
    {
        public void Configure(EntityTypeBuilder<Stock> builder)
        {
            builder.ToTable("Stocks");
            builder.HasKey(s => s.ID);
            builder.Property(s => s.Quantity).IsRequired();
            builder.Property(s => s.ProductID).IsRequired();
            builder.HasOne(s => s.Product)
                   .WithOne(p => p.Stock)
                   .HasForeignKey<Stock>(s => s.ProductID)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
