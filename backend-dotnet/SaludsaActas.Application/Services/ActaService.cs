using FluentValidation;
using SaludsaActas.Application.DTOs;
using SaludsaActas.Application.Interfaces;
using SaludsaActas.Domain.Entities;
using SaludsaActas.Domain.Interfaces;

namespace SaludsaActas.Application.Services;

public class ActaService : IActaService
{
    private readonly IActaRepository _actaRepository;
    private readonly IEmpleadoRepository _empleadoRepository;
    private readonly IActivoRepository _activoRepository;
    private readonly IValidator<CreateActaDto> _validator;

    public ActaService(
        IActaRepository actaRepository,
        IEmpleadoRepository empleadoRepository,
        IActivoRepository activoRepository,
        IValidator<CreateActaDto> validator)
    {
        _actaRepository = actaRepository;
        _empleadoRepository = empleadoRepository;
        _activoRepository = activoRepository;
        _validator = validator;
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

    public async Task<ActaDto> CreateAsync(CreateActaDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var empleado = await _empleadoRepository.GetByIdAsync(dto.EmpleadoId);

        if (empleado is null)
        {
            throw new InvalidOperationException(
                "El empleado seleccionado no existe.");
        }

        var fecha = DateTime.UtcNow;

        var actaId = await GenerateActaIdAsync(fecha);

        var tipo = dto.Tipo.Equals(
            "Renovacion",
            StringComparison.OrdinalIgnoreCase)
            ? "Renovacion"
            : "Dotacion";

        var acta = new Acta
        {
            Id = actaId,
            Fecha = fecha,
            Tipo = tipo,
            Estado = "PENDIENTE_FIRMA",
            SincronizadoSaludsa = false,
            EmpleadoId = empleado.Id,
            Empleado = empleado,
            TienePagare = dto.Activos.Count > 0
        };

        var activosProcesados =
            new Dictionary<string, Activo>(
                StringComparer.OrdinalIgnoreCase);

        foreach (var activoDto in dto.Activos)
        {
            var serialNumber = activoDto.SerialNumber.Trim();

            Activo activo;

            if (activosProcesados.TryGetValue(
                serialNumber,
                out var activoProcesado))
            {
                activo = activoProcesado;
            }
            else
            {
                var activoExistente =
                    await _activoRepository
                        .GetBySerialNumberAsync(serialNumber);

                if (activoExistente is not null)
                {
                    activo = activoExistente;

                    activo.Manufacturer =
                        activoDto.Manufacturer.Trim();

                    activo.Model =
                        activoDto.Model.Trim();

                    activo.Hostname =
                        activoDto.Hostname.Trim();

                    activo.PurchaseCost =
                        activoDto.PurchaseCost;

                    activo.Status =
                        activoDto.Status.Trim();

                    activo.Location =
                        activoDto.Location.Trim();

                    activo.Observation =
                        string.IsNullOrWhiteSpace(
                            activoDto.Observation)
                            ? null
                            : activoDto.Observation.Trim();
                }
                else
                {
                    activo = new Activo
                    {
                        Manufacturer =
                            activoDto.Manufacturer.Trim(),

                        Model =
                            activoDto.Model.Trim(),

                        SerialNumber =
                            serialNumber,

                        Hostname =
                            activoDto.Hostname.Trim(),

                        PurchaseCost =
                            activoDto.PurchaseCost,

                        Status =
                            activoDto.Status.Trim(),

                        Location =
                            activoDto.Location.Trim(),

                        Observation =
                            string.IsNullOrWhiteSpace(
                                activoDto.Observation)
                                ? null
                                : activoDto.Observation.Trim(),

                        FechaRegistro =
                            DateTime.UtcNow
                    };
                }

                activosProcesados[serialNumber] = activo;
            }

            if (!acta.Activos.Contains(activo))
            {
                acta.Activos.Add(activo);
            }
        }

        foreach (var accesorioDto in dto.Accesorios)
        {
            var accesorio = new Accesorio
            {
                EquipmentType =
                    accesorioDto.EquipmentType.Trim(),

                Manufacturer =
                    accesorioDto.Manufacturer.Trim(),

                Model =
                    string.IsNullOrWhiteSpace(
                        accesorioDto.Model)
                        ? null
                        : accesorioDto.Model.Trim(),

                SerialNumber =
                    string.IsNullOrWhiteSpace(
                        accesorioDto.SerialNumber)
                        ? "NA"
                        : accesorioDto.SerialNumber.Trim(),

                Quantity =
                    accesorioDto.Quantity,

                PurchaseCost =
                    accesorioDto.PurchaseCost,

                Status =
                    accesorioDto.Status.Trim(),

                Location =
                    accesorioDto.Location.Trim(),

                Observation =
                    string.IsNullOrWhiteSpace(
                        accesorioDto.Observation)
                        ? null
                        : accesorioDto.Observation.Trim(),

                FechaRegistro =
                    DateTime.UtcNow
            };

            acta.Accesorios.Add(accesorio);
        }

        await _actaRepository.AddAsync(acta);
        await _actaRepository.SaveChangesAsync();

        return MapToDto(acta);
    }

    public async Task<ActaDto> MarcarComoFirmadaAsync(string id)
    {
        var acta = await _actaRepository.GetByIdAsync(id);

        if (acta is null)
        {
            throw new InvalidOperationException(
                "El acta no existe.");
        }

        acta.Estado = "FIRMADA";

        await _actaRepository.SaveChangesAsync();

        return MapToDto(acta);
    }

    private async Task<string> GenerateActaIdAsync(DateTime fecha)
    {
        var ultimoId =
            await _actaRepository
                .GetLastIdForDateAsync(fecha);

        var siguienteNumero = 1;

        if (!string.IsNullOrWhiteSpace(ultimoId))
        {
            var ultimaParte =
                ultimoId.Split('-').Last();

            if (int.TryParse(
                ultimaParte,
                out var ultimoNumero))
            {
                siguienteNumero =
                    ultimoNumero + 1;
            }
        }

        return $"ACT-{fecha:yyyyMMdd}-{siguienteNumero:000}";
    }

    private static ActaDto MapToDto(Acta acta)
    {
        return new ActaDto
        {
            Id = acta.Id,
            Fecha = acta.Fecha,
            Tipo = acta.Tipo,
            Estado = acta.Estado,
            SincronizadoSaludsa =
                acta.SincronizadoSaludsa,
            EstadoSincronizacion =
                acta.EstadoSincronizacion,
            TimestampSincronizacion =
                acta.TimestampSincronizacion,
            TienePagare =
                acta.TienePagare,
            ArchivoActa =
                acta.ArchivoActa,
            ArchivoPagare =
                acta.ArchivoPagare,

            Empleado = new EmpleadoDto
            {
                Id = acta.Empleado.Id,
                Username = acta.Empleado.Username,
                FullName = acta.Empleado.FullName,
                NationalId = acta.Empleado.NationalId,
                City = acta.Empleado.City
            },

            Activos = acta.Activos
                .Select(activo => new ActivoDto
                {
                    Id = activo.Id,
                    Manufacturer =
                        activo.Manufacturer,
                    Model =
                        activo.Model,
                    SerialNumber =
                        activo.SerialNumber,
                    Hostname =
                        activo.Hostname,
                    PurchaseCost =
                        activo.PurchaseCost,
                    Status =
                        activo.Status,
                    Location =
                        activo.Location,
                    Observation =
                        activo.Observation
                })
                .ToList(),

            Accesorios = acta.Accesorios
                .Select(accesorio => new AccesorioDto
                {
                    Id = accesorio.Id,
                    EquipmentType =
                        accesorio.EquipmentType,
                    Manufacturer =
                        accesorio.Manufacturer,
                    Model =
                        accesorio.Model,
                    SerialNumber =
                        accesorio.SerialNumber,
                    Quantity =
                        accesorio.Quantity,
                    PurchaseCost =
                        accesorio.PurchaseCost,
                    Status =
                        accesorio.Status,
                    Location =
                        accesorio.Location,
                    Observation =
                        accesorio.Observation
                })
                .ToList()
        };
    }
}