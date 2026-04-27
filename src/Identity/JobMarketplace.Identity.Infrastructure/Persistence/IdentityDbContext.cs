
using System.Text.Json;
using JobMarketplace.Identity.Domain.Aggregates;
using JobMarketplace.Identity.Domain.Interfaces;
using JobMarketplace.SharedKernel.Outbox;
using JobMarketplace.SharedKernel.Primitives;
using Microsoft.EntityFrameworkCore;

namespace JobMarketplace.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options)
    : DbContext(options), IIdentityUnitOfWork
{
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        InterceptDomainEvents();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void InterceptDomainEvents()
    {
        var aggregates = ChangeTracker
            .Entries<AggregateRoot<object>>()
            .Select(e => e.Entity)
            .ToList();

        // AggregateRoot is generic; we need to access via the non-generic interface
        // Use the concrete UserProfile type instead:
        var userProfiles = ChangeTracker
            .Entries<UserProfile>()
            .Select(e => e.Entity)
            .ToList();

        foreach (var profile in userProfiles)
        {
            var events = profile.PopDomainEvents();
            foreach (var domainEvent in events)
            {
                OutboxMessages.Add(new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    Type = domainEvent.GetType().AssemblyQualifiedName!,
                    Content = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                    OccurredOnUtc = DateTime.UtcNow
                });
            }
        }
    }
}