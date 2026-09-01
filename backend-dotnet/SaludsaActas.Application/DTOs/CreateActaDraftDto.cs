namespace SaludsaActas.Application.DTOs;

public class CreateActaDraftDto
{
    public string Titulo { get; set; } = string.Empty;

    public string UsuarioJson { get; set; } = string.Empty;

    public string EquiposJson { get; set; } = string.Empty;

    public bool MarcarFirmada { get; set; }
}