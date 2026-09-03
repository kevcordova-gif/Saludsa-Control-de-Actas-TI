using SaludsaActas.Application.DTOs;

namespace SaludsaActas.Application.Interfaces;

public interface IDiscountDocumentService
{
    Task<GeneratedDocumentDto> GenerateWordAsync(
        CreateDiscountDocumentDto dto);
}