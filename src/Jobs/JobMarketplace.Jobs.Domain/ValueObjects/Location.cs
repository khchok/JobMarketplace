using JobMarketplace.SharedKernel.Primitives;
using JobMarketplace.SharedKernel.Results;

namespace JobMarketplace.Jobs.Domain.ValueObjects;

public sealed class Location : ValueObject
{
    private Location(string city, string country)
    {
        City = city;
        Country = country;
    }

    public string City { get; }
    public string Country { get; }

    public static Result<Location> Create(string city, string country)
    {
        if (string.IsNullOrWhiteSpace(city))
            return Result<Location>.Failure(Error.Validation("City cannot be empty."));

        if (string.IsNullOrWhiteSpace(country))
            return Result<Location>.Failure(Error.Validation("Country cannot be empty."));

        return Result<Location>.Success(new Location(city, country));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return City;
        yield return Country;
    }
}