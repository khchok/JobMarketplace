using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Applications.Application.Commands.AcceptApplication;

public sealed record AcceptApplicationCommand(Guid ApplicationId, UserId RequestingUserId) : IRequest<Result>;