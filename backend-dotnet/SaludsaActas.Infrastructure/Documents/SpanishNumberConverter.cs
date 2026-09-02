namespace SaludsaActas.Infrastructure.Documents;

public static class SpanishNumberConverter
{
    public static string ConvertToWords(int number)
    {
        if (number == 0)
        {
            return "CERO";
        }

        if (number < 0)
        {
            return $"MENOS {ConvertToWords(Math.Abs(number))}";
        }

        return ConvertNumber(number)
            .Trim()
            .ToUpperInvariant();
    }

    private static string ConvertNumber(int number)
    {
        if (number == 0)
        {
            return string.Empty;
        }

        if (number < 10)
        {
            return number switch
            {
                1 => "uno",
                2 => "dos",
                3 => "tres",
                4 => "cuatro",
                5 => "cinco",
                6 => "seis",
                7 => "siete",
                8 => "ocho",
                9 => "nueve",
                _ => string.Empty
            };
        }

        if (number < 20)
        {
            return number switch
            {
                10 => "diez",
                11 => "once",
                12 => "doce",
                13 => "trece",
                14 => "catorce",
                15 => "quince",
                16 => "dieciséis",
                17 => "diecisiete",
                18 => "dieciocho",
                19 => "diecinueve",
                _ => string.Empty
            };
        }

        if (number < 30)
        {
            return number switch
            {
                20 => "veinte",
                21 => "veintiuno",
                22 => "veintidós",
                23 => "veintitrés",
                24 => "veinticuatro",
                25 => "veinticinco",
                26 => "veintiséis",
                27 => "veintisiete",
                28 => "veintiocho",
                29 => "veintinueve",
                _ => string.Empty
            };
        }

        if (number < 100)
        {
            var tens = number / 10;
            var units = number % 10;

            var tensText = tens switch
            {
                3 => "treinta",
                4 => "cuarenta",
                5 => "cincuenta",
                6 => "sesenta",
                7 => "setenta",
                8 => "ochenta",
                9 => "noventa",
                _ => string.Empty
            };

            return units == 0
                ? tensText
                : $"{tensText} y {ConvertNumber(units)}";
        }

        if (number == 100)
        {
            return "cien";
        }

        if (number < 1000)
        {
            var hundreds = number / 100;
            var remainder = number % 100;

            var hundredsText = hundreds switch
            {
                1 => "ciento",
                2 => "doscientos",
                3 => "trescientos",
                4 => "cuatrocientos",
                5 => "quinientos",
                6 => "seiscientos",
                7 => "setecientos",
                8 => "ochocientos",
                9 => "novecientos",
                _ => string.Empty
            };

            return remainder == 0
                ? hundredsText
                : $"{hundredsText} {ConvertNumber(remainder)}";
        }

        if (number < 2000)
        {
            var remainder = number - 1000;

            return remainder == 0
                ? "mil"
                : $"mil {ConvertNumber(remainder)}";
        }

        if (number < 1_000_000)
        {
            var thousands = number / 1000;
            var remainder = number % 1000;

            var thousandsText =
                $"{ConvertNumber(thousands)} mil";

            return remainder == 0
                ? thousandsText
                : $"{thousandsText} {ConvertNumber(remainder)}";
        }

        if (number < 2_000_000)
        {
            var remainder = number - 1_000_000;

            return remainder == 0
                ? "un millón"
                : $"un millón {ConvertNumber(remainder)}";
        }

        if (number < 1_000_000_000)
        {
            var millions = number / 1_000_000;
            var remainder = number % 1_000_000;

            var millionsText =
                $"{ConvertNumber(millions)} millones";

            return remainder == 0
                ? millionsText
                : $"{millionsText} {ConvertNumber(remainder)}";
        }

        throw new ArgumentOutOfRangeException(
            nameof(number),
            "El valor es demasiado grande para convertirlo a texto.");
    }
}