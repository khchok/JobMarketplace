namespace JobMarketplace.SharedKernel.Ids;

public readonly record struct JobId(Guid Value)
{
    public static JobId NewId() => new(Guid.NewGuid());

    public static JobId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}