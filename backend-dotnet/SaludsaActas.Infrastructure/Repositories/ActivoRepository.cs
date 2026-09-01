using Microsoft.EntityFrameworkCore;
using SaludsaActas.Domain.Entities;
using SaludsaActas.Domain.Interfaces;
using SaludsaActas.Infrastructure.Data;

namespace SaludsaActas.Infrastructure.Repositories;

public class ActivoRepository : IActivoRepository
{
    private readonly AppDbContext _context;

    public ActivoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Activo?> GetBySerialNumberAsync(string serialNumber)
    {
        return await _context.Activos
            .FirstOrDefaultAsync(
                activo => activo.SerialNumber == serialNumber);
    }
}