using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Applications.Application.Commands.RejectApplication;

public sealed record RejectApplicationCommand(Guid ApplicationId, UserId RequestingUserId) : IRequest<Result>;