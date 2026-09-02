using System.Globalization;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SaludsaActas.Application.DTOs;
using SaludsaActas.Application.Interfaces;
using SaludsaActas.Domain.Entities;
using SaludsaActas.Domain.Interfaces;

namespace SaludsaActas.Infrastructure.Documents;

public class DocumentService : IDocumentService
{
    private readonly IActaRepository _actaRepository;
    private readonly DocumentOptions _options;
    private readonly IHostEnvironment _environment;

    public DocumentService(
        IActaRepository actaRepository,
        IOptions<DocumentOptions> options,
        IHostEnvironment environment)
    {
        _actaRepository = actaRepository;
        _options = options.Value;
        _environment = environment;
    }

    public async Task<GeneratedDocumentDto> GenerateWordAsync(
        string actaId,
        string documentType)
    {
        var acta = await _actaRepository.GetByIdAsync(actaId);

        if (acta is null)
        {
            throw new InvalidOperationException(
                "El acta no existe.");
        }

        if (documentType.Equals(
                "acta",
                StringComparison.OrdinalIgnoreCase))
        {
            return GenerateActaWord(acta);
        }

        if (documentType.Equals(
                "pagare",
                StringComparison.OrdinalIgnoreCase))
        {
            return GeneratePagareWord(acta);
        }

        throw new NotSupportedException(
            "El tipo de documento solicitado no está soportado.");
    }

    private GeneratedDocumentDto GenerateActaWord(Acta acta)
    {
        var templatePath = Path.Combine(
            _environment.ContentRootPath,
            _options.TemplatesDirectory,
            "acta_template.docx");

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException(
                "No se encontró la plantilla acta_template.docx.",
                templatePath);
        }

        var equipos = BuildEquipmentList(acta);

        if (equipos.Count == 0)
        {
            throw new InvalidOperationException(
                "El acta no contiene equipos.");
        }

        var templateBytes =
            File.ReadAllBytes(templatePath);

        using var memoryStream =
            new MemoryStream();

        memoryStream.Write(
            templateBytes,
            0,
            templateBytes.Length);

        memoryStream.Position = 0;

        using (var document =
               WordprocessingDocument.Open(
                   memoryStream,
                   true))
        {
            var mainPart =
                document.MainDocumentPart
                ?? throw new InvalidOperationException(
                    "La plantilla Word no contiene un MainDocumentPart.");

            var mainDocument =
                mainPart.Document
                ?? throw new InvalidOperationException(
                    "La plantilla Word no contiene un documento principal.");

            var body =
                mainDocument.Body
                ?? throw new InvalidOperationException(
                    "La plantilla Word no contiene contenido.");

            ProcessEquipmentTable(
                body,
                equipos);

            var now = DateTime.Now;

            var legalRepresentativeName =
                GetLegalRepresentativeName();

            var legalRepresentativeId =
                GetLegalRepresentativeId();

            ReplacePlaceholder(
                body,
                "full_name",
                acta.Empleado.FullName);

            ReplacePlaceholder(
                body,
                "national_id",
                acta.Empleado.NationalId);

            ReplacePlaceholder(
                body,
                "city",
                FormatCity(acta.Empleado.City));

            ReplacePlaceholder(
                body,
                "actual_date",
                FormatDate(now));

            ReplacePlaceholder(
                body,
                "legal_representative_name",
                legalRepresentativeName);

            ReplacePlaceholder(
                body,
                "legal_representative_id",
                legalRepresentativeId);

            mainDocument.Save();
        }

        var generatedBytes =
            memoryStream.ToArray();

        var mainEquipment =
            equipos.First();

        var fileName =
            $"ENTREGA_" +
            $"{SanitizeFileName(acta.Empleado.Username)}_" +
            $"{SanitizeFileName(mainEquipment.EquipmentType)}_" +
            $"{SanitizeFileName(mainEquipment.SerialNumber)}.docx";

        SaveGeneratedDocument(
            fileName,
            generatedBytes);

        return BuildGeneratedDocument(
            generatedBytes,
            fileName);
    }

    private GeneratedDocumentDto GeneratePagareWord(
        Acta acta)
    {
        if (!acta.TienePagare ||
            acta.Activos.Count == 0)
        {
            throw new InvalidOperationException(
                "El acta no requiere pagaré porque no contiene una laptop.");
        }

        var templatePath = Path.Combine(
            _environment.ContentRootPath,
            _options.TemplatesDirectory,
            "pagare_template.docx");

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException(
                "No se encontró la plantilla pagare_template.docx.",
                templatePath);
        }

        var laptop =
            acta.Activos.First();

        var amount =
            Convert.ToInt32(
                decimal.Round(
                    laptop.PurchaseCost,
                    0,
                    MidpointRounding.AwayFromZero));

        var numericalAmount =
            amount.ToString(
                CultureInfo.InvariantCulture);

        var textAmount =
            SpanishNumberConverter
                .ConvertToWords(amount);

        var now =
            DateTime.Now;

        var legalRepresentativeName =
            GetLegalRepresentativeName();

        var legalRepresentativeId =
            GetLegalRepresentativeId();

        var templateBytes =
            File.ReadAllBytes(templatePath);

        using var memoryStream =
            new MemoryStream();

        memoryStream.Write(
            templateBytes,
            0,
            templateBytes.Length);

        memoryStream.Position = 0;

        using (var document =
               WordprocessingDocument.Open(
                   memoryStream,
                   true))
        {
            var mainPart =
                document.MainDocumentPart
                ?? throw new InvalidOperationException(
                    "La plantilla del pagaré no contiene un MainDocumentPart.");

            var mainDocument =
                mainPart.Document
                ?? throw new InvalidOperationException(
                    "La plantilla del pagaré no contiene un documento principal.");

            var body =
                mainDocument.Body
                ?? throw new InvalidOperationException(
                    "La plantilla del pagaré no contiene contenido.");

            ReplacePlaceholder(
                body,
                "full_name",
                acta.Empleado.FullName);

            ReplacePlaceholder(
                body,
                "national_id",
                acta.Empleado.NationalId);

            ReplacePlaceholder(
                body,
                "city",
                FormatCity(acta.Empleado.City));

            ReplacePlaceholder(
                body,
                "actual_date",
                FormatDate(now));

            ReplacePlaceholder(
                body,
                "actual_date_header",
                FormatPagareHeaderDate(now));

            ReplacePlaceholder(
                body,
                "numerical_amount",
                numericalAmount);

            ReplacePlaceholder(
                body,
                "text_amount",
                textAmount);

            ReplacePlaceholder(
                body,
                "legal_representative_name",
                legalRepresentativeName);

            ReplacePlaceholder(
                body,
                "legal_representative_id",
                legalRepresentativeId);

            mainDocument.Save();
        }

        var generatedBytes =
            memoryStream.ToArray();

        var fileName =
            $"PAGARE_" +
            $"{SanitizeFileName(acta.Empleado.Username)}_" +
            $"Laptop_" +
            $"{SanitizeFileName(laptop.SerialNumber)}.docx";

        SaveGeneratedDocument(
            fileName,
            generatedBytes);

        return BuildGeneratedDocument(
            generatedBytes,
            fileName);
    }

    private string GetLegalRepresentativeName()
    {
        return string.IsNullOrWhiteSpace(
            _options.LegalRepresentativeName)
            ? "[REPRESENTANTE LEGAL NO CONFIGURADO]"
            : _options.LegalRepresentativeName.Trim();
    }

    private string GetLegalRepresentativeId()
    {
        return string.IsNullOrWhiteSpace(
            _options.LegalRepresentativeId)
            ? "[CÉDULA NO CONFIGURADA]"
            : _options.LegalRepresentativeId.Trim();
    }

    private void SaveGeneratedDocument(
        string fileName,
        byte[] content)
    {
        var outputDirectory = Path.Combine(
            _environment.ContentRootPath,
            _options.OutputDirectory);

        Directory.CreateDirectory(
            outputDirectory);

        var outputPath = Path.Combine(
            outputDirectory,
            fileName);

        File.WriteAllBytes(
            outputPath,
            content);
    }

    private static GeneratedDocumentDto BuildGeneratedDocument(
        byte[] content,
        string fileName)
    {
        return new GeneratedDocumentDto
        {
            Content = content,
            FileName = fileName,
            ContentType =
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };
    }

    private static List<EquipmentTemplateData> BuildEquipmentList(
        Acta acta)
    {
        var result =
            new List<EquipmentTemplateData>();

        foreach (var activo in acta.Activos)
        {
            result.Add(
                new EquipmentTemplateData
                {
                    Quantity = "1",
                    EquipmentType = "Laptop",
                    Hostname = activo.Hostname,
                    Manufacturer = activo.Manufacturer,
                    Model = activo.Model,
                    SerialNumber = activo.SerialNumber,

                    PurchaseCost =
                        activo.PurchaseCost.ToString(
                            "0.00",
                            CultureInfo.InvariantCulture),

                    Status = activo.Status,

                    Observation =
                        activo.Observation
                        ?? string.Empty
                });
        }

        foreach (var accesorio in acta.Accesorios)
        {
            result.Add(
                new EquipmentTemplateData
                {
                    Quantity =
                        accesorio.Quantity.ToString(),

                    EquipmentType =
                        accesorio.EquipmentType,

                    Hostname =
                        string.Empty,

                    Manufacturer =
                        accesorio.Manufacturer,

                    Model =
                        accesorio.Model
                        ?? "NA",

                    SerialNumber =
                        accesorio.SerialNumber
                        ?? "NA",

                    PurchaseCost =
                        accesorio.PurchaseCost.ToString(
                            "0.00",
                            CultureInfo.InvariantCulture),

                    Status =
                        accesorio.Status,

                    Observation =
                        accesorio.Observation
                        ?? string.Empty
                });
        }

        return result;
    }

    private static void ProcessEquipmentTable(
        Body body,
        List<EquipmentTemplateData> equipos)
    {
        foreach (var table in body.Descendants<Table>())
        {
            var rows =
                table.Elements<TableRow>()
                    .ToList();

            var startRowIndex =
                rows.FindIndex(
                    row =>
                        GetElementText(row)
                            .Contains(
                                "for eq in equipos",
                                StringComparison.OrdinalIgnoreCase));

            if (startRowIndex < 0)
            {
                continue;
            }

            if (startRowIndex + 2 >= rows.Count)
            {
                throw new InvalidOperationException(
                    "La estructura de la tabla de equipos no es válida.");
            }

            var startRow =
                rows[startRowIndex];

            var templateRow =
                rows[startRowIndex + 1];

            var endRow =
                rows
                    .Skip(startRowIndex + 2)
                    .FirstOrDefault(
                        row =>
                            GetElementText(row)
                                .Contains(
                                    "endfor",
                                    StringComparison.OrdinalIgnoreCase));

            if (endRow is null)
            {
                throw new InvalidOperationException(
                    "No se encontró el cierre del ciclo de equipos en la plantilla.");
            }

            foreach (var equipo in equipos)
            {
                var newRow =
                    (TableRow)templateRow
                        .CloneNode(true);

                ReplacePlaceholder(
                    newRow,
                    "eq.quantity",
                    equipo.Quantity);

                ReplacePlaceholder(
                    newRow,
                    "eq.equipment_type",
                    equipo.EquipmentType);

                ReplacePlaceholder(
                    newRow,
                    "eq.hostname",
                    equipo.Hostname);

                ReplacePlaceholder(
                    newRow,
                    "eq.manufacturer",
                    equipo.Manufacturer);

                ReplacePlaceholder(
                    newRow,
                    "eq.model",
                    equipo.Model);

                ReplacePlaceholder(
                    newRow,
                    "eq.serial_number",
                    equipo.SerialNumber);

                ReplacePlaceholder(
                    newRow,
                    "eq.purchase_cost",
                    equipo.PurchaseCost);

                ReplacePlaceholder(
                    newRow,
                    "eq.status",
                    equipo.Status);

                ReplacePlaceholder(
                    newRow,
                    "eq.observation",
                    equipo.Observation);

                endRow.InsertBeforeSelf(
                    newRow);
            }

            startRow.Remove();
            templateRow.Remove();
            endRow.Remove();

            return;
        }

        throw new InvalidOperationException(
            "No se encontró la tabla de equipos en la plantilla Word.");
    }

    private static void ReplacePlaceholder(
        OpenXmlElement root,
        string key,
        string value)
    {
        var paragraphs =
            root.Descendants<Paragraph>()
                .ToList();

        foreach (var paragraph in paragraphs)
        {
            ReplacePlaceholderInParagraph(
                paragraph,
                key,
                value ?? string.Empty);
        }
    }

    private static void ReplacePlaceholderInParagraph(
        Paragraph paragraph,
        string key,
        string value)
    {
        var pattern =
            @"\{\{\s*" +
            Regex.Escape(key) +
            @"\s*\}\}";

        var regex =
            new Regex(
                pattern,
                RegexOptions.IgnoreCase);

        while (true)
        {
            var texts =
                paragraph
                    .Descendants<Text>()
                    .ToList();

            if (texts.Count == 0)
            {
                return;
            }

            var combined =
                string.Concat(
                    texts.Select(
                        text =>
                            text.Text
                            ?? string.Empty));

            var match =
                regex.Match(combined);

            if (!match.Success)
            {
                return;
            }

            var startPosition =
                match.Index;

            var endPosition =
                match.Index +
                match.Length;

            var startNodeIndex = -1;
            var endNodeIndex = -1;

            var startOffset = 0;
            var endOffset = 0;

            var currentPosition = 0;

            for (var i = 0; i < texts.Count; i++)
            {
                var currentText =
                    texts[i].Text
                    ?? string.Empty;

                var textLength =
                    currentText.Length;

                var nodeStart =
                    currentPosition;

                var nodeEnd =
                    currentPosition +
                    textLength;

                if (startNodeIndex < 0 &&
                    startPosition >= nodeStart &&
                    startPosition < nodeEnd)
                {
                    startNodeIndex = i;

                    startOffset =
                        startPosition -
                        nodeStart;
                }

                if (endPosition > nodeStart &&
                    endPosition <= nodeEnd)
                {
                    endNodeIndex = i;

                    endOffset =
                        endPosition -
                        nodeStart;

                    break;
                }

                currentPosition =
                    nodeEnd;
            }

            if (startNodeIndex < 0 ||
                endNodeIndex < 0)
            {
                return;
            }

            if (startNodeIndex == endNodeIndex)
            {
                var original =
                    texts[startNodeIndex].Text
                    ?? string.Empty;

                texts[startNodeIndex].Text =
                    original[..startOffset] +
                    value +
                    original[endOffset..];

                texts[startNodeIndex].Space =
                    SpaceProcessingModeValues.Preserve;
            }
            else
            {
                var startOriginal =
                    texts[startNodeIndex].Text
                    ?? string.Empty;

                var endOriginal =
                    texts[endNodeIndex].Text
                    ?? string.Empty;

                texts[startNodeIndex].Text =
                    startOriginal[..startOffset] +
                    value;

                texts[startNodeIndex].Space =
                    SpaceProcessingModeValues.Preserve;

                for (var i =
                         startNodeIndex + 1;
                     i < endNodeIndex;
                     i++)
                {
                    texts[i].Text =
                        string.Empty;
                }

                texts[endNodeIndex].Text =
                    endOriginal[endOffset..];

                texts[endNodeIndex].Space =
                    SpaceProcessingModeValues.Preserve;
            }
        }
    }

    private static string GetElementText(
        OpenXmlElement element)
    {
        return string.Concat(
            element
                .Descendants<Text>()
                .Select(
                    text =>
                        text.Text
                        ?? string.Empty));
    }

    private static string FormatDate(
        DateTime date)
    {
        var months = new[]
        {
            "enero",
            "febrero",
            "marzo",
            "abril",
            "mayo",
            "junio",
            "julio",
            "agosto",
            "septiembre",
            "octubre",
            "noviembre",
            "diciembre"
        };

        var month =
            months[date.Month - 1];

        var capitalizedMonth =
            char.ToUpper(month[0]) +
            month[1..];

        return
            $"{date.Day} de {capitalizedMonth} del {date.Year}";
    }

    private static string FormatPagareHeaderDate(
        DateTime date)
    {
        var months = new[]
        {
            "ENERO",
            "FEBRERO",
            "MARZO",
            "ABRIL",
            "MAYO",
            "JUNIO",
            "JULIO",
            "AGOSTO",
            "SEPTIEMBRE",
            "OCTUBRE",
            "NOVIEMBRE",
            "DICIEMBRE"
        };

        return
            $"{date.Day}, {months[date.Month - 1]}, {date.Year}";
    }

    private static string FormatCity(
        string? city)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return "Ubicación no especificada";
        }

        var value =
            city.Trim()
                .ToUpperInvariant();

        return value switch
        {
            "GYE" => "Guayaquil",
            "CUE" => "Cuenca",
            "UIO" => "Quito",
            "MAC" => "Machala",
            "MAN" => "Manta",
            _ => city.Trim()
        };
    }

    private static string SanitizeFileName(
        string value)
    {
        var invalidCharacters =
            Path.GetInvalidFileNameChars();

        var sanitized =
            new string(
                value
                    .Select(
                        character =>
                            invalidCharacters.Contains(character)
                                ? '_'
                                : character)
                    .ToArray());

        return sanitized.Trim();
    }

    private sealed class EquipmentTemplateData
    {
        public string Quantity { get; set; } =
            string.Empty;

        public string EquipmentType { get; set; } =
            string.Empty;

        public string Hostname { get; set; } =
            string.Empty;

        public string Manufacturer { get; set; } =
            string.Empty;

        public string Model { get; set; } =
            string.Empty;

        public string SerialNumber { get; set; } =
            string.Empty;

        public string PurchaseCost { get; set; } =
            string.Empty;

        public string Status { get; set; } =
            string.Empty;

        public string Observation { get; set; } =
            string.Empty;
    }
}