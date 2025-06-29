using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockApp.Domain.Entities;

namespace StockApp.Infra.Data.EntityConfiguration
{
    public class EmployeeEvaluationConfiguration : IEntityTypeConfiguration<EmployeeEvaluation>
    {
        public void Configure(EntityTypeBuilder<EmployeeEvaluation> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.EvaluationScore)
                .IsRequired();

            builder.Property(e => e.Feedback)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.Goals)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.EvaluationDate)
                .IsRequired();

            builder.Property(e => e.EvaluatedBy)
                .IsRequired()
                .HasMaxLength(100);

            // Relacionamento com Employee
            builder.HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}