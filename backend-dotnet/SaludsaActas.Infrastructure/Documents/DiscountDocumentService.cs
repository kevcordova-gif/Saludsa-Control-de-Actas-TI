using System.Globalization;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SaludsaActas.Application.DTOs;
using SaludsaActas.Application.Interfaces;

namespace SaludsaActas.Infrastructure.Documents;

public class DiscountDocumentService : IDiscountDocumentService
{
    private readonly IActiveDirectoryService _activeDirectoryService;
    private readonly IValidator<CreateDiscountDocumentDto> _validator;
    private readonly DocumentOptions _options;
    private readonly IHostEnvironment _environment;

    public DiscountDocumentService(
        IActiveDirectoryService activeDirectoryService,
        IValidator<CreateDiscountDocumentDto> validator,
        IOptions<DocumentOptions> options,
        IHostEnvironment environment)
    {
        _activeDirectoryService = activeDirectoryService;
        _validator = validator;
        _options = options.Value;
        _environment = environment;
    }

    public async Task<GeneratedDocumentDto> GenerateWordAsync(
        CreateDiscountDocumentDto dto)
    {
        var validationResult =
            await _validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(
                validationResult.Errors);
        }

        var employee =
            await _activeDirectoryService
                .GetByUsernameAsync(dto.Username.Trim());

        if (employee is null)
        {
            throw new InvalidOperationException(
                "El empleado no existe en Active Directory.");
        }

        var equipment = dto.Equipos.First();

        var templatePath = Path.Combine(
            _environment.ContentRootPath,
            _options.TemplatesDirectory,
            "descuento_template.docx");

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException(
                "No se encontró la plantilla descuento_template.docx.",
                templatePath);
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
                    "La plantilla de descuento no contiene un MainDocumentPart.");

            var mainDocument =
                mainPart.Document
                ?? throw new InvalidOperationException(
                    "La plantilla de descuento no contiene un documento principal.");

            var body =
                mainDocument.Body
                ?? throw new InvalidOperationException(
                    "La plantilla de descuento no contiene contenido.");

            var now = DateTime.Now;

            ReplacePlaceholder(
                body,
                "full_name",
                employee.FullName);

            ReplacePlaceholder(
                body,
                "national_id",
                employee.NationalId);

            ReplacePlaceholder(
                body,
                "discount_month",
                dto.DeductionMonth.Trim());

            ReplacePlaceholder(
                body,
                "actual_date_narrative",
                FormatNarrativeDate(now));

            ReplacePlaceholder(
                body,
                "text_amount",
                ConvertDecimalToWords(
                    equipment.PurchaseCost));

            ReplacePlaceholder(
                body,
                "eq.quantity",
                equipment.Quantity.ToString(
                    CultureInfo.InvariantCulture));

            ReplacePlaceholder(
                body,
                "eq.manufacturer",
                equipment.Manufacturer.Trim());

            ReplacePlaceholder(
                body,
                "eq.model",
                equipment.Model.Trim());

            ReplacePlaceholder(
                body,
                "eq.serial_number",
                equipment.SerialNumber.Trim());

            ReplacePlaceholder(
                body,
                "eq.purchase_cost",
                equipment.PurchaseCost.ToString(
                    "0.00",
                    CultureInfo.InvariantCulture));

            ReplacePlaceholder(
                body,
                "eq.equipment_type",
                equipment.EquipmentType.Trim());

            mainDocument.Save();
        }

        var generatedBytes =
            memoryStream.ToArray();

        var fileName =
            $"DESCUENTO_" +
            $"{SanitizeFileName(employee.Username)}_" +
            $"{SanitizeFileName(equipment.SerialNumber)}.docx";

        SaveGeneratedDocument(
            fileName,
            generatedBytes);

        return new GeneratedDocumentDto
        {
            Content = generatedBytes,
            FileName = fileName,
            ContentType =
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };
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

    private static string ConvertDecimalToWords(
        decimal amount)
    {
        var normalizedAmount =
            decimal.Round(
                amount,
                2,
                MidpointRounding.AwayFromZero);

        var integerPart =
            decimal.ToInt32(
                decimal.Truncate(normalizedAmount));

        var decimalPart =
            decimal.ToInt32(
                decimal.Round(
                    (normalizedAmount - integerPart) * 100,
                    0,
                    MidpointRounding.AwayFromZero));

        if (decimalPart == 100)
        {
            integerPart++;
            decimalPart = 0;
        }

        var words =
            SpanishNumberConverter
                .ConvertToWords(integerPart);

        return $"{words} CON {decimalPart:00}/100";
    }

    private static string FormatNarrativeDate(
        DateTime date)
    {
        var culture =
            CultureInfo.GetCultureInfo("es-EC");

        return date
            .ToString(
                "dddd, d 'de' MMMM 'del' yyyy",
                culture)
            .ToLower(culture);
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
                            text.Text ?? string.Empty));

            var match =
                regex.Match(combined);

            if (!match.Success)
            {
                return;
            }

            var startPosition =
                match.Index;

            var endPosition =
                match.Index + match.Length;

            var startNodeIndex = -1;
            var endNodeIndex = -1;

            var startOffset = 0;
            var endOffset = 0;

            var currentPosition = 0;

            for (var i = 0; i < texts.Count; i++)
            {
                var currentText =
                    texts[i].Text ?? string.Empty;

                var textLength =
                    currentText.Length;

                var nodeStart =
                    currentPosition;

                var nodeEnd =
                    currentPosition + textLength;

                if (startNodeIndex < 0 &&
                    startPosition >= nodeStart &&
                    startPosition < nodeEnd)
                {
                    startNodeIndex = i;

                    startOffset =
                        startPosition - nodeStart;
                }

                if (endPosition > nodeStart &&
                    endPosition <= nodeEnd)
                {
                    endNodeIndex = i;

                    endOffset =
                        endPosition - nodeStart;

                    break;
                }

                currentPosition = nodeEnd;
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

                for (var i = startNodeIndex + 1;
                     i < endNodeIndex;
                     i++)
                {
                    texts[i].Text = string.Empty;
                }

                texts[endNodeIndex].Text =
                    endOriginal[endOffset..];

                texts[endNodeIndex].Space =
                    SpaceProcessingModeValues.Preserve;
            }
        }
    }

    private static string SanitizeFileName(
        string value)
    {
        var invalidCharacters =
            Path.GetInvalidFileNameChars();

        return new string(
            value.Select(character =>
                    invalidCharacters.Contains(character)
                        ? '_'
                        : character)
                .ToArray())
            .Trim();
    }
}