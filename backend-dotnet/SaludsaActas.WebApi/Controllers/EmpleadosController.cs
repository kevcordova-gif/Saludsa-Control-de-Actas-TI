using Microsoft.AspNetCore.Mvc;
using SaludsaActas.Application.Interfaces;

namespace SaludsaActas.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpleadosController : ControllerBase
{
    private readonly IEmpleadoService _empleadoService;

    public EmpleadosController(IEmpleadoService empleadoService)
    {
        _empleadoService = empleadoService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var empleado = await _empleadoService.GetByIdAsync(id);

        if (empleado is null)
        {
            return NotFound(new
            {
                message = "Empleado no encontrado."
            });
        }

        return Ok(empleado);
    }

    [HttpGet("username/{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {
        var empleado = await _empleadoService.GetByUsernameAsync(username);

        if (empleado is null)
        {
            return NotFound(new
            {
                message = "Empleado no encontrado."
            });
        }

        return Ok(empleado);
    }
}