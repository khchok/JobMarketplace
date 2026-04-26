using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Jobs.Application.Commands.CloseJob;

public sealed record CloseJobCommand(Guid JobId, UserId RequestingUserId) : IRequest<Result>;