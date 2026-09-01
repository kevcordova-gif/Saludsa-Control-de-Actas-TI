using SaludsaActas.Application.DTOs;

namespace SaludsaActas.Application.Interfaces;

public interface IEmpleadoService
{
    Task<EmpleadoDto?> GetByIdAsync(int id);

    Task<EmpleadoDto?> GetByUsernameAsync(string username);

    Task<EmpleadoDto> CreateAsync(CreateEmpleadoDto dto);
}