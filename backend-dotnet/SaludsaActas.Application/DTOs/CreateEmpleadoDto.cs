namespace SaludsaActas.Application.DTOs;

public class CreateEmpleadoDto
{
    public string Username { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string NationalId { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;
}