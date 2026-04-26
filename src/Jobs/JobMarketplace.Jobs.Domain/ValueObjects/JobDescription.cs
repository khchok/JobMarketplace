using JobMarketplace.SharedKernel.Primitives;
using JobMarketplace.SharedKernel.Results;

namespace JobMarketplace.Jobs.Domain.ValueObjects;

public sealed class JobDescription : ValueObject
{
    private JobDescription(string value) => Value = value;

    public string Value { get; }

    public static Result<JobDescription> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<JobDescription>.Failure(Error.Validation("Job description cannot be empty."));

        if (value.Length > 5000)
            return Result<JobDescription>.Failure(Error.Validation("Job description cannot exceed 5000 characters."));

        return Result<JobDescription>.Success(new JobDescription(value));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}