namespace SaludsaActas.Domain.Entities;

public class Accesorio
{
    public int Id { get; set; }

    public string EquipmentType { get; set; } = string.Empty;

    public string Manufacturer { get; set; } = string.Empty;

    public string? Model { get; set; }

    public string? SerialNumber { get; set; } = "NA";

    public int Quantity { get; set; }

    public decimal PurchaseCost { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string? Observation { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public ICollection<Acta> Actas { get; set; } = new List<Acta>();
}