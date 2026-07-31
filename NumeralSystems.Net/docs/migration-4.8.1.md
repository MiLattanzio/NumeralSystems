# Migrating to 4.8.1

[Documentation home](index.md) ·
[Text and binary encodings](string-encoding.md) ·
[Numeral alphabets](numeral-alphabets.md) ·
[API reference](api-reference.md) ·
[Changelog](../../CHANGELOG.md)

Version 4.8.1 removes every API that was marked obsolete in 4.8.0. Update call
sites to the explicit replacement before changing the package version.

## Removed API and replacement

| Removed | Replacement |
| --- | --- |
| `Value.FromString(string, HashSet<string>)` | Build `NumeralAlphabet` from an ordered sequence, then call `Value.FromString(string, NumeralAlphabet)` |
| `Value.FromString(string, bool)` | `Value.FromUtf16String` or .NET 8 `Value.FromRunes` |
| `NumeralSystems.Net.Encoding.String.GetIdentity` | `CharacterIdentity.GetUtf16CodeUnits` or .NET 8 `GetRunes` |
| `Type.Base.String.EncodeToBase` | `CharacterRadixTransform.EncodeUtf16` |
| `Type.Base.String.DecodeFromBase` | `CharacterRadixTransform.DecodeUtf16` |
| `Type.Base.String.ToIndicesOfBase` | `Value.FromUtf16String` or `CharacterRadixTransform` |
| `Type.Base.String.FromIndicesOfBase` | `Value.ToUtf16String` or `CharacterRadixTransform` |
| `Type.Base.String.GetSmallestBase` | `CharacterRadixTransform.GetSmallestBaseUtf16` |

`Type.Base.String` itself remains available as the mutable
`IList<Type.Base.Char>` wrapper. Only its unrelated static transformation
members were removed.

## Ordered alphabets

Do not recover order from a set:

```csharp
var alphabet = new NumeralAlphabet(orderedSymbols);
var value = Value.FromString(text, alphabet);
```

The position of each symbol is its numeric digit. Store that order in a list,
array, immutable collection, or reusable `NumeralAlphabet`.

## Explicit character units

For UTF-16 code units:

```csharp
var value = Value.FromUtf16String(text, fit: true);
var restored = value.ToUtf16String();
```

On .NET 8, use Rune members when a supplementary character must contribute one
Unicode scalar rather than two surrogate code units:

```csharp
var value = Value.FromRunes(text, fit: true);
var restored = value.ToRuneString();
```

For the fixed-width experimental radix transformation, call
`CharacterRadixTransform` directly. For RFC Base16/Base32/Base64 byte formats,
use `StandardBaseCodec` instead.

## Package metadata

The NuGet author is now `Mi Lattanzio`. The private project contact
for security and conduct reports is [mi@polecola.it](mailto:mi@polecola.it).

## Verification checklist

1. Search for the removed signatures before upgrading.
2. Choose ordered alphabet, standard byte codec, UTF-16, or Rune APIs based on
   the actual data model.
3. Build with warnings treated as errors.
4. Run round-trip tests for persisted identifiers and text transforms.
5. Inspect JSON or wire payloads independently from presentation alphabets.
