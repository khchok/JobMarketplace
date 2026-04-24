namespace JobMarketplace.SharedKernel.Results;

public sealed class Error
{
    public static readonly Error None = new(string.Empty, string.Empty);

    private Error(string code, string description)
    {
        Code = code;
        Description = description;
    }

    public string Code { get; }
    public string Description { get; }

    public static Error NotFound(string description) => new("NotFound", description);
    public static Error Unauthorized(string description) => new("Unauthorized", description);
    public static Error Conflict(string description) => new("Conflict", description);
    public static Error Validation(string description) => new("Validation", description);
}