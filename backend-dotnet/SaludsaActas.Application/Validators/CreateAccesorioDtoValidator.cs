using FluentValidation;
using SaludsaActas.Application.DTOs;

namespace SaludsaActas.Application.Validators;

public class CreateAccesorioDtoValidator : AbstractValidator<CreateAccesorioDto>
{
    public CreateAccesorioDtoValidator()
    {
        RuleFor(x => x.EquipmentType)
            .NotEmpty()
            .WithMessage("El tipo de accesorio es obligatorio.")
            .MaximumLength(100)
            .WithMessage("El tipo de accesorio no puede superar los 100 caracteres.");

        RuleFor(x => x.Manufacturer)
            .NotEmpty()
            .WithMessage("El fabricante del accesorio es obligatorio.")
            .MaximumLength(100)
            .WithMessage("El fabricante no puede superar los 100 caracteres.");

        RuleFor(x => x.Model)
            .MaximumLength(100)
            .WithMessage("El modelo no puede superar los 100 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Model));

        RuleFor(x => x.SerialNumber)
            .MaximumLength(100)
            .WithMessage("El número de serie no puede superar los 100 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.SerialNumber));

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor a cero.");

        RuleFor(x => x.PurchaseCost)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El costo de compra no puede ser negativo.");

        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("El estado del accesorio es obligatorio.")
            .MaximumLength(50)
            .WithMessage("El estado no puede superar los 50 caracteres.");

        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("La ubicación del accesorio es obligatoria.")
            .MaximumLength(100)
            .WithMessage("La ubicación no puede superar los 100 caracteres.");

        RuleFor(x => x.Observation)
            .MaximumLength(500)
            .WithMessage("La observación no puede superar los 500 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Observation));
    }
}