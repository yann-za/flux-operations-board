using FluxOperations.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxOperations.Infrastructure.Data.Configurations;

public sealed class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(a => a.Severity)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.ResolvedBy)
            .HasMaxLength(200);

        builder.Property(a => a.ResolutionNote)
            .HasMaxLength(1000);

        builder.HasIndex(a => a.IsResolved);
        builder.HasIndex(a => a.Severity);
        builder.HasIndex(a => new { a.FluxId, a.IsResolved });
    }
}
