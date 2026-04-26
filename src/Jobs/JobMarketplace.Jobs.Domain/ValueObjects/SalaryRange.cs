using JobMarketplace.SharedKernel.Primitives;
using JobMarketplace.SharedKernel.Results;

namespace JobMarketplace.Jobs.Domain.ValueObjects;

public sealed class SalaryRange : ValueObject
{
    private SalaryRange(decimal min, decimal max, string currency)
    {
        Min = min;
        Max = max;
        Currency = currency;
    }

    public decimal Min { get; }
    public decimal Max { get; }
    public string Currency { get; }

    public static Result<SalaryRange> Create(decimal min, decimal max, string currency)
    {
        if (min < 0)
            return Result<SalaryRange>.Failure(Error.Validation("Salary minimum cannot be negative."));

        if (min > max)
            return Result<SalaryRange>.Failure(Error.Validation("Salary minimum cannot exceed maximum."));

        if (string.IsNullOrWhiteSpace(currency))
            return Result<SalaryRange>.Failure(Error.Validation("Currency cannot be empty."));

        return Result<SalaryRange>.Success(new SalaryRange(min, max, currency));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Min;
        yield return Max;
        yield return Currency;
    }
}