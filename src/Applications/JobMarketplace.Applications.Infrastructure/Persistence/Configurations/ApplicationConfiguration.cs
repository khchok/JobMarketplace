using JobMarketplace.Applications.Domain.Enums;
using JobMarketplace.SharedKernel.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApplicationId = JobMarketplace.SharedKernel.Ids.ApplicationId;

namespace JobMarketplace.Applications.Infrastructure.Persistence.Configurations;

public sealed class ApplicationConfiguration : IEntityTypeConfiguration<Domain.Aggregates.Application>
{
    public void Configure(EntityTypeBuilder<Domain.Aggregates.Application> builder)
    {
        builder.ToTable("applications");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasConversion(id => id.Value, value => ApplicationId.From(value))
            .HasColumnName("id");

        builder.Property(a => a.JobId)
            .HasConversion(id => id.Value, value => JobId.From(value))
            .HasColumnName("job_id")
            .IsRequired();

        builder.Property(a => a.CandidateId)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .HasColumnName("candidate_id")
            .IsRequired();

        builder.HasIndex(a => new { a.JobId, a.CandidateId }).IsUnique();

        builder.OwnsOne(a => a.CoverLetter, b =>
            b.Property(c => c.Value).HasColumnType("text").HasColumnName("cover_letter").IsRequired());

        builder.Property(a => a.Status)
            .HasConversion(s => s.ToString(), s => Enum.Parse<ApplicationStatus>(s))
            .HasMaxLength(20)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(a => a.SubmittedAt).HasColumnName("submitted_at").IsRequired();
        builder.Property(a => a.UpdatedAt).HasColumnName("updated_at").IsRequired();
    }
}