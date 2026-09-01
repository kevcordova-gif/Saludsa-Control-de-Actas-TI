using FluentValidation;
using SaludsaActas.Application.DTOs;

namespace SaludsaActas.Application.Validators;

public class CreateActaDtoValidator : AbstractValidator<CreateActaDto>
{
    public CreateActaDtoValidator()
    {
        RuleFor(x => x.EmpleadoId)
            .GreaterThan(0)
            .WithMessage("Debe seleccionar un empleado válido.");

        RuleFor(x => x.Tipo)
            .NotEmpty()
            .WithMessage("El tipo de acta es obligatorio.")
            .Must(tipo =>
                tipo.Equals("Dotacion", StringComparison.OrdinalIgnoreCase) ||
                tipo.Equals("Renovacion", StringComparison.OrdinalIgnoreCase))
            .WithMessage("El tipo de acta debe ser Dotacion o Renovacion.");

        RuleFor(x => x)
            .Must(x =>
                (x.Activos?.Count ?? 0) > 0 ||
                (x.Accesorios?.Count ?? 0) > 0)
            .WithMessage(
                "El acta debe contener al menos un activo o un accesorio.");

        RuleForEach(x => x.Activos)
            .SetValidator(new CreateActivoDtoValidator());

        RuleForEach(x => x.Accesorios)
            .SetValidator(new CreateAccesorioDtoValidator());
    }
}