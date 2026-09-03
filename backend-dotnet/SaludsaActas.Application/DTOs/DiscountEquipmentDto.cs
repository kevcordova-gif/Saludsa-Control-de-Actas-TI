namespace SaludsaActas.Application.DTOs;

public class DiscountEquipmentDto
{
    public string EquipmentType { get; set; } = string.Empty;

    public string Manufacturer { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    public int Quantity { get; set; } = 1;

    public decimal PurchaseCost { get; set; }
}