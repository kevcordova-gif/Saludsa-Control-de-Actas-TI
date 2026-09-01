using FluentValidation;
using SaludsaActas.Application.DTOs;

namespace SaludsaActas.Application.Validators;

public class CreateActivoDtoValidator : AbstractValidator<CreateActivoDto>
{
    public CreateActivoDtoValidator()
    {
        RuleFor(x => x.Manufacturer)
            .NotEmpty()
            .WithMessage("El fabricante del activo es obligatorio.")
            .MaximumLength(100)
            .WithMessage("El fabricante no puede superar los 100 caracteres.");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("El modelo del activo es obligatorio.")
            .MaximumLength(100)
            .WithMessage("El modelo no puede superar los 100 caracteres.");

        RuleFor(x => x.SerialNumber)
            .NotEmpty()
            .WithMessage("El número de serie es obligatorio.")
            .MaximumLength(100)
            .WithMessage("El número de serie no puede superar los 100 caracteres.");

        RuleFor(x => x.Hostname)
            .NotEmpty()
            .WithMessage("El hostname es obligatorio.")
            .MaximumLength(100)
            .WithMessage("El hostname no puede superar los 100 caracteres.");

        RuleFor(x => x.PurchaseCost)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El costo de compra no puede ser negativo.");

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("El estado del activo es obligatorio.")
            .MaximumLength(50)
            .WithMessage("El estado no puede superar los 50 caracteres.");

        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("La ubicación del activo es obligatoria.")
            .MaximumLength(100)
            .WithMessage("La ubicación no puede superar los 100 caracteres.");

        RuleFor(x => x.Observation)
            .MaximumLength(500)
            .WithMessage("La observación no puede superar los 500 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Observation));
    }
}