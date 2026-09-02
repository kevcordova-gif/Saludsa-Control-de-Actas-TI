using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaludsaActas.Application.DTOs;
using SaludsaActas.Application.Interfaces;

namespace SaludsaActas.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActasController : ControllerBase
{
    private readonly IActaService _actaService;
    private readonly IDocumentService _documentService;

    public ActasController(
        IActaService actaService,
        IDocumentService documentService)
    {
        _actaService = actaService;
        _documentService = documentService;
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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateActaDto dto)
    {
        try
        {
            var acta = await _actaService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = acta.Id },
                acta
            );
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                message = "Los datos enviados no son válidos.",
                errors = ex.Errors.Select(error => new
                {
                    field = error.PropertyName,
                    message = error.ErrorMessage
                })
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPatch("{id}/firmar")]
    public async Task<IActionResult> MarcarComoFirmada(string id)
    {
        try
        {
            var acta = await _actaService.MarcarComoFirmadaAsync(id);

            return Ok(acta);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("{id}/documents/{documentType}/word")]
    public async Task<IActionResult> DownloadWord(
        string id,
        string documentType)
    {
        try
        {
            var document =
                await _documentService.GenerateWordAsync(
                    id,
                    documentType);

            return File(
                document.Content,
                document.ContentType,
                document.FileName);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (FileNotFoundException)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "No se pudo generar el documento.",
                detail: "La plantilla Word requerida no está disponible.");
        }
    }
}