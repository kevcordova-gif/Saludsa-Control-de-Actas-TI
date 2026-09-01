using Microsoft.EntityFrameworkCore;
using SaludsaActas.Domain.Entities;
using SaludsaActas.Domain.Interfaces;
using SaludsaActas.Infrastructure.Data;

namespace SaludsaActas.Infrastructure.Repositories;

public class ActaDraftRepository : IActaDraftRepository
{
    private readonly AppDbContext _context;

    public ActaDraftRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ActaDraft>> GetAllAsync()
    {
        return await _context.ActaDrafts
            .OrderByDescending(draft => draft.UpdatedAt)
            .ToListAsync();
    }

    public async Task<ActaDraft?> GetByIdAsync(int id)
    {
        return await _context.ActaDrafts
            .FirstOrDefaultAsync(draft => draft.Id == id);
    }

    public async Task AddAsync(ActaDraft draft)
    {
        await _context.ActaDrafts.AddAsync(draft);
    }

    public void Delete(ActaDraft draft)
    {
        _context.ActaDrafts.Remove(draft);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}