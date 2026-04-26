using JobMarketplace.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobMarketplace.Applications.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id");
        builder.Property(m => m.Type).HasMaxLength(500).HasColumnName("type").IsRequired();
        builder.Property(m => m.Content).HasColumnType("text").HasColumnName("content").IsRequired();
        builder.Property(m => m.OccurredOnUtc).HasColumnName("occurred_on_utc").IsRequired();
        builder.Property(m => m.ProcessedOnUtc).HasColumnName("processed_on_utc");
    }
}