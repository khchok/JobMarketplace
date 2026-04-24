namespace JobMarketplace.SharedKernel.Ids;

public readonly record struct ApplicationId(Guid Value)
{
    public static ApplicationId NewId() => new(Guid.NewGuid());

    public static ApplicationId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}