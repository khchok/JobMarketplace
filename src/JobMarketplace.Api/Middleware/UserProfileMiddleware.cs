using System.Security.Claims;
using JobMarketplace.Api.Auth;
using JobMarketplace.Identity.Domain.Enums;
using JobMarketplace.SharedKernel.Ids;

namespace JobMarketplace.Api.Middleware;

public sealed class UserProfileMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var sub = context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? context.User.FindFirstValue("sub");
            var roleStr = context.User.FindFirstValue("app_role");

            if (sub is not null && Guid.TryParse(sub, out var userId) && roleStr is not null
                && Enum.TryParse<UserRole>(roleStr, out var role))
            {
                var currentUser = new CurrentUserService
                {
                    UserId = UserId.From(userId),
                    Role = role
                };
                context.RequestServices.GetRequiredService<ICurrentUserServiceAccessor>().Set(currentUser);
            }
        }

        await next(context);
    }
}