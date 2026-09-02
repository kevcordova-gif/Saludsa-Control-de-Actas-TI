namespace SaludsaActas.Application.DTOs;

public class GeneratedDocumentDto
{
    public byte[] Content { get; set; } = Array.Empty<byte>();

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = "application/pdf";
}