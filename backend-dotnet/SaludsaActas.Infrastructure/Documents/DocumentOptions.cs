namespace SaludsaActas.Infrastructure.Documents;

public class DocumentOptions
{
    public string TemplatesDirectory { get; set; } = string.Empty;

    public string OutputDirectory { get; set; } = string.Empty;

    public string LibreOfficePath { get; set; } = string.Empty;
}