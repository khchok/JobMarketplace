using JobMarketplace.SharedKernel.Primitives;
using JobMarketplace.SharedKernel.Results;

namespace JobMarketplace.Applications.Domain.ValueObjects;

public sealed class CoverLetter : ValueObject
{
    private CoverLetter(string value) => Value = value;

    public string Value { get; }

    public static Result<CoverLetter> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<CoverLetter>.Failure(Error.Validation("Cover letter cannot be empty."));

        if (value.Length > 2000)
            return Result<CoverLetter>.Failure(Error.Validation("Cover letter cannot exceed 2000 characters."));

        return Result<CoverLetter>.Success(new CoverLetter(value));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}