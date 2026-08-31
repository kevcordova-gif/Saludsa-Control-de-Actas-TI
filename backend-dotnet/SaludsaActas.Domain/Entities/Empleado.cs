namespace SaludsaActas.Domain.Entities;

public class Empleado
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string NationalId { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public ICollection<Acta> Actas { get; set; } = new List<Acta>();
}