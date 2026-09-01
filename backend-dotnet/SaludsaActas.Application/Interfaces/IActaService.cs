using SaludsaActas.Application.DTOs;

namespace SaludsaActas.Application.Interfaces;

public interface IActaService
{
    Task<ActaDto?> GetByIdAsync(string id);

    Task<List<ActaDto>> GetAllAsync();

    Task<ActaDto> CreateAsync(CreateActaDto dto);

    Task<ActaDto> MarcarComoFirmadaAsync(string id);
}