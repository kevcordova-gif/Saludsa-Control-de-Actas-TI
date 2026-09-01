using SaludsaActas.Domain.Entities;

namespace SaludsaActas.Domain.Interfaces;

public interface IEmpleadoRepository
{
    Task<Empleado?> GetByIdAsync(int id);

    Task<Empleado?> GetByUsernameAsync(string username);

    Task<bool> ExistsByUsernameAsync(string username);

    Task AddAsync(Empleado empleado);

    Task SaveChangesAsync();
}