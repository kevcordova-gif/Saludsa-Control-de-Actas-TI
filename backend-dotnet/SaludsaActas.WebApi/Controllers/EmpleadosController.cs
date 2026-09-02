using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaludsaActas.Application.DTOs;
using SaludsaActas.Application.Interfaces;

namespace SaludsaActas.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmpleadosController : ControllerBase
{
    private readonly IEmpleadoService _empleadoService;

    public EmpleadosController(
        IEmpleadoService empleadoService)
    {
        _empleadoService = empleadoService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var empleado =
            await _empleadoService.GetByIdAsync(id);

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
    public async Task<IActionResult> GetByUsername(
        string username)
    {
        var empleado =
            await _empleadoService
                .GetByUsernameAsync(username);

        if (empleado is null)
        {
            return NotFound(new
            {
                message = "Empleado no encontrado."
            });
        }

        return Ok(empleado);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateEmpleadoDto dto)
    {
        try
        {
            var empleado =
                await _empleadoService
                    .CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = empleado.Id },
                empleado);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                message =
                    "Los datos enviados no son válidos.",

                errors =
                    ex.Errors.Select(error => new
                    {
                        field =
                            error.PropertyName,

                        message =
                            error.ErrorMessage
                    })
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("sync-ad/{username}")]
    public async Task<IActionResult> SyncFromActiveDirectory(
        string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest(new
            {
                message =
                    "Debe indicar un username."
            });
        }

        try
        {
            var empleado =
                await _empleadoService
                    .SyncFromActiveDirectoryAsync(
                        username);

            return Ok(new
            {
                message =
                    "Empleado sincronizado correctamente.",

                empleado
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (Exception)
        {
            return Problem(
                statusCode:
                    StatusCodes
                        .Status500InternalServerError,

                title:
                    "No se pudo sincronizar el empleado.",

                detail:
                    "Ocurrió un error al consultar o guardar los datos del empleado.");
        }
    }
}