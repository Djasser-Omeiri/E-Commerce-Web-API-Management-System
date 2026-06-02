using E_Commerce_Web_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace E_Commerce_Web_API.Data.Configurations

{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.ID);
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(p => p.Description)
                .IsRequired();
            builder.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.Property(p => p.CategoryID)
                .IsRequired();
            builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryID);
            builder.HasData(
            new Product
            {
                ID = 1,
                Name = "Laptop",
                Price = 1200,
                Description = "Lenovo ThinkPad",
                CategoryID = 1
            },
            new Product
            {
                ID = 2,
                Name = "Mouse",
                Price = 25,
                Description = "Wireless Mouse",
                CategoryID = 1
            },
            new Product
            {
                ID = 3,
                Name = "Clean Code",
                Price = 40,
                Description = "Programming Book",
                CategoryID = 2
            },
            new Product
            {
                ID = 4,
                Name = "T-Shirt",
                Price = 15,
                Description = "Cotton T-Shirt",
                CategoryID = 3
            }
);
        }
    }
}
