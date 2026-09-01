using SaludsaActas.Domain.Entities;

namespace SaludsaActas.Domain.Interfaces;

public interface IActaRepository
{
    Task<Acta?> GetByIdAsync(string id);

    Task<List<Acta>> GetAllAsync();

    Task<string?> GetLastIdForDateAsync(DateTime date);

    Task<bool> ExistsAsync(string id);

    Task AddAsync(Acta acta);

    Task SaveChangesAsync();
}