using System.Text.Json;
using NumeralSystems.Net.Serialization;

namespace NumeralSystems.Net.Json;

/// <summary>Registers NumeralSystems.Net converters with System.Text.Json.</summary>
public static class NumeralJsonSerializerOptionsExtensions
{
    /// <summary>
    /// Adds the exact <see cref="Numeral"/> converter unless it is already registered.
    /// </summary>
    public static JsonSerializerOptions AddNumeralSystems(this JsonSerializerOptions options)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (options.Converters.All(converter => converter is not NumeralJsonConverter))
            options.Converters.Add(new NumeralJsonConverter());
        return options;
    }
}
