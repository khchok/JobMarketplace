using FluentValidation;

namespace JobMarketplace.Jobs.Application.Commands.CreateJob;

public sealed class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
        RuleFor(x => x.SalaryMin).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SalaryMax).GreaterThanOrEqualTo(x => x.SalaryMin);
        RuleFor(x => x.Currency).NotEmpty();
    }
}