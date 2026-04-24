namespace JobMarketplace.SharedKernel.Ids;

public readonly record struct UserId(Guid Value)
{
    public static UserId NewId() => new(Guid.NewGuid());

    public static UserId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}