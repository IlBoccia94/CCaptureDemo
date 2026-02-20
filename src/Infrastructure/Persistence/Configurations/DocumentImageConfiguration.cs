using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class DocumentImageConfiguration : IEntityTypeConfiguration<DocumentImage>
{
    public void Configure(EntityTypeBuilder<DocumentImage> builder)
    {
        builder.ToTable("document_images");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SourcePath).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.CroppedPath).HasMaxLength(1000);
        builder.Property(x => x.OverlayPath).HasMaxLength(1000);
    }
}
