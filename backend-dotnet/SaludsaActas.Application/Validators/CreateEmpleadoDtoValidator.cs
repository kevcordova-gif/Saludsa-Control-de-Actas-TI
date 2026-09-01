using FluentValidation;
using SaludsaActas.Application.DTOs;

namespace SaludsaActas.Application.Validators;

public class CreateEmpleadoDtoValidator : AbstractValidator<CreateEmpleadoDto>
{
    public CreateEmpleadoDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("El username es obligatorio.")
            .MaximumLength(100)
            .WithMessage("El username no puede superar los 100 caracteres.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("El nombre completo es obligatorio.")
            .MaximumLength(200)
            .WithMessage("El nombre completo no puede superar los 200 caracteres.");

        RuleFor(x => x.NationalId)
            .NotEmpty()
            .WithMessage("La identificación es obligatoria.")
            .MaximumLength(20)
            .WithMessage("La identificación no puede superar los 20 caracteres.");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("La ciudad es obligatoria.")
            .MaximumLength(100)
            .WithMessage("La ciudad no puede superar los 100 caracteres.");
    }
}