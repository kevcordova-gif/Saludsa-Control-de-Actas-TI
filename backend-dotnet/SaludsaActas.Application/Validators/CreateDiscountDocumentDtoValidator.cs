using FluentValidation;
using SaludsaActas.Application.DTOs;

namespace SaludsaActas.Application.Validators;

public class CreateDiscountDocumentDtoValidator
    : AbstractValidator<CreateDiscountDocumentDto>
{
    public CreateDiscountDocumentDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage(
                "El username del empleado es obligatorio.");

        RuleFor(x => x.DeductionMonth)
            .NotEmpty()
            .WithMessage(
                "El mes de descuento es obligatorio.");

        RuleFor(x => x.Equipos)
            .NotNull()
            .WithMessage(
                "Debe incluir al menos un equipo a descontar.")
            .Must(equipos =>
                equipos is not null &&
                equipos.Count > 0)
            .WithMessage(
                "Debe incluir al menos un equipo a descontar.");

        RuleForEach(x => x.Equipos)
            .ChildRules(equipo =>
            {
                equipo.RuleFor(x => x.EquipmentType)
                    .NotEmpty()
                    .WithMessage(
                        "El tipo de equipo es obligatorio.");

                equipo.RuleFor(x => x.Manufacturer)
                    .NotEmpty()
                    .WithMessage(
                        "El fabricante es obligatorio.");

                equipo.RuleFor(x => x.Model)
                    .NotEmpty()
                    .WithMessage(
                        "El modelo es obligatorio.");

                equipo.RuleFor(x => x.SerialNumber)
                    .NotEmpty()
                    .WithMessage(
                        "El número de serie es obligatorio.");

                equipo.RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage(
                        "La cantidad debe ser mayor a cero.");

                equipo.RuleFor(x => x.PurchaseCost)
                    .GreaterThan(0)
                    .WithMessage(
                        "El valor a descontar debe ser mayor a cero.");
            });
    }
}