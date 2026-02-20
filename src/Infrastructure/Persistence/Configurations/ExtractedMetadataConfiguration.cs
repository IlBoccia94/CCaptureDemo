using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class ExtractedMetadataConfiguration : IEntityTypeConfiguration<ExtractedMetadata>
{
    public void Configure(EntityTypeBuilder<ExtractedMetadata> builder)
    {
        builder.ToTable("extracted_metadata");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FieldName).HasMaxLength(128).IsRequired();
        builder.Property(x => x.FieldValue).HasMaxLength(4000);
        builder.HasIndex(x => new { x.DocumentId, x.FieldName });
    }
}
