using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class ProcessingLogConfiguration : IEntityTypeConfiguration<ProcessingLog>
{
    public void Configure(EntityTypeBuilder<ProcessingLog> builder)
    {
        builder.ToTable("processing_logs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Step).HasMaxLength(128).IsRequired();
        builder.Property(x => x.Message).HasMaxLength(4000).IsRequired();
        builder.HasIndex(x => new { x.DocumentId, x.CreatedAtUtc });
    }
}
