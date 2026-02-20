using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("documents");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.FileName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(d => d.StoragePath)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.CreatedAtUtc)
            .IsRequired();
    }
}
