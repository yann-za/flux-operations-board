using FluentValidation;

namespace FluxOperations.Application.Commands.Fluxes;

public sealed class CreateFluxCommandValidator : AbstractValidator<CreateFluxCommand>
{
    public CreateFluxCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Flux name is required.")
            .MaximumLength(200).WithMessage("Flux name must not exceed 200 characters.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid flux type.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).When(x => x.Description is not null);

        RuleFor(x => x.ScheduleCron)
            .Matches(@"^(\*|[0-9,-/]+)\s+(\*|[0-9,-/]+)\s+(\*|[0-9,-/]+)\s+(\*|[0-9,-/]+)\s+(\*|[0-9,-/]+)$")
            .WithMessage("ScheduleCron must be a valid cron expression.")
            .When(x => !string.IsNullOrWhiteSpace(x.ScheduleCron));
    }
}
