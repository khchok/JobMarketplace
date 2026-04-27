using JobMarketplace.Identity.Application.DTOs;
using JobMarketplace.SharedKernel.Results;
using MediatR;

namespace JobMarketplace.Identity.Application.Commands.LoginUser;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<Result<LoginResultDto>>;
