using E_Commerce_Web_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce_Web_API.Data.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");
            builder.HasKey(r => r.ID);
            builder.Property(r => r.Comment).IsRequired().HasMaxLength(1000);
            builder.Property(r => r.Rating).IsRequired().HasComment("Rating must be between 1 and 5");
            builder.ToTable(r=>r.HasCheckConstraint("CK_Review_Rating", "Rating >= 1 AND Rating <= 5"));
            builder.Property(r => r.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(r => r.ProductID).IsRequired();

            builder.HasOne(r => r.Product)
                   .WithMany(p => p.Reviews)
                   .HasForeignKey(r => r.ProductID)
                   .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
