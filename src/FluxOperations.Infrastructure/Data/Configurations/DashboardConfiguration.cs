using FluxOperations.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxOperations.Infrastructure.Data.Configurations;

public sealed class DashboardConfiguration : IEntityTypeConfiguration<Dashboard>
{
    public void Configure(EntityTypeBuilder<Dashboard> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.OwnerId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Theme)
            .HasMaxLength(50);

        builder.HasMany(d => d.Widgets)
            .WithOne(w => w.Dashboard)
            .HasForeignKey(w => w.DashboardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => d.OwnerId);
    }
}
