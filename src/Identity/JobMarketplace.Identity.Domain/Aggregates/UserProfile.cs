using JobMarketplace.Identity.Domain.Enums;
using JobMarketplace.Identity.Domain.Events;
using JobMarketplace.Identity.Domain.ValueObjects;
using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Primitives;
using JobMarketplace.SharedKernel.Results;

namespace JobMarketplace.Identity.Domain.Aggregates;

public sealed class UserProfile : AggregateRoot<UserId>
{
    private UserProfile() : base(default!) { } // For EF Core materialization

    private UserProfile(
        UserId id,
        EmailAddress email,
        string fullName,
        UserRole role,
        string passwordHash,
        DateTime createdAt)
        : base(id)
    {
        Email = email;
        FullName = fullName;
        Role = role;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public EmailAddress Email { get; private set; } = null!;
    public string FullName { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public static Result<UserProfile> Create(string email, string fullName, UserRole role, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return Result<UserProfile>.Failure(Error.Validation("Full name cannot be empty."));

        var emailResult = EmailAddress.Create(email);
        if (emailResult.IsFailure)
            return Result<UserProfile>.Failure(emailResult.Error);

        var profile = new UserProfile(
            UserId.NewId(),
            emailResult.Value,
            fullName,
            role,
            passwordHash,
            DateTime.UtcNow);

        profile.RaiseDomainEvent(new UserProfileCreatedEvent(profile.Id));

        return Result<UserProfile>.Success(profile);
    }

    public Result UpdateFullName(string newFullName)
    {
        if (string.IsNullOrEmpty(newFullName))
            return Result.Failure(Error.Validation("Full name cannot be empty."));

        FullName = newFullName;
        return Result.Success();
    }
}
