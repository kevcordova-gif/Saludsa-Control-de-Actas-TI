using FluentValidation;
using SaludsaActas.Application.DTOs;
using SaludsaActas.Application.Interfaces;
using SaludsaActas.Domain.Entities;
using SaludsaActas.Domain.Interfaces;

namespace SaludsaActas.Application.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IValidator<CreateEmpleadoDto> _validator;

    public EmpleadoService(
        IEmpleadoRepository empleadoRepository,
        IValidator<CreateEmpleadoDto> validator)
    {
        _empleadoRepository = empleadoRepository;
        _validator = validator;
    }

    public async Task<EmpleadoDto?> GetByIdAsync(int id)
    {
        var empleado = await _empleadoRepository.GetByIdAsync(id);

        if (empleado is null)
            return null;

        return MapToDto(empleado);
    }

    public async Task<EmpleadoDto?> GetByUsernameAsync(string username)
    {
        var empleado = await _empleadoRepository.GetByUsernameAsync(username);

        if (empleado is null)
            return null;

        return MapToDto(empleado);
    }

    public async Task<EmpleadoDto> CreateAsync(CreateEmpleadoDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var username = dto.Username.Trim();

        var exists = await _empleadoRepository.ExistsByUsernameAsync(username);

        if (exists)
        {
            throw new InvalidOperationException(
                "Ya existe un empleado con ese username.");
        }

        var empleado = new Empleado
        {
            Username = username,
            FullName = dto.FullName.Trim(),
            NationalId = dto.NationalId.Trim(),
            City = dto.City.Trim()
        };

        await _empleadoRepository.AddAsync(empleado);
        await _empleadoRepository.SaveChangesAsync();

        return MapToDto(empleado);
    }

    private static EmpleadoDto MapToDto(Empleado empleado)
    {
        return new EmpleadoDto
        {
            Id = empleado.Id,
            Username = empleado.Username,
            FullName = empleado.FullName,
            NationalId = empleado.NationalId,
            City = empleado.City
        };
    }
}