namespace SaludsaActas.Application.DTOs;

public class CreateAccesorioDto
{
    public string EquipmentType { get; set; } = string.Empty;

    public string Manufacturer { get; set; } = string.Empty;

    public string? Model { get; set; }

    public string? SerialNumber { get; set; }

    public int Quantity { get; set; }

    public decimal PurchaseCost { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string? Observation { get; set; }
}