using Microsoft.AspNetCore.Mvc;
using SaludsaActas.Application.DTOs;
using SaludsaActas.Application.Interfaces;

namespace SaludsaActas.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActaDraftsController : ControllerBase
{
    private readonly IActaDraftService _actaDraftService;

    public ActaDraftsController(IActaDraftService actaDraftService)
    {
        _actaDraftService = actaDraftService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var drafts = await _actaDraftService.GetAllAsync();

        return Ok(drafts);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var draft = await _actaDraftService.GetByIdAsync(id);

        if (draft is null)
        {
            return NotFound(new
            {
                message = "Borrador no encontrado."
            });
        }

        return Ok(draft);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateActaDraftDto dto)
    {
        var draft = await _actaDraftService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = draft.Id },
            draft
        );
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _actaDraftService.DeleteAsync(id);

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }
}