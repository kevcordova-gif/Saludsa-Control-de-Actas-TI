using SaludsaActas.Application.DTOs;
using SaludsaActas.Application.Interfaces;
using SaludsaActas.Domain.Entities;
using SaludsaActas.Domain.Interfaces;

namespace SaludsaActas.Application.Services;

public class ActaDraftService : IActaDraftService
{
    private readonly IActaDraftRepository _actaDraftRepository;

    public ActaDraftService(IActaDraftRepository actaDraftRepository)
    {
        _actaDraftRepository = actaDraftRepository;
    }

    public async Task<List<ActaDraftDto>> GetAllAsync()
    {
        var drafts = await _actaDraftRepository.GetAllAsync();

        return drafts
            .Select(MapToDto)
            .ToList();
    }

    public async Task<ActaDraftDto?> GetByIdAsync(int id)
    {
        var draft = await _actaDraftRepository.GetByIdAsync(id);

        return draft is null
            ? null
            : MapToDto(draft);
    }

    public async Task<ActaDraftDto> CreateAsync(CreateActaDraftDto dto)
    {
        var now = DateTime.UtcNow;

        var draft = new ActaDraft
        {
            Titulo = dto.Titulo.Trim(),
            UsuarioJson = dto.UsuarioJson,
            EquiposJson = dto.EquiposJson,
            MarcarFirmada = dto.MarcarFirmada,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _actaDraftRepository.AddAsync(draft);
        await _actaDraftRepository.SaveChangesAsync();

        return MapToDto(draft);
    }

    public async Task DeleteAsync(int id)
    {
        var draft = await _actaDraftRepository.GetByIdAsync(id);

        if (draft is null)
        {
            throw new InvalidOperationException(
                "El borrador no existe.");
        }

        _actaDraftRepository.Delete(draft);

        await _actaDraftRepository.SaveChangesAsync();
    }

    private static ActaDraftDto MapToDto(ActaDraft draft)
    {
        return new ActaDraftDto
        {
            Id = draft.Id,
            CreatedAt = draft.CreatedAt,
            UpdatedAt = draft.UpdatedAt,
            Titulo = draft.Titulo,
            UsuarioJson = draft.UsuarioJson,
            EquiposJson = draft.EquiposJson,
            MarcarFirmada = draft.MarcarFirmada
        };
    }
}