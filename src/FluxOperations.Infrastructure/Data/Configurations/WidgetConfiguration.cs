using FluxOperations.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FluxOperations.Infrastructure.Data.Configurations;

public sealed class WidgetConfiguration : IEntityTypeConfiguration<Widget>
{
    public void Configure(EntityTypeBuilder<Widget> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(w => w.ConfigJson)
            .HasColumnType("nvarchar(max)");
    }
}
