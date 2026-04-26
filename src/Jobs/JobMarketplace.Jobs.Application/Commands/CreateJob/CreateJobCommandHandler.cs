using JobMarketplace.Jobs.Application.Interfaces;
using JobMarketplace.Jobs.Domain.Aggregates;
using JobMarketplace.Jobs.Domain.Repositories;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Jobs.Application.Commands.CreateJob;

public sealed class CreateJobCommandHandler(
    IJobRepository repository,
    IJobsUnitOfWork unitOfWork)
    : IRequestHandler<CreateJobCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        var result = Job.Create(
            request.EmployerId,
            request.Title,
            request.Description,
            request.City,
            request.Country,
            request.SalaryMin,
            request.SalaryMax,
            request.Currency);

        if (result.IsFailure)
            return Result<Guid>.Failure(result.Error);

        repository.Add(result.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(result.Value.Id.Value);
    }
}