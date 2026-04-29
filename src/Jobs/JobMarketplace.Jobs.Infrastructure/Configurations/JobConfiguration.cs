using JobMarketplace.Jobs.Domain.Aggregates;
using JobMarketplace.Jobs.Domain.Enums;
using JobMarketplace.SharedKernel.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobMarketplace.Jobs.Infrastructure.Configurations;

public sealed class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("jobs");

        builder.HasKey(j => j.Id);
        builder.Property(j => j.Id)
            .HasConversion(id => id.Value, value => JobId.From(value))
            .HasColumnName("id");

        builder.Property(j => j.EmployerId)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .HasColumnName("employer_id")
            .IsRequired();

        builder.OwnsOne(j => j.Title, b =>
            b.Property(t => t.Value).HasMaxLength(100).HasColumnName("title").IsRequired());

        builder.OwnsOne(j => j.Description, b =>
            b.Property(d => d.Value).HasColumnType("text").HasColumnName("description").IsRequired());

        builder.OwnsOne(j => j.Location, b =>
        {
            b.Property(l => l.City).HasMaxLength(100).HasColumnName("city").IsRequired();
            b.Property(l => l.Country).HasMaxLength(100).HasColumnName("country").IsRequired();
        });

        builder.OwnsOne(j => j.SalaryRange, b =>
        {
            b.Property(s => s.Min).HasPrecision(18, 2).HasColumnName("salary_min").IsRequired();
            b.Property(s => s.Max).HasPrecision(18, 2).HasColumnName("salary_max").IsRequired();
            b.Property(s => s.Currency).HasMaxLength(10).HasColumnName("salary_currency").IsRequired();
        });

        builder.Property(j => j.Status)
            .HasConversion(s => s.ToString(), s => Enum.Parse<JobStatus>(s))
            .HasMaxLength(20)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(j => j.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(j => j.PublishedAt).HasColumnName("published_at");
    }
}