namespace SaludsaActas.Domain.Entities;

public class Acta
{
    public string Id { get; set; } = string.Empty;

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public string Tipo { get; set; } = "DOTACION";

    public string Estado { get; set; } = "PENDIENTE_FIRMA";

    public bool SincronizadoSaludsa { get; set; } = false;

    public string? EstadoSincronizacion { get; set; }

    public DateTime? TimestampSincronizacion { get; set; }

    public int EmpleadoId { get; set; }

    public bool TienePagare { get; set; } = false;

    public string? ArchivoActa { get; set; }

    public string? ArchivoPagare { get; set; }

    public Empleado Empleado { get; set; } = null!;

    public ICollection<Activo> Activos { get; set; } = new List<Activo>();

    public ICollection<Accesorio> Accesorios { get; set; } = new List<Accesorio>();
}