using JobMarketplace.Identity.Domain.Enums;
using JobMarketplace.Identity.Domain.Events;
using JobMarketplace.Identity.Domain.ValueObjects;
using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Primitives;
using JobMarketplace.SharedKernel.Results;

namespace JobMarketplace.Identity.Domain.Aggregates;

public sealed class UserProfile : AggregateRoot<UserId>
{
    private UserProfile(
        UserId id,
        string supabaseUserId,
        EmailAddress email,
        string fullName,
        UserRole role,
        DateTime createdAt)
        : base(id)
    {
        SupabaseUserId = supabaseUserId;
        Email = email;
        FullName = fullName;
        Role = role;
        CreatedAt = createdAt;
    }

    public string SupabaseUserId { get; private set; } = string.Empty;
    public EmailAddress Email { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static Result<UserProfile> Create(string supabaseUserId, string email, string fullName, UserRole role)
    {
        var userId = UserId.NewId();
        var createdAt = DateTime.UtcNow;
        var emailResult = EmailAddress.Create(email);

        if (emailResult.IsFailure)
            return Result<UserProfile>.Failure(emailResult.Error);

        var userProfile = new UserProfile(userId, supabaseUserId, emailResult.Value, fullName, role, createdAt);
        userProfile.RaiseDomainEvent(new UserProfileCreatedEvent(userId, supabaseUserId));


        return Result<UserProfile>.Success(userProfile);
    }

    public Result UpdateFullName(string newFullName)
    {
        if (string.IsNullOrEmpty(newFullName))
        {
            return Result.Failure(Error.Validation("Full name cannot be empty."));
        }

        FullName = newFullName;

        return Result.Success();
    }
}