namespace JobMarketplace.Applications.Infrastructure.Persistence.ReadModels;

internal sealed class JobReadModel
{
    public Guid Id { get; set; }
    public Guid EmployerId { get; set; }
    public string Status { get; set; } = string.Empty;
}