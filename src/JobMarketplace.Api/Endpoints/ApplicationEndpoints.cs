using JobMarketplace.Api.Auth;
using JobMarketplace.Api.Extensions;
using JobMarketplace.Applications.Application.Commands.AcceptApplication;
using JobMarketplace.Applications.Application.Commands.RejectApplication;
using JobMarketplace.Applications.Application.Commands.ReviewApplication;
using JobMarketplace.Applications.Application.Commands.SubmitApplication;
using JobMarketplace.Applications.Application.Queries.GetApplication;
using JobMarketplace.Applications.Application.Queries.ListApplicationsForJob;
using JobMarketplace.Applications.Application.Queries.ListMyApplications;
using MediatR;

namespace JobMarketplace.Api.Endpoints;

public static class ApplicationEndpoints
{
    public static RouteGroupBuilder MapApplicationEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            SubmitRequest request,
            ICurrentUserServiceAccessor accessor,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new SubmitApplicationCommand(
                request.JobId, accessor.Current.UserId, request.CoverLetter);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/applications/{result.Value}", result.Value)
                : result.ToHttpResult();
        }).RequireAuthorization("Candidate");

        group.MapGet("/mine", async (
            int page, int pageSize,
            ICurrentUserServiceAccessor accessor,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new ListMyApplicationsQuery(
                accessor.Current.UserId,
                page > 0 ? page : 1,
                pageSize > 0 ? pageSize : 20);
            var result = await sender.Send(query, ct);
            return result.ToHttpResult();
        }).RequireAuthorization("Candidate");

        group.MapGet("/job/{jobId:guid}", async (
            Guid jobId, int page, int pageSize,
            ICurrentUserServiceAccessor accessor,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new ListApplicationsForJobQuery(
                jobId, accessor.Current.UserId,
                page > 0 ? page : 1,
                pageSize > 0 ? pageSize : 20);
            var result = await sender.Send(query, ct);
            return result.ToHttpResult();
        }).RequireAuthorization("Employer");

        group.MapGet("/{id:guid}", async (
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetApplicationQuery(id), ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        group.MapPut("/{id:guid}/review", async (
            Guid id,
            ICurrentUserServiceAccessor accessor,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new ReviewApplicationCommand(id, accessor.Current.UserId), ct);
            return result.ToHttpResult();
        }).RequireAuthorization("Employer");

        group.MapPut("/{id:guid}/accept", async (
            Guid id,
            ICurrentUserServiceAccessor accessor,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new AcceptApplicationCommand(id, accessor.Current.UserId), ct);
            return result.ToHttpResult();
        }).RequireAuthorization("Employer");

        group.MapPut("/{id:guid}/reject", async (
            Guid id,
            ICurrentUserServiceAccessor accessor,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new RejectApplicationCommand(id, accessor.Current.UserId), ct);
            return result.ToHttpResult();
        }).RequireAuthorization("Employer");

        return group;
    }

    public sealed record SubmitRequest(Guid JobId, string CoverLetter);
}