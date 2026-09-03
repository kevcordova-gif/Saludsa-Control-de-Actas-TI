using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SaludsaActas.Application.DTOs;
using SaludsaActas.Application.Interfaces;

namespace SaludsaActas.WebApi.Controllers;

[ApiController]
[Route("api/discounts")]
public class DiscountsController : ControllerBase
{
    private readonly IDiscountDocumentService _discountDocumentService;

    public DiscountsController(
        IDiscountDocumentService discountDocumentService)
    {
        _discountDocumentService = discountDocumentService;
    }

    [HttpPost("word")]
    public async Task<IActionResult> GenerateWord(
        [FromBody] CreateDiscountDocumentDto dto)
    {
        try
        {
            var document =
                await _discountDocumentService
                    .GenerateWordAsync(dto);

            return File(
                document.Content,
                document.ContentType,
                document.FileName);
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
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (FileNotFoundException)
        {
            return Problem(
                statusCode:
                    StatusCodes
                        .Status500InternalServerError,

                title:
                    "No se pudo generar el documento.",

                detail:
                    "La plantilla Word de descuento no está disponible.");
        }
        catch (Exception)
        {
            return Problem(
                statusCode:
                    StatusCodes
                        .Status500InternalServerError,

                title:
                    "No se pudo generar el documento de descuento.",

                detail:
                    "Ocurrió un error durante la generación del documento.");
        }
    }
}