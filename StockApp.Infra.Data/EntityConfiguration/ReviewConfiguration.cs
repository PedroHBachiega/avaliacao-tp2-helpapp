using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockApp.Domain.Entities;

namespace StockApp.Infra.Data.EntityConfiguration
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.Id);
            
            builder.Property(r => r.UserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(r => r.Rating)
                .IsRequired();

            builder.Property(r => r.Comment)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(r => r.Date)
                .IsRequired();

            builder.Property(r => r.IsApproved)
                .HasDefaultValue(false);

            builder.HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => r.ProductId);
            builder.HasIndex(r => r.UserId);
            builder.HasIndex(r => new { r.ProductId, r.UserId })
                .IsUnique();
        }
    }
}