using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Jobs.Application.Commands.CreateJob;

public sealed record CreateJobCommand(
    UserId EmployerId,
    string Title,
    string Description,
    string City,
    string Country,
    decimal SalaryMin,
    decimal SalaryMax,
    string Currency) : IRequest<Result<Guid>>;