using SaludsaActas.Application.DTOs;

namespace SaludsaActas.Application.Interfaces;

public interface IDocumentService
{
    Task<GeneratedDocumentDto> GenerateWordAsync(
        string actaId,
        string documentType);
}