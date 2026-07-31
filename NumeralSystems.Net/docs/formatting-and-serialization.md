# Formatting, providers, Span, and JSON

[Documentation home](index.md) ·
[Numeral alphabets](numeral-alphabets.md) ·
[Text and binary encodings](string-encoding.md) ·
[Cookbook](cookbook.md) ·
[API reference](api-reference.md)

Version 4.8 integrates numeral text with standard .NET formatting and
serialization patterns while keeping alphabets explicit.

## `NumeralFormatInfo`

`NumeralFormatInfo` is an immutable `IFormatProvider` carrying:

- an ordered `NumeralAlphabet`;
- the token between individual digit symbols;
- the negative sign;
- the decimal separator.

Construction validates all symbol/token conflicts:

```csharp
using NumeralSystems.Net;

var format = new NumeralFormatInfo(
    NumeralAlphabet.Base16,
    digitSeparator: "|",
    negativeSign: "~",
    decimalSeparator: ",");

var hexadecimal = Numeral.System.OfBase(16);
hexadecimal.AdjustToFitIntegralLength = false;

var value = hexadecimal.Parse("~A|F,B", format);
Console.WriteLine(value.ToString("G", format)); // ~A|F,B
```

`NumeralFormatInfo.ForBase` and `ForAlphabet` copy sign and decimal tokens from
another provider:

```csharp
var italian = NumeralFormatInfo.ForBase(10, new CultureInfo("it-IT"));
var value = Numeral.System.OfBase(10).Parse("-12,5", italian);
```

Passing a culture directly is also supported. In that case the deterministic
default alphabet is selected for the numeral system's base.

## Standard formats

`Numeral` implements `IFormattable`.

| Format | Meaning |
| --- | --- |
| `G` or empty | General format using the supplied `IFormatProvider` |
| `R` | Stable invariant format using the deterministic default alphabet, no digit separator, `-`, and `.` |

```csharp
var general = value.ToString("G", italian);
var persistent = value.ToString("R", CultureInfo.InvariantCulture);
```

`R` guarantees a numeric round trip when parsed by a numeral system of the same
base with its default alphabet. Use JSON when leading zeros, trailing
fractional zeros, or the exact digit arrays must also be preserved.

Unsupported format strings throw `FormatException` instead of being silently
ignored.

## Structured parsing with a provider

Provider-based parsing has both throwing and structured forms:

```csharp
ParseResult result = hexadecimal.TryParse("~A|F,B", format);

if (!result.Success)
    Console.WriteLine($"{result.Reason} at {result.Position}: {result.Message}");
```

An alphabet whose size differs from the numeral-system base produces
`InvalidConfiguration`/`AlphabetSizeMismatch` diagnostics through the normal
`ParseResult` path.

## Span APIs on .NET 8

The .NET 8 target adds Span overloads without changing the .NET Standard 2.1
surface:

```csharp
Span<char> buffer = stackalloc char[64];
var value = Numeral.System.OfBase(16)[65535];

if (value.TryFormat(buffer, out var written, "R", null))
    Console.WriteLine(buffer[..written]); // FFFF
```

Ordered integer alphabets also support Span:

```csharp
Span<char> encoded = stackalloc char[128];

NumeralAlphabet.Base62.TryEncode(
    BigInteger.Pow(2, 256),
    encoded,
    out var charsWritten);

var decoded = NumeralAlphabet.Base62.Decode(encoded[..charsWritten]);
```

`NumeralSystem.TryParse(ReadOnlySpan<char>, ...)` preserves the same
`ParseResult` semantics and UTF-16 error positions as the string overload.

For byte encodings, `StandardBaseCodec.TryEncode` and `TryDecode` write into
caller-provided spans and return `false` when the destination is too small.
Malformed decode input also returns `false` from `TryDecode`.

## Optional JSON serialization on .NET 8

Install the separate package so applications that do not serialize numerals do
not carry JSON integration:

```console
dotnet add package NumeralSystems.Net.Json --version 5.1.0
```

Register the converter on the `JsonSerializerOptions` used by the application:

```csharp
using NumeralSystems.Net.Json;

var system = Numeral.System.OfBase(16);
system.AdjustToFitIntegralLength = false;

var value = new Numeral(
    system,
    integral: new List<int> { 0, 15 },
    fractional: new List<int> { 0, 1 },
    positive: false);

var options = new JsonSerializerOptions().AddNumeralSystems();
var json = JsonSerializer.Serialize(value, options);
var restored = JsonSerializer.Deserialize<Numeral>(json, options);
```

`AddNumeralSystems` is idempotent. The converter type remains available as
`NumeralSystems.Net.Serialization.NumeralJsonConverter` when a framework or
dependency-injection layer requires direct converter construction.

The JSON shape is intentionally alphabet- and culture-independent:

```json
{
  "base": 16,
  "positive": false,
  "numerator": "-3841",
  "denominator": "256",
  "integral": [0, 15],
  "fractional": [0, 1]
}
```

This preserves:

- the positional base;
- the sign;
- the normalized exact rational numerator and denominator without JSON number
  precision limits;
- leading integral zeros;
- fractional zeros and exact digit count;
- every digit without converting through `double` or `decimal`.

Deserialization rejects bases below 2, incomplete or invalid rational fields,
missing digit properties, non-integer digits, and digits outside
`0..base-1` with `JsonException`. The reader accepts 4.8 payloads without
`numerator` and `denominator`; in that case it derives the exact finite value
from the digit arrays. Empty digit arrays are preserved for compatibility with
a default-constructed `Numeral`.
An alphabet is not serialized because a `Numeral` stores numeric digit indices,
not a presentation alphabet. Transmit an application-specific alphabet
identifier separately when the wire protocol requires one.

## Target matrix

| Feature | .NET Standard 2.1 | .NET 8 |
| --- | --- | --- |
| Ordered numeral alphabets | Yes | Yes |
| UTF-16 character APIs | Yes | Yes |
| RFC Base16/Base32/Base64 | Yes | Yes |
| Stream/reader/writer APIs | Yes | Yes |
| `IFormatProvider` and `IFormattable` | Yes | Yes |
| Rune APIs | No | Yes |
| Span APIs | No | Yes |
| `NumeralSystems.Net.Json` package | No | Yes |

The core package contains both target-framework assemblies; NuGet selects the
most specific compatible implementation automatically. JSON support is a
separate .NET 8 package.
