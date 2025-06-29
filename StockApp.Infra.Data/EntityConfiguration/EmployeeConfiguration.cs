using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockApp.Domain.Entities;

namespace StockApp.Infra.Data.EntityConfiguration
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Position)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Department)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.HireDate)
                .IsRequired();

            // Relacionamento com EmployeeEvaluation
            builder.HasMany<EmployeeEvaluation>()
                .WithOne(e => e.Employee)
                .HasForeignKey(e => e.EmployeeId);
        }
    }
}