namespace SaludsaActas.Application.DTOs;

public class CreateDiscountDocumentDto
{
    public string Username { get; set; } = string.Empty;

    public string DeductionMonth { get; set; } = string.Empty;

    public List<DiscountEquipmentDto> Equipos { get; set; } = new();
}