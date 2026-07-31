using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NumeralSystems.Net.Serialization;

/// <summary>
/// Serializes a <see cref="Numeral"/> as its base, sign, exact rational value,
/// and digit arrays independently of culture and presentation alphabets.
/// </summary>
public sealed class NumeralJsonConverter : JsonConverter<Numeral>
{
    /// <inheritdoc />
    public override Numeral Read(
        ref Utf8JsonReader reader,
        System.Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
            throw new JsonException("A numeral must be a JSON object.");

        if (!root.TryGetProperty("base", out var baseProperty) ||
            !baseProperty.TryGetInt32(out var baseValue) ||
            baseValue < 2)
            throw new JsonException("The numeral base must be an integer greater than or equal to 2.");
        if (!root.TryGetProperty("positive", out var positiveProperty) ||
            positiveProperty.ValueKind is not (JsonValueKind.True or JsonValueKind.False))
            throw new JsonException("The numeral positive property must be a boolean.");

        var integral = ReadDigits(root, "integral", baseValue);
        var fractional = ReadDigits(root, "fractional", baseValue);
        var exactValue = ReadExactValue(root);
        var positive = positiveProperty.GetBoolean();

        if (exactValue is not null && !exactValue.IsZero && (exactValue.Sign > 0) != positive)
            throw new JsonException("The exact rational sign must agree with the positive property.");

        var system = Numeral.System.OfBase(baseValue);
        system.AdjustToFitIntegralLength = false;
        return Numeral.FromRepresentation(
            system,
            integral,
            fractional,
            positive,
            exactValue);
    }

    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer,
        Numeral value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("base", value.Base.Size);
        writer.WriteBoolean("positive", value.Positive);
        writer.WriteString(
            "numerator",
            value.ExactValue.Numerator.ToString(CultureInfo.InvariantCulture));
        writer.WriteString(
            "denominator",
            value.ExactValue.Denominator.ToString(CultureInfo.InvariantCulture));
        WriteDigits(writer, "integral", value.IntegralIndices);
        WriteDigits(writer, "fractional", value.FractionalIndices);
        writer.WriteEndObject();
    }

    private static RationalValue? ReadExactValue(JsonElement root)
    {
        var hasNumerator = root.TryGetProperty("numerator", out var numeratorProperty);
        var hasDenominator = root.TryGetProperty("denominator", out var denominatorProperty);
        if (hasNumerator != hasDenominator)
            throw new JsonException("The exact numerator and denominator must be provided together.");
        if (!hasNumerator) return null;

        if (numeratorProperty.ValueKind != JsonValueKind.String ||
            denominatorProperty.ValueKind != JsonValueKind.String ||
            !BigInteger.TryParse(
                numeratorProperty.GetString(),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var numerator) ||
            !BigInteger.TryParse(
                denominatorProperty.GetString(),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var denominator) ||
            denominator <= 0)
            throw new JsonException("The exact rational value is invalid.");

        return new RationalValue(numerator, denominator);
    }

    private static List<int> ReadDigits(
        JsonElement root,
        string propertyName,
        int baseValue)
    {
        if (!root.TryGetProperty(propertyName, out var property) ||
            property.ValueKind != JsonValueKind.Array)
            throw new JsonException($"The numeral {propertyName} property must be an array.");

        var result = new List<int>();
        foreach (var element in property.EnumerateArray())
        {
            if (!element.TryGetInt32(out var digit) || digit < 0 || digit >= baseValue)
                throw new JsonException(
                    $"Every {propertyName} digit must be between 0 and {baseValue - 1}.");
            result.Add(digit);
        }
        return result;
    }

    private static void WriteDigits(
        Utf8JsonWriter writer,
        string propertyName,
        IEnumerable<int> digits)
    {
        writer.WritePropertyName(propertyName);
        writer.WriteStartArray();
        foreach (var digit in digits)
            writer.WriteNumberValue(digit);
        writer.WriteEndArray();
    }
}
