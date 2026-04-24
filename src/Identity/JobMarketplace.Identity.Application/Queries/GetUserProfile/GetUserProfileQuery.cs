using JobMarketplace.Identity.Application.DTOs;
using JobMarketplace.SharedKernel.Ids;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Identity.Application.Queries.GetUserProfile;

public sealed record GetUserProfileQuery(UserId UserId) : IRequest<Result<UserProfileDto>>;