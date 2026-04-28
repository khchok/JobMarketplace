using System.Text.Json;
using JobMarketplace.Applications.Infrastructure.Persistence;
using JobMarketplace.Identity.Infrastructure.Persistence;
using JobMarketplace.Jobs.Infrastructure.Persistence;
using JobMarketplace.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;

namespace JobMarketplace.Api.Endpoints;

public static class AdminEndpoints
{
    public static RouteGroupBuilder MapAdminEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/events/identity", async (
            IdentityDbContext db,
            CancellationToken ct,
            int page = 1, int pageSize = 20) =>
            Results.Ok(await QueryEventsAsync(db, "identity", page, pageSize, ct))
        ).RequireAuthorization("Admin");

        group.MapGet("/events/jobs", async (
            JobsDbContext db,
            CancellationToken ct,
            int page = 1, int pageSize = 20) =>
            Results.Ok(await QueryEventsAsync(db, "jobs", page, pageSize, ct))
        ).RequireAuthorization("Admin");

        group.MapGet("/events/applications", async (
            ApplicationsDbContext db,
            CancellationToken ct,
            int page = 1, int pageSize = 20) =>
            Results.Ok(await QueryEventsAsync(db, "applications", page, pageSize, ct))
        ).RequireAuthorization("Admin");

        return group;
    }

    private static async Task<EventLogResponse> QueryEventsAsync(
        DbContext db,
        string module,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        page = page > 0 ? page : 1;
        pageSize = pageSize is > 0 and <= 100 ? pageSize : 20;

        var query = db.Set<OutboxMessage>();
        var total = await query.CountAsync(ct);

        var messages = await query
            .OrderByDescending(m => m.OccurredOnUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = messages.Select(m => new EventLogItem(
            m.Id,
            ExtractEventType(m.Type),
            m.OccurredOnUtc,
            m.ProcessedOnUtc,
            m.ProcessedOnUtc.HasValue,
            SafeDeserialize(m.Content)));

        return new EventLogResponse(module, page, pageSize, total, items);
    }

    private static JsonElement SafeDeserialize(string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return default;
        try { return JsonSerializer.Deserialize<JsonElement>(content); }
        catch (JsonException) { return default; }
    }

    private static string ExtractEventType(string assemblyQualifiedName)
    {
        var typePart = assemblyQualifiedName.Split(',')[0].Trim();
        var name = typePart.Split('.')[^1];
        return string.IsNullOrEmpty(name) ? "(unknown)" : name;
    }

    public sealed record EventLogResponse(
        string Module,
        int Page,
        int PageSize,
        int Total,
        IEnumerable<EventLogItem> Items);

    public sealed record EventLogItem(
        Guid Id,
        string EventType,
        DateTime OccurredOnUtc,
        DateTime? ProcessedOnUtc,
        bool Processed,
        JsonElement Content);
}
