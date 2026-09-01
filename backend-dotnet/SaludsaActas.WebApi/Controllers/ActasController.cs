using Microsoft.AspNetCore.Mvc;
using SaludsaActas.Application.Interfaces;

namespace SaludsaActas.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActasController : ControllerBase
{
    private readonly IActaService _actaService;

    public ActasController(IActaService actaService)
    {
        _actaService = actaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var actas = await _actaService.GetAllAsync();

        return Ok(actas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var acta = await _actaService.GetByIdAsync(id);

        if (acta is null)
        {
            return NotFound(new
            {
                message = "Acta no encontrada."
            });
        }

        return Ok(acta);
    }
}