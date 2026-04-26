using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Applications.Application.Commands.ReviewApplication;

public sealed record ReviewApplicationCommand(Guid ApplicationId, UserId RequestingUserId) : IRequest<Result>;