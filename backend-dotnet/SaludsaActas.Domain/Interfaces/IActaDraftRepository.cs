using SaludsaActas.Domain.Entities;

namespace SaludsaActas.Domain.Interfaces;

public interface IActaDraftRepository
{
    Task<List<ActaDraft>> GetAllAsync();

    Task<ActaDraft?> GetByIdAsync(int id);

    Task AddAsync(ActaDraft draft);

    void Delete(ActaDraft draft);

    Task SaveChangesAsync();
}