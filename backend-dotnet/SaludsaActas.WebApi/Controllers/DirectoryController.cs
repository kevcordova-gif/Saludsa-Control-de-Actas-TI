using Microsoft.AspNetCore.Mvc;
using SaludsaActas.Application.Interfaces;

namespace SaludsaActas.WebApi.Controllers;

[ApiController]
[Route("api/directory")]
public class DirectoryController : ControllerBase
{
    private readonly IActiveDirectoryService _activeDirectoryService;
    private readonly ILogger<DirectoryController> _logger;

    public DirectoryController(
        IActiveDirectoryService activeDirectoryService,
        ILogger<DirectoryController> logger)
    {
        _activeDirectoryService = activeDirectoryService;
        _logger = logger;
    }

    [HttpGet("employees")]
    public async Task<IActionResult> SearchEmployees(
        [FromQuery] string search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return BadRequest(new
            {
                message = "Debe ingresar un término de búsqueda."
            });
        }

        try
        {
            var employees =
                await _activeDirectoryService
                    .SearchEmployeesAsync(search);

            return Ok(employees);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(
                ex,
                "Error de configuración al consultar Active Directory.");

            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Configuración de Active Directory incompleta.",
                detail: "No se pudo completar la consulta al directorio corporativo.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al consultar Active Directory.");

            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "No se pudo consultar Active Directory.",
                detail: "Ocurrió un error al consultar el directorio corporativo.");
        }
    }

    [HttpGet("employees/{username}")]
    public async Task<IActionResult> GetEmployeeByUsername(
        string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest(new
            {
                message = "Debe indicar un username."
            });
        }

        try
        {
            var employee =
                await _activeDirectoryService
                    .GetByUsernameAsync(username);

            if (employee is null)
            {
                return NotFound(new
                {
                    message = "Empleado no encontrado en Active Directory."
                });
            }

            return Ok(employee);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(
                ex,
                "Error de configuración al consultar Active Directory.");

            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Configuración de Active Directory incompleta.",
                detail: "No se pudo completar la consulta al directorio corporativo.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al consultar Active Directory.");

            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "No se pudo consultar Active Directory.",
                detail: "Ocurrió un error al consultar el directorio corporativo.");
        }
    }
}