namespace SaludsaActas.Domain.Entities;

public class ActaDraft
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string Titulo { get; set; } = string.Empty;

    public string UsuarioJson { get; set; } = string.Empty;

    public string EquiposJson { get; set; } = string.Empty;

    public bool MarcarFirmada { get; set; } = false;
}