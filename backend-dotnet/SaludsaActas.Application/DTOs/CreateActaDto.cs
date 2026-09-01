namespace SaludsaActas.Application.DTOs;

public class CreateActaDto
{
    public int EmpleadoId { get; set; }

    public string Tipo { get; set; } = "Dotacion";

    public List<CreateActivoDto> Activos { get; set; } = new();

    public List<CreateAccesorioDto> Accesorios { get; set; } = new();
}