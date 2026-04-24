using JobMarketplace.SharedKernel.Primitives;
using JobMarketplace.SharedKernel.Results;

namespace JobMarketplace.Identity.Domain.ValueObjects;

public sealed class EmailAddress : ValueObject
{
    private EmailAddress(string value) => Value = value;
    public string Value { get; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Result<EmailAddress> Create(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return Result<EmailAddress>.Failure(Error.Validation("Email address cannot be empty."));
        }

        if (!email.Contains('@'))
        {
            return Result<EmailAddress>.Failure(Error.Validation("Invalid email address format."));
        }

        return Result<EmailAddress>.Success(new EmailAddress(email.Trim().ToLowerInvariant()));
    }
}