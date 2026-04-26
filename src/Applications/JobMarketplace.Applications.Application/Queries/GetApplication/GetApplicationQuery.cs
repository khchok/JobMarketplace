using JobMarketplace.Applications.Application.DTOs;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Applications.Application.Queries.GetApplication;

public sealed record GetApplicationQuery(Guid ApplicationId) : IRequest<Result<ApplicationDetailDto>>;