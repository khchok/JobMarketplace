using System.Text.Json;
using JobMarketplace.Applications.Infrastructure.Persistence;
using JobMarketplace.Identity.Infrastructure.Persistence;
using JobMarketplace.Jobs.Infrastructure.Persistence;
using JobMarketplace.SharedKernel.Outbox;
using JobMarketplace.SharedKernel.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobMarketplace.Api.BackgroundServices;

public sealed class OutboxProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxProcessor> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessAllContextsAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ProcessAllContextsAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

        await ProcessContextAsync<IdentityDbContext>(scope, publisher, ct);
        await ProcessContextAsync<JobsDbContext>(scope, publisher, ct);
        await ProcessContextAsync<ApplicationsDbContext>(scope, publisher, ct);
    }

    private async Task ProcessContextAsync<TContext>(
        IServiceScope scope,
        IPublisher publisher,
        CancellationToken ct) where TContext : DbContext
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

        var messages = await dbContext.Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(ct);

        foreach (var message in messages)
        {
            try
            {
                var type = Type.GetType(message.Type);
                if (type is null)
                {
                    logger.LogWarning("Could not resolve type {Type}", message.Type);
                    continue;
                }

                var domainEvent = JsonSerializer.Deserialize(message.Content, type) as IDomainEvent;
                if (domainEvent is null)
                {
                    logger.LogWarning("Could not deserialize outbox message {Id}", message.Id);
                    continue;
                }

                await publisher.Publish(domainEvent, ct);
                message.ProcessedOnUtc = DateTime.UtcNow;
                await dbContext.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing outbox message {Id}", message.Id);
            }
        }
    }
}