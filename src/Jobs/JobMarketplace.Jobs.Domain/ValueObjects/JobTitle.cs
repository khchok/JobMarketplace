using JobMarketplace.SharedKernel.Primitives;
using JobMarketplace.SharedKernel.Results;

namespace JobMarketplace.Jobs.Domain.ValueObjects;

public sealed class JobTitle : ValueObject
{
    public string Value { get; }
    private JobTitle(string value) => Value = value;

    public static Result<JobTitle> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result<JobTitle>.Failure(Error.Validation("Job title cannot be empty."));

        if (title.Length > 100)
            return Result<JobTitle>.Failure(Error.Validation("Job title cannot exceed 100 characters."));

        return Result<JobTitle>.Success(new JobTitle(title.Trim()));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}