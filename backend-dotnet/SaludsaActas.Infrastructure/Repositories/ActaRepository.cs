using Microsoft.EntityFrameworkCore;
using SaludsaActas.Domain.Entities;
using SaludsaActas.Domain.Interfaces;
using SaludsaActas.Infrastructure.Data;

namespace SaludsaActas.Infrastructure.Repositories;

public class ActaRepository : IActaRepository
{
    private readonly AppDbContext _context;

    public ActaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Acta?> GetByIdAsync(string id)
    {
        return await _context.Actas
            .Include(a => a.Empleado)
            .Include(a => a.Activos)
            .Include(a => a.Accesorios)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Acta>> GetAllAsync()
    {
        return await _context.Actas
            .Include(a => a.Empleado)
            .Include(a => a.Activos)
            .Include(a => a.Accesorios)
            .OrderByDescending(a => a.Fecha)
            .ToListAsync();
    }

    public async Task<string?> GetLastIdForDateAsync(DateTime date)
    {
        var prefix = $"ACT-{date:yyyyMMdd}-";

        return await _context.Actas
            .Where(a => a.Id.StartsWith(prefix))
            .OrderByDescending(a => a.Id)
            .Select(a => a.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _context.Actas
            .AnyAsync(a => a.Id == id);
    }

    public async Task AddAsync(Acta acta)
    {
        await _context.Actas.AddAsync(acta);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}