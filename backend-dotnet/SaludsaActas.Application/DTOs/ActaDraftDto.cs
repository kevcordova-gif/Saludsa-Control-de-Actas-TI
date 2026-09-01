namespace SaludsaActas.Application.DTOs;

public class ActaDraftDto
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string UsuarioJson { get; set; } = string.Empty;

    public string EquiposJson { get; set; } = string.Empty;

    public bool MarcarFirmada { get; set; }
}