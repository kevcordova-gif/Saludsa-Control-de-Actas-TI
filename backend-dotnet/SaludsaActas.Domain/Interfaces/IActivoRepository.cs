using SaludsaActas.Domain.Entities;

namespace SaludsaActas.Domain.Interfaces;

public interface IActivoRepository
{
    Task<Activo?> GetBySerialNumberAsync(string serialNumber);
}