# Migrating to 4.8.0

[Documentation home](index.md) ·
[Text and binary encodings](string-encoding.md) ·
[Formatting and JSON](formatting-and-serialization.md) ·
[Troubleshooting](troubleshooting.md) ·
[Changelog](../../CHANGELOG.md)

Version 4.8 keeps the 4.7 numeral and bit-pattern APIs while making text-unit
semantics explicit. The main migration decision is whether existing code is
representing a number, encoding bytes, or experimentally transforming .NET
characters.

## Choose the correct API family

| Existing intent | 4.8 API |
| --- | --- |
| Format or parse a positional number | `NumeralAlphabet`, `NumeralSystem`, `Numeral` |
| Implement interoperable Base16/Base32/Base64 | `StandardBaseCodec` |
| Preserve the historical per-`char` transformation | `CharacterRadixTransform.EncodeUtf16` / `DecodeUtf16` |
| Process Unicode scalar values | `CharacterRadixTransform.EncodeRunes` / `DecodeRunes` on .NET 8 |
| Find distinct text units | `CharacterIdentity.GetUtf16CodeUnits` / `GetRunes` |

## Replace ambiguous string APIs

Before:

```csharp
var encoded = Type.Base.String.EncodeToBase(text, 64, out var width);
var decoded = Type.Base.String.DecodeFromBase(encoded, 64, width);
```

After, when historical UTF-16 behavior is intended:

```csharp
var encoded = CharacterRadixTransform.EncodeUtf16(text, 64, out var width);
var decoded = CharacterRadixTransform.DecodeUtf16(encoded, 64, width);
```

After, when standard Base64 is intended:

```csharp
var encoded = StandardBaseCodec.EncodeBase64(Encoding.UTF8.GetBytes(text));
var decoded = Encoding.UTF8.GetString(StandardBaseCodec.DecodeBase64(encoded));
```

These ambiguous methods were deprecated in 4.8.0 and removed in 4.8.1. Migrate
before updating when an application still calls them.

## Smallest-base behavior changed

`Type.Base.String.GetSmallestBase` previously returned the maximum UTF-16 code
unit itself. A positional base must be greater than every digit. Its explicit
replacement returns `maximum + 1`:

```csharp
CharacterRadixTransform.GetSmallestBaseUtf16(text)
```

For example, the smallest base for `"A"` is 66, not 65. Empty input returns 2.
Code that previously incremented the old result must remove that manual
increment.

## Empty strings

The following are now explicitly valid and empty:

- UTF-16/Rune character transformation, with an inferred width of zero;
- standard Base16/Base32/Base64 encoding and decoding;
- `Value.FromUtf16String`, `Value.FromRunes`, and
  `Value.FromString(value, NumeralAlphabet)`.

An empty string is still not a valid textual numeral for
`NumeralSystem.Parse`; it returns `ParseErrorReason.EmptyInput`. This distinction
prevents a missing number from being confused with zero.

## UTF-16 fitted and default bases

`Value.FromUtf16String(text, fit: false)` now uses base 65,536 rather than
65,535. The previous base could not contain digit 65,535 (`U+FFFF`) because
digits must be strictly smaller than their base.

`Value.FromString(string, bool)` was removed in 4.8.1. Use
`FromUtf16String` or `FromRunes` to state the unit.

## Formatting providers

Manual alphabet/token calls remain valid. New code can group them in one
immutable provider:

```csharp
var provider = new NumeralFormatInfo(
    NumeralAlphabet.Base16,
    digitSeparator: "",
    negativeSign: "-",
    decimalSeparator: ".");

var text = numeral.ToString("G", provider);
var restored = numeral.Base.Parse(text, provider);
```

Use `R` for a stable invariant numeric round trip with the deterministic
default alphabet.

## JSON

On .NET 8, `JsonSerializer.Serialize(numeral)` now uses the built-in converter.
The JSON representation is structured and exact; it is not the output of
`ToString()` and does not include a presentation alphabet.

> **5.1 note:** built-in registration described in this historical guide was
> removed in 5.1. Install `NumeralSystems.Net.Json` and call
> `JsonSerializerOptions.AddNumeralSystems()` when upgrading beyond 5.0.

If an older application serialized public properties incidentally, treat the
new `{ base, positive, integral, fractional }` object as a versioned wire-format
change and migrate stored payloads explicitly.

## Multi-targeting

The NuGet package now contains `netstandard2.1` and `net8.0` assemblies. Rune,
Span, and built-in `System.Text.Json` integration are in the .NET 8 asset.
Portable consumers retain UTF-16, streaming, standard codecs, providers, and
all existing numeral/bit APIs through .NET Standard 2.1.
