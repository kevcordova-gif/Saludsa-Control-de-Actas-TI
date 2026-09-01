using Microsoft.EntityFrameworkCore;
using SaludsaActas.Domain.Entities;
using SaludsaActas.Domain.Interfaces;
using SaludsaActas.Infrastructure.Data;

namespace SaludsaActas.Infrastructure.Repositories;

public class EmpleadoRepository : IEmpleadoRepository
{
    private readonly AppDbContext _context;

    public EmpleadoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Empleado?> GetByIdAsync(int id)
    {
        return await _context.Empleados
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Empleado?> GetByUsernameAsync(string username)
    {
        return await _context.Empleados
            .FirstOrDefaultAsync(e => e.Username == username);
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _context.Empleados
            .AnyAsync(e => e.Username == username);
    }

    public async Task AddAsync(Empleado empleado)
    {
        await _context.Empleados.AddAsync(empleado);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}