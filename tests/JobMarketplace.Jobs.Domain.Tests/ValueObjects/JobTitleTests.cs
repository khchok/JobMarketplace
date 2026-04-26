using JobMarketplace.Jobs.Domain.ValueObjects;
using FluentAssertions;

namespace JobMarketplace.Jobs.Domain.Tests.ValueObjects;

public class JobTitleTests
{
    [Fact]
    public void Create_WithValidTitle_ShouldSucceed()
    {
        var title = "Senior .Net Developer";
        var result = JobTitle.Create(title);
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(title);
    }

    [Fact]
    public void Create_WithEmptyTitle_ShouldFail()
    {
        var result = JobTitle.Create("");
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation");
    }

    [Fact]
    public void Create_WithWhitespaceTitle_Fails()
    {
        var result = JobTitle.Create("   ");
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_WithTitleOver100Chars_Fails()
    {
        var longTitle = new string('A', 101);
        var result = JobTitle.Create(longTitle);
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_WithTitleExactly100Chars_Succeeds()
    {
        var title = new string('A', 100);
        var result = JobTitle.Create(title);
        result.IsSuccess.Should().BeTrue();
    }
}