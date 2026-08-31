namespace SaludsaActas.Domain.Entities;

public class Activo
{
    public int Id { get; set; }

    public string Manufacturer { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    public string Hostname { get; set; } = string.Empty;

    public decimal PurchaseCost { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string? Observation { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public ICollection<Acta> Actas { get; set; } = new List<Acta>();
}