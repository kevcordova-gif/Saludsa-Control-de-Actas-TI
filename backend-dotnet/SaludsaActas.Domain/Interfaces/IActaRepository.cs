using SaludsaActas.Domain.Entities;

namespace SaludsaActas.Domain.Interfaces;

public interface IActaRepository
{
    Task<Acta?> GetByIdAsync(string id);

    Task<List<Acta>> GetAllAsync();

    Task AddAsync(Acta acta);

    Task<bool> ExistsAsync(string id);

    Task SaveChangesAsync();
}