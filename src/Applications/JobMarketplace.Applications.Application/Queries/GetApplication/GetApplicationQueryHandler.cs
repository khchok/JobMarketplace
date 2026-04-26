using JobMarketplace.Applications.Application.DTOs;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Applications.Application.Queries.GetApplication;

public interface IApplicationReadRepository
{
    Task<ApplicationDetailDto?> GetDetailByIdAsync(Guid applicationId, CancellationToken ct = default);
}

public sealed class GetApplicationQueryHandler(IApplicationReadRepository readRepository)
    : IRequestHandler<GetApplicationQuery, Result<ApplicationDetailDto>>
{
    public async Task<Result<ApplicationDetailDto>> Handle(
        GetApplicationQuery request, CancellationToken cancellationToken)
    {
        var dto = await readRepository.GetDetailByIdAsync(request.ApplicationId, cancellationToken);
        if (dto is null)
            return Result<ApplicationDetailDto>.Failure(
                Error.NotFound($"Application {request.ApplicationId} not found."));

        return Result<ApplicationDetailDto>.Success(dto);
    }
}