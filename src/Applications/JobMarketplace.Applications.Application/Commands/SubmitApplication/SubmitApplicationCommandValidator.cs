using FluentValidation;

namespace JobMarketplace.Applications.Application.Commands.SubmitApplication;

public sealed class SubmitApplicationCommandValidator : AbstractValidator<SubmitApplicationCommand>
{
    public SubmitApplicationCommandValidator()
    {
        RuleFor(x => x.JobId).NotEmpty();
        RuleFor(x => x.CoverLetter).NotEmpty().MaximumLength(2000);
    }
}