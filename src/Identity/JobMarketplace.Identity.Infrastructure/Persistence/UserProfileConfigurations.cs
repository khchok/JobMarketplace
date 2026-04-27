using JobMarketplace.Identity.Domain.Aggregates;
using JobMarketplace.Identity.Domain.Enums;
using JobMarketplace.SharedKernel.Ids;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobMarketplace.Identity.Infrastructure.Persistence;

public sealed class UserProfileConfigurations : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .HasColumnName("id");

        builder.OwnsOne(p => p.Email, emailBuilder =>
        {
            emailBuilder.Property(e => e.Value)
                .HasMaxLength(320)
                .HasColumnName("email")
                .IsRequired();
            emailBuilder.HasIndex(e => e.Value).IsUnique();
        });

        builder.Property(p => p.FullName)
            .HasMaxLength(200)
            .HasColumnName("full_name")
            .IsRequired();

        builder.Property(p => p.Role)
            .HasConversion(r => r.ToString(), s => Enum.Parse<UserRole>(s))
            .HasMaxLength(20)
            .HasColumnName("role")
            .IsRequired();

        builder.Property(p => p.PasswordHash)
            .HasMaxLength(100)
            .HasColumnName("password_hash")
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
    }
}
