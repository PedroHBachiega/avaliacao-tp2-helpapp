using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockApp.Domain.Entities;

namespace StockApp.Infra.Data.EntityConfiguration
{
    public class ContractConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.ContractNumber)
                .HasMaxLength(50)
                .IsRequired();
                
            builder.Property(c => c.Description)
                .HasMaxLength(500)
                .IsRequired();
                
            builder.Property(c => c.StartDate)
                .IsRequired();
                
            builder.Property(c => c.EndDate)
                .IsRequired();
                
            builder.Property(c => c.Value)
                .HasPrecision(18, 2)
                .IsRequired();
                
            builder.Property(c => c.Terms)
                .HasMaxLength(2000)
                .IsRequired();
                
            builder.Property(c => c.IsActive)
                .IsRequired();
                
            builder.Property(c => c.ContractType)
                .IsRequired();
                
            builder.Property(c => c.CreatedAt)
                .IsRequired();
                
            // Relacionamento com Supplier (opcional)
            builder.HasOne(c => c.Supplier)
                .WithMany()
                .HasForeignKey(c => c.SupplierId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}