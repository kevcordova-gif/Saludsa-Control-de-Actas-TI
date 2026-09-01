using SaludsaActas.Application.DTOs;
using SaludsaActas.Application.Interfaces;
using SaludsaActas.Domain.Interfaces;

namespace SaludsaActas.Application.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly IEmpleadoRepository _empleadoRepository;

    public EmpleadoService(IEmpleadoRepository empleadoRepository)
    {
        _empleadoRepository = empleadoRepository;
    }

    public async Task<EmpleadoDto?> GetByIdAsync(int id)
    {
        var empleado = await _empleadoRepository.GetByIdAsync(id);

        if (empleado is null)
            return null;

        return new EmpleadoDto
        {
            Id = empleado.Id,
            Username = empleado.Username,
            FullName = empleado.FullName,
            NationalId = empleado.NationalId,
            City = empleado.City
        };
    }

    public async Task<EmpleadoDto?> GetByUsernameAsync(string username)
    {
        var empleado = await _empleadoRepository.GetByUsernameAsync(username);

        if (empleado is null)
            return null;

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