# Text, numeral, and binary encodings

[Documentation home](index.md) ·
[Numeral alphabets](numeral-alphabets.md) ·
[Formatting and JSON](formatting-and-serialization.md) ·
[Cookbook](cookbook.md) ·
[API reference](api-reference.md)

Version 4.8 separates three operations that previously looked similar but have
different data models and interoperability guarantees.

| Operation | Input model | API | Interoperable standard |
| --- | --- | --- | --- |
| Represent a number | Numeric digits and an ordered alphabet | `NumeralAlphabet`, `Numeral`, `NumeralSystem` | Application-defined |
| Encode bytes as text | Arbitrary bytes | `StandardBaseCodec` | RFC 4648 Base16/Base32/Base64 |
| Transform characters into radix digits | UTF-16 units or Unicode scalars | `CharacterRadixTransform` | No; experimental |

Do not use one layer as a substitute for another. In particular,
`NumeralAlphabet.Base64` is a 64-symbol numeral alphabet; it does not implement
the byte grouping, padding, or wire format of RFC Base64.

## Standard Base16, Base32, and Base64

`StandardBaseCodec` operates on bytes:

```csharp
using System.Text;
using NumeralSystems.Net.Encoding;

var bytes = Encoding.UTF8.GetBytes("Hello, 🌍");

var hex = StandardBaseCodec.EncodeBase16(bytes);
var base32 = StandardBaseCodec.EncodeBase32(bytes);
var base64 = StandardBaseCodec.EncodeBase64(bytes);

var original = Encoding.UTF8.GetString(
    StandardBaseCodec.DecodeBase64(base64));
```

The encodings are deliberately specific:

- Base16 writes uppercase `0-9A-F` and accepts uppercase or lowercase input;
- Base32 uses the RFC 4648 `A-Z2-7` alphabet, not the Crockford-style numeral
  alphabet exposed by `NumeralAlphabet.Base32`;
- Base64 uses the RFC 4648 `A-Z`, `a-z`, `0-9`, `+`, `/` alphabet;
- Base32 and Base64 encode with padding by default and decode either padded or
  unpadded input;
- decoding ignores Unicode whitespace but rejects invalid symbols, padding,
  final-block lengths, and non-zero unused bits;
- empty byte sequences encode to empty text and decode back to zero bytes.

Padding can be omitted explicitly:

```csharp
var compact32 = StandardBaseCodec.EncodeBase32(bytes, includePadding: false);
var compact64 = StandardBaseCodec.EncodeBase64(bytes, includePadding: false);
```

## Stream large inputs

The stream APIs keep a small fixed buffer and preserve partial bit groups
between reads. They do not materialize the complete input or output:

```csharp
await using var input = File.OpenRead("archive.bin");
await using var outputStream = File.Create("archive.b64");
using var writer = new StreamWriter(outputStream, Encoding.ASCII);

StandardBaseCodec.Encode(
    input,
    writer,
    StandardBaseEncoding.Base64);

writer.Flush();
```

Decoding uses a `TextReader` and a writable `Stream`:

```csharp
using var reader = File.OpenText("archive.b64");
await using var decoded = File.Create("archive.bin");

StandardBaseCodec.Decode(
    reader,
    decoded,
    StandardBaseEncoding.Base64);
```

The caller owns every stream, reader, and writer; codec methods leave them
open. `bufferSize` is configurable and must be positive.

## UTF-16 code-unit transformation

`CharacterRadixTransform.EncodeUtf16` preserves the historical behavior: each
.NET `char` is treated as a numeric value and expanded to a fixed-width digit
sequence. Digit values are stored directly in UTF-16 code units.

```csharp
var encoded = CharacterRadixTransform.EncodeUtf16(
    "Hello",
    destinationBase: 64,
    digitsPerCodeUnit: out var width);

var decoded = CharacterRadixTransform.DecodeUtf16(
    encoded,
    sourceBase: 64,
    digitsPerCodeUnit: width);
```

This result is not normal printable text. It may contain nulls, controls,
unpaired surrogates, or characters changed by a transport. Store the base and
width with the transformed value.

Defined empty behavior:

```csharp
var encoded = CharacterRadixTransform.EncodeUtf16("", 16, out var width);
// encoded == "", width == 0

var decoded = CharacterRadixTransform.DecodeUtf16("", 16, 0);
// decoded == ""
```

## Rune transformation on .NET 8

The Rune API operates on Unicode scalar values. A supplementary character such
as `😀` is one value instead of two UTF-16 surrogate code units:

```csharp
var encoded = CharacterRadixTransform.EncodeRunes(
    "A😀𝄞",
    destinationBase: 256,
    digitsPerRune: out var width);

var decoded = CharacterRadixTransform.DecodeRunes(
    encoded,
    sourceBase: 256,
    digitsPerRune: width);
```

Unpaired surrogates are rejected with an exception. Rune digit bases are
limited to 55,296 so every emitted digit is itself a valid Unicode scalar.
Normal bases such as 2, 16, 32, 64, or 256 are unaffected by that limit.

Streaming UTF-16 and Rune overloads accept a caller-selected width. A fixed
width is required because a one-pass stream cannot first inspect the complete
input to find its maximum value.

## Extract distinct text units

Use `CharacterIdentity` instead of the old misleading `Encoding.String` name:

```csharp
var codeUnits = CharacterIdentity.GetUtf16CodeUnits("😀😀"); // two surrogates
var runes = CharacterIdentity.GetRunes("😀😀");             // one scalar
```

Both methods preserve first-occurrence order and return read-only lists. The
Rune member is available on .NET 8 and validates surrogate pairing.

## Convert text units to `Value`

These APIs are explicit about their unit:

```csharp
var utf16 = Value.FromUtf16String("A😀", fit: true);
var runeValue = Value.FromRunes("A😀", fit: true); // .NET 8

Console.WriteLine(utf16.ToUtf16String()); // A😀
Console.WriteLine(runeValue.ToRuneString()); // A😀
```

`fit: true` selects `maxDigit + 1`, with a minimum base of 2. Without fitting,
UTF-16 uses base 65,536 and Rune values use base 1,114,112.

The old `Value.FromString(string, bool)`, `Encoding.String`, and static
`Type.Base.String` transformation members remain as obsolete forwarding APIs.
They are retained for compatibility, not recommended for new code.

## Smallest-base contract

For positional digits, every digit must satisfy `0 <= digit < base`.
Therefore the smallest valid base is one greater than the maximum digit:

```csharp
CharacterRadixTransform.GetSmallestBaseUtf16("A"); // 66, because 'A' is 65
CharacterRadixTransform.GetSmallestBaseRunes("😀"); // 128513
CharacterRadixTransform.GetSmallestBaseUtf16("");  // 2
```

The historical `Type.Base.String.GetSmallestBase` now follows this corrected
contract while forwarding to `GetSmallestBaseUtf16`.

## Modern Span overloads

On .NET 8, `StandardBaseCodec` supports `ReadOnlySpan<byte>`, `Span<char>`,
`ReadOnlySpan<char>`, and `Span<byte>`. Numeral parsing and formatting expose
matching Span entry points. See [Formatting and JSON](formatting-and-serialization.md)
for examples and target-specific availability.
