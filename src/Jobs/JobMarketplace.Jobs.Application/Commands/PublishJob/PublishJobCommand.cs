using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Jobs.Application.Commands.PublishJob;

public sealed record PublishJobCommand(Guid JobId, UserId RequestingUserId) : IRequest<Result>;