namespace JobMarketplace.Applications.Infrastructure.Persistence.ReadModels;

internal sealed class JobReadModel
{
    public Guid Id { get; set; }
    public Guid EmployerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}