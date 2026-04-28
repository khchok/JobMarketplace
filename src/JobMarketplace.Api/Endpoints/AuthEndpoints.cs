using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobMarketplace.Api.Extensions;
using JobMarketplace.Identity.Application.Commands.LoginUser;
using JobMarketplace.Identity.Application.Commands.RegisterUser;
using JobMarketplace.Identity.Application.DTOs;
using JobMarketplace.Identity.Domain.Enums;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace JobMarketplace.Api.Endpoints;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this RouteGroupBuilder group, IConfiguration config)
    {
        group.MapPost("/register", async (RegisterRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new RegisterUserCommand(request.Email, request.Password, request.FullName, request.Role);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        });

        group.MapPost("/login", async (LoginRequest request, HttpContext ctx, ISender sender, IConfiguration cfg, CancellationToken ct) =>
        {
            var command = new LoginUserCommand(request.Email, request.Password);
            var result = await sender.Send(command, ct);
            if (result.IsFailure)
                return result.ToHttpResult();

            var token = GenerateToken(result.Value, cfg["Jwt:Secret"]!);
            SetAuthCookie(ctx, token);
            return Results.Ok(new { result.Value.Email, result.Value.Role });
        });

        group.MapGet("/me", (HttpContext ctx) =>
        {
            var userId = ctx.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                         ?? ctx.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = ctx.User.FindFirstValue(JwtRegisteredClaimNames.Email);
            var role = ctx.User.FindFirstValue("app_role");
            return Results.Ok(new { userId, email, role });
        }).RequireAuthorization("CandidateOrEmployer");

        group.MapPost("/logout", (HttpContext ctx) =>
        {
            ctx.Response.Cookies.Delete("access_token");
            return Results.Ok();
        });

        // Demo endpoint — seeds a ready-to-use account for each role so anyone can try the API
        group.MapPost("/demo", async (DemoRequest request, HttpContext ctx, ISender sender, IConfiguration cfg, CancellationToken ct) =>
        {
            var email = request.Role == UserRole.Employer ? "employer@jobmarket.dev" : "candidate@jobmarket.dev";
            var password = "P@ssword123";
            var fullName = request.Role == UserRole.Employer ? "Demo Employer" : "Demo Candidate";

            // Register if not exists (conflict = already seeded, proceed to login)
            var register = new RegisterUserCommand(email, password, fullName, request.Role);
            await sender.Send(register, ct);

            var login = new LoginUserCommand(email, password);
            var result = await sender.Send(login, ct);
            if (result.IsFailure)
                return result.ToHttpResult();

            var token = GenerateToken(result.Value, cfg["Jwt:Secret"]!);
            SetAuthCookie(ctx, token);
            return Results.Ok(new { email, password, note = "These are shared demo credentials." });
        });

        return group;
    }

    private static void SetAuthCookie(HttpContext ctx, string token)
    {
        var isHttps = ctx.Request.IsHttps;
        ctx.Response.Cookies.Append("access_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = isHttps ? SameSiteMode.Strict : SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }

    private static string GenerateToken(LoginResultDto user, string secret)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim("app_role", user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public sealed record RegisterRequest(string Email, string Password, string FullName, UserRole Role);
    public sealed record LoginRequest(string Email, string Password);
    public sealed record DemoRequest(UserRole Role);
}