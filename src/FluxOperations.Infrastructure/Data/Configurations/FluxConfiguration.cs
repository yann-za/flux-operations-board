using FluxOperations.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxOperations.Infrastructure.Data.Configurations;

public sealed class FluxConfiguration : IEntityTypeConfiguration<Flux>
{
    public void Configure(EntityTypeBuilder<Flux> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.Description)
            .HasMaxLength(1000);

        builder.Property(f => f.SourceSystem)
            .HasMaxLength(200);

        builder.Property(f => f.TargetSystem)
            .HasMaxLength(200);

        builder.Property(f => f.ScheduleCron)
            .HasMaxLength(100);

        builder.Property(f => f.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(f => f.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasMany(f => f.Alerts)
            .WithOne(a => a.Flux)
            .HasForeignKey(a => a.FluxId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.Metrics)
            .WithOne(m => m.Flux)
            .HasForeignKey(m => m.FluxId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(f => f.Status);
        builder.HasIndex(f => f.Type);
        builder.HasIndex(f => f.IsArchived);
        builder.HasIndex(f => f.Name);
    }
}
