using SaludsaActas.Application.DTOs;

namespace SaludsaActas.Application.Interfaces;

public interface IActaDraftService
{
    Task<List<ActaDraftDto>> GetAllAsync();

    Task<ActaDraftDto?> GetByIdAsync(int id);

    Task<ActaDraftDto> CreateAsync(CreateActaDraftDto dto);

    Task DeleteAsync(int id);
}