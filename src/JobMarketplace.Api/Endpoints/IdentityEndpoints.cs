using JobMarketplace.Api.Auth;
using JobMarketplace.Api.Extensions;
using JobMarketplace.Identity.Application.Commands.UpdateUserProfile;
using JobMarketplace.Identity.Application.Queries.GetUserProfile;
using MediatR;

namespace JobMarketplace.Api.Endpoints;

public static class IdentityEndpoints
{
    public static RouteGroupBuilder MapIdentityEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/profile/me", async (
            ICurrentUserServiceAccessor accessor,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new GetUserProfileQuery(accessor.Current.UserId);
            var result = await sender.Send(query, ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        group.MapPut("/profile", async (
            UpdateProfileRequest request,
            ICurrentUserServiceAccessor accessor,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateUserProfileCommand(accessor.Current.UserId, request.FullName);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        }).RequireAuthorization();

        return group;
    }

    public sealed record UpdateProfileRequest(string FullName);
}