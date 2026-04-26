using JobMarketplace.Jobs.Application.DTOs;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Jobs.Application.Queries.GetJob;

public sealed record GetJobQuery(Guid JobId) : IRequest<Result<JobDetailDto>>;