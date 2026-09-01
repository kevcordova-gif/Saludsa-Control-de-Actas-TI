using SaludsaActas.Application.DTOs;

namespace SaludsaActas.Application.Interfaces;

public interface IActaService
{
    Task<ActaDto?> GetByIdAsync(string id);

    Task<List<ActaDto>> GetAllAsync();
}