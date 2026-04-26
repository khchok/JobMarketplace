using System.Text.Json;
using JobMarketplace.Jobs.Application.Interfaces;
using JobMarketplace.Jobs.Domain.Aggregates;
using JobMarketplace.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;

namespace JobMarketplace.Jobs.Infrastructure.Persistence;


public sealed class JobsDbContext(DbContextOptions<JobsDbContext> options)
    : DbContext(options), IJobsUnitOfWork
{
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("jobs");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(JobsDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var jobs = ChangeTracker.Entries<Job>()
            .Select(e => e.Entity)
            .ToList();

        foreach (var job in jobs)
        {
            var events = job.PopDomainEvents();
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

        return await base.SaveChangesAsync(cancellationToken);
    }
}