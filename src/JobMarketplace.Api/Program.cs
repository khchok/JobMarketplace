using System.Text;
using JobMarketplace.Identity.Infrastructure;
using JobMarketplace.Jobs.Infrastructure;
using JobMarketplace.Applications.Infrastructure;
using JobMarketplace.SharedKernel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using JobMarketplace.Api.Auth;
using JobMarketplace.Api.BackgroundServices;
using JobMarketplace.Api.Middleware;
using JobMarketplace.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);


// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
    opt.Events = new JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            if (ctx.Request.Cookies.TryGetValue("access_token", out var cookieToken))
                ctx.Token = cookieToken;
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddOpenApi();

// Authorization policies
builder.Services.AddAuthorizationBuilder()
                    .AddPolicy("Admin", p => p.RequireClaim("app_role", "Admin"))
                    .AddPolicy("Employer", p => p.RequireClaim("app_role", "Employer"))
                    .AddPolicy("Candidate", p => p.RequireClaim("app_role", "Candidate"))
                    .AddPolicy("CandidateOrEmployer", p => p.RequireClaim("app_role", ["Employer", "Candidate"]));


// Add infras
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddJobsInfrastructure(builder.Configuration);
builder.Services.AddApplicationsInfrastructure(builder.Configuration);

// Event handler registration by MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddSharedKernelPipeline();

// Request scoped current user accessor
builder.Services.AddScoped<ICurrentUserServiceAccessor, CurrentUserServiceAccessor>();

// Background service
builder.Services.AddHostedService<OutboxProcessor>();

builder.WebHost.UseSentry();

var app = builder.Build();

app.MapOpenApi();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", utc = DateTime.UtcNow }))
   .AllowAnonymous();

// Middleware after builder.build;
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UserProfileMiddleware>();


// finally endpoints
app.MapGroup("/api/auth").MapAuthEndpoints(builder.Configuration);
app.MapGroup("/api/identity").MapIdentityEndpoints();
app.MapGroup("/api/jobs").MapJobEndpoints();
app.MapGroup("/api/applications").MapApplicationEndpoints();
app.MapGroup("/api/admin").RequireAuthorization("Admin").MapAdminEndpoints();

app.Run();
