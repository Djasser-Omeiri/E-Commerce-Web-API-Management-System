using E_Commerce_Web_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace E_Commerce_Web_API.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.ID);
            builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
            builder.ToTable("Categories");
            builder.HasData(
                  new Category { ID = 1, Name = "Electronics" },
                  new Category { ID = 2, Name = "Books" },
                  new Category { ID = 3, Name = "Clothing" });
        }

    }
}
