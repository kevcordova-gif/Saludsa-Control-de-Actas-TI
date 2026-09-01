namespace SaludsaActas.Application.DTOs;

public class ActaDto
{
    public string Id { get; set; } = string.Empty;

    public DateTime Fecha { get; set; }

    public string Tipo { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public bool SincronizadoSaludsa { get; set; }

    public string? EstadoSincronizacion { get; set; }

    public DateTime? TimestampSincronizacion { get; set; }

    public bool TienePagare { get; set; }

    public string? ArchivoActa { get; set; }

    public string? ArchivoPagare { get; set; }

    public EmpleadoDto Empleado { get; set; } = new();

    public List<ActivoDto> Activos { get; set; } = new();

    public List<AccesorioDto> Accesorios { get; set; } = new();
}