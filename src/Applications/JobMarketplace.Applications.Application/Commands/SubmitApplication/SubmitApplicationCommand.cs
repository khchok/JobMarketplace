using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Applications.Application.Commands.SubmitApplication;

public sealed record SubmitApplicationCommand(
    Guid JobId,
    UserId CandidateId,
    string CoverLetter) : IRequest<Result<Guid>>;