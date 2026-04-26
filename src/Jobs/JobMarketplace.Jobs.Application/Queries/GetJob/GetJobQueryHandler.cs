using JobMarketplace.Jobs.Application.DTOs;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Jobs.Application.Queries.GetJob;

public interface IJobReadRepository
{
    Task<JobDetailDto?> GetDetailByIdAsync(Guid jobId, CancellationToken ct = default);
}

public sealed class GetJobQueryHandler(IJobReadRepository readRepository)
    : IRequestHandler<GetJobQuery, Result<JobDetailDto>>
{
    public async Task<Result<JobDetailDto>> Handle(GetJobQuery request, CancellationToken cancellationToken)
    {
        var dto = await readRepository.GetDetailByIdAsync(request.JobId, cancellationToken);
        if (dto is null)
            return Result<JobDetailDto>.Failure(Error.NotFound($"Job {request.JobId} not found."));

        return Result<JobDetailDto>.Success(dto);
    }
}