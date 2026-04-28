using System.Text.Json;
using JobMarketplace.Applications.Application.Interfaces;
using JobMarketplace.Applications.Domain.Aggregates;
using JobMarketplace.Applications.Infrastructure.Persistence.ReadModels;
using JobMarketplace.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;

namespace JobMarketplace.Applications.Infrastructure.Persistence;

public sealed class ApplicationsDbContext(DbContextOptions<ApplicationsDbContext> options)
    : DbContext(options), IApplicationsUnitOfWork
{
    public DbSet<Domain.Aggregates.Application> Applications => Set<Domain.Aggregates.Application>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    internal DbSet<JobReadModel> JobReadModels => Set<JobReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("applications");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationsDbContext).Assembly);

        // Read-only view of jobs table — no migrations generated for this entity
        modelBuilder.Entity<JobReadModel>(b =>
        {
            b.ToView("jobs", schema: "jobs");
            b.HasKey(j => j.Id);
            b.Property(j => j.Id).HasColumnName("id");
            b.Property(j => j.EmployerId).HasColumnName("employer_id");
            b.Property(j => j.Title).HasColumnName("title");
            b.Property(j => j.City).HasColumnName("city");
            b.Property(j => j.Country).HasColumnName("country");
            b.Property(j => j.Status).HasColumnName("status");
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var applications = ChangeTracker.Entries<Domain.Aggregates.Application>()
            .Select(e => e.Entity)
            .ToList();

        foreach (var app in applications)
        {
            var events = app.PopDomainEvents();
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