namespace SaludsaActas.Infrastructure.ActiveDirectory;

public class ActiveDirectoryOptions
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 636;

    public bool UseSsl { get; set; } = true;

    public string BaseDn { get; set; } = string.Empty;

    public int SearchLimit { get; set; } = 15;
}