using FluentAssertions;
using JobMarketplace.Applications.Domain.ValueObjects;

namespace JobMarketplace.Applications.Domain.Tests.ValueObjects;

// valid test
// empty test
// over limit test
// boundary test (just at the limit)
public class CoverLetterTests
{
    [Fact]
    public void Create_WithValidText_Succeeds()
    {
        var result = CoverLetter.Create("I am applying because...");
        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("I am applying because...");
    }

    [Fact]
    public void Create_WithEmptyText_Fails()
    {
        var result = CoverLetter.Create("");
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Validation");
    }

    [Fact]
    public void Create_WithTextOver2000Chars_Fails()
    {
        var result = CoverLetter.Create(new string('A', 2001));
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_WithTextExactly2000Chars_Succeeds()
    {
        var result = CoverLetter.Create(new string('A', 2000));
        result.IsSuccess.Should().BeTrue();
    }
}