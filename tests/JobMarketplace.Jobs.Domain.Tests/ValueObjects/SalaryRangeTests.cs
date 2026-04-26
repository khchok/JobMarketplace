using JobMarketplace.Jobs.Domain.ValueObjects;
using FluentAssertions;

namespace JobMarketplace.Jobs.Domain.Tests.ValueObjects;

public class SalaryRangeTests
{
    [Fact]
    public void Create_WithValidRange_Succeeds()
    {
        var result = SalaryRange.Create(3000m, 5000m, "MYR");
        result.IsSuccess.Should().BeTrue();
        result.Value.Min.Should().Be(3000m);
        result.Value.Max.Should().Be(5000m);
        result.Value.Currency.Should().Be("MYR");
    }

    [Fact]
    public void Create_WithMinGreaterThanMax_Fails()
    {
        var result = SalaryRange.Create(6000m, 5000m, "MYR");
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation");
    }

    [Fact]
    public void Create_WithNegativeMin_Fails()
    {
        var result = SalaryRange.Create(-100m, 5000m, "MYR");
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_WithMinEqualToMax_Succeeds()
    {
        var result = SalaryRange.Create(5000m, 5000m, "USD");
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_WithEmptyCurrency_Fails()
    {
        var result = SalaryRange.Create(1000m, 5000m, "");
        result.IsFailure.Should().BeTrue();
    }
}