using SaludsaActas.Application.DTOs;
using SaludsaActas.Application.Interfaces;
using SaludsaActas.Domain.Entities;
using SaludsaActas.Domain.Interfaces;

namespace SaludsaActas.Application.Services;

public class ActaService : IActaService
{
    private readonly IActaRepository _actaRepository;

    public ActaService(IActaRepository actaRepository)
    {
        _actaRepository = actaRepository;
    }

    public async Task<ActaDto?> GetByIdAsync(string id)
    {
        var acta = await _actaRepository.GetByIdAsync(id);

        return acta is null ? null : MapToDto(acta);
    }

    public async Task<List<ActaDto>> GetAllAsync()
    {
        var actas = await _actaRepository.GetAllAsync();

        return actas
            .Select(MapToDto)
            .ToList();
    }

    private static ActaDto MapToDto(Acta acta)
    {
        return new ActaDto
        {
            Id = acta.Id,
            Fecha = acta.Fecha,
            Tipo = acta.Tipo,
            Estado = acta.Estado,
            SincronizadoSaludsa = acta.SincronizadoSaludsa,
            EstadoSincronizacion = acta.EstadoSincronizacion,
            TimestampSincronizacion = acta.TimestampSincronizacion,
            TienePagare = acta.TienePagare,
            ArchivoActa = acta.ArchivoActa,
            ArchivoPagare = acta.ArchivoPagare,

            Empleado = new EmpleadoDto
            {
                Id = acta.Empleado.Id,
                Username = acta.Empleado.Username,
                FullName = acta.Empleado.FullName,
                NationalId = acta.Empleado.NationalId,
                City = acta.Empleado.City
            },

            Activos = acta.Activos.Select(a => new ActivoDto
            {
                Id = a.Id,
                Manufacturer = a.Manufacturer,
                Model = a.Model,
                SerialNumber = a.SerialNumber,
                Hostname = a.Hostname,
                PurchaseCost = a.PurchaseCost,
                Status = a.Status,
                Location = a.Location,
                Observation = a.Observation
            }).ToList(),

            Accesorios = acta.Accesorios.Select(a => new AccesorioDto
            {
                Id = a.Id,
                EquipmentType = a.EquipmentType,
                Manufacturer = a.Manufacturer,
                Model = a.Model,
                SerialNumber = a.SerialNumber,
                Quantity = a.Quantity,
                PurchaseCost = a.PurchaseCost,
                Status = a.Status,
                Location = a.Location,
                Observation = a.Observation
            }).ToList()
        };
    }
}