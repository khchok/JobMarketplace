using JobMarketplace.Api.Auth;
using JobMarketplace.Api.Extensions;
using JobMarketplace.Jobs.Application.Commands.CloseJob;
using JobMarketplace.Jobs.Application.Commands.CreateJob;
using JobMarketplace.Jobs.Application.Commands.PublishJob;
using JobMarketplace.Jobs.Application.Queries.GetJob;
using JobMarketplace.Jobs.Application.Queries.ListJobs;
using JobMarketplace.Jobs.Application.Queries.ListMyJobs;
using MediatR;

namespace JobMarketplace.Api.Endpoints;

public static class JobEndpoints
{
    public static RouteGroupBuilder MapJobEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateJobRequest request,
            ICurrentUserServiceAccessor accessor,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateJobCommand(
                accessor.Current.UserId,
                request.Title, request.Description,
                request.City, request.Country,
                request.SalaryMin, request.SalaryMax, request.Currency);

            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/jobs/{result.Value}", result.Value)
                : result.ToHttpResult();
        }).RequireAuthorization("Employer");

        group.MapGet("/", async (
            string? keyword, string? country, string? city, decimal? salaryMin,
            int page, int pageSize,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new ListJobsQuery(keyword, country, city, salaryMin,
                Page: page > 0 ? page : 1,
                PageSize: pageSize > 0 ? pageSize : 20);
            var result = await sender.Send(query, ct);
            return result.ToHttpResult();
        });

        group.MapGet("/mine", async (
            int page, int pageSize,
            ICurrentUserServiceAccessor accessor,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new ListMyJobsQuery(
                accessor.Current.UserId,
                page > 0 ? page : 1,
                pageSize > 0 ? pageSize : 20);
            var result = await sender.Send(query, ct);
            return result.ToHttpResult();
        }).RequireAuthorization("Employer");

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetJobQuery(id), ct);
            return result.ToHttpResult();
        });

        group.MapPut("/{id:guid}/publish", async (
            Guid id,
            ICurrentUserServiceAccessor accessor,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new PublishJobCommand(id, accessor.Current.UserId), ct);
            return result.ToHttpResult();
        }).RequireAuthorization("Employer");

        group.MapPut("/{id:guid}/close", async (
            Guid id,
            ICurrentUserServiceAccessor accessor,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new CloseJobCommand(id, accessor.Current.UserId), ct);
            return result.ToHttpResult();
        }).RequireAuthorization("Employer");

        return group;
    }

    public sealed record CreateJobRequest(
        string Title,
        string Description,
        string City,
        string Country,
        decimal SalaryMin,
        decimal SalaryMax,
        string Currency);
}