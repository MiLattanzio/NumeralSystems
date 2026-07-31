# Ordered numeral alphabets and exact round trips

[Documentation home](index.md) ·
[Numeral systems](numeral-systems.md) ·
[Getting started](getting-started.md) ·
[Cookbook](cookbook.md) ·
[API reference](api-reference.md)

`NumeralAlphabet` is the ordered, immutable mapping between digit indices and
text symbols. Symbol position is numeric meaning:

```text
alphabet[0] = zero
alphabet[1] = one
...
alphabet[base - 1] = largest digit
```

Using a sequence instead of a set makes encoding reproducible and makes the
round-trip contract explicit:

```csharp
BigInteger value = BigInteger.Pow(2, 256) + 12345;

var text = NumeralAlphabet.Base58.Encode(value);
var decoded = NumeralAlphabet.Base58.Decode(text);

Console.WriteLine(decoded == value); // True
```

`Encode` and `Decode` accept signed `BigInteger` values. They preserve the exact
integer value and never convert through `double`, `decimal`, `long`, or another
bounded primitive.

## Immutability and comparison

The constructor copies its input. `Symbols` is exposed as
`IReadOnlyList<string>`, the indexer is read-only, and lookups use ordinal,
case-sensitive comparison.

```csharp
var alphabet = new NumeralAlphabet(new[] { "zero", "one", "two" });

Console.WriteLine(alphabet.Count);          // 3
Console.WriteLine(alphabet[2]);             // two
Console.WriteLine(alphabet.IndexOf("one")); // 1
Console.WriteLine(alphabet.IndexOf("ONE")); // -1
```

Changing the original collection after construction does not affect the
alphabet.

## Validation rules

Construction rejects:

- fewer than two symbols;
- null or empty symbols;
- duplicate symbols;
- a symbol that is a prefix of another symbol.

Prefix rejection makes tokenization deterministic even without a digit
separator:

```csharp
// Rejected because "a" could be digit 0 or the first part of digit 1.
var invalid = new NumeralAlphabet(new[] { "a", "ab" });
```

`ValidateFormat(separator, negativeSign, decimalSeparator)` additionally rejects
formatting tokens that:

- contain a digit symbol;
- are contained in a digit symbol;
- contain or are contained in another formatting token;
- are null or empty when a sign or decimal separator is required.

```csharp
NumeralAlphabet.Base16.ValidateFormat(
    separator: "",
    negativeSign: "-",
    numberDecimalSeparator: ".");
```

The validation happens automatically in the `NumeralSystem` and `Numeral`
overloads that accept `NumeralAlphabet`.

## Predefined alphabets

| Property | Base | Ordered symbols / convention |
| --- | ---: | --- |
| `Base2` | 2 | `01` |
| `Base8` | 8 | `01234567` |
| `Base10` | 10 | `0123456789` |
| `Base16` | 16 | uppercase hexadecimal |
| `Base32` | 32 | Crockford-style, excluding I, L, O, and U |
| `Base36` | 36 | digits followed by uppercase letters |
| `Base58` | 58 | Bitcoin Base58 |
| `Base62` | 62 | digits, uppercase letters, lowercase letters |
| `Base64` | 64 | RFC 4648 alphabet without `=` padding |

Retrieve one by base:

```csharp
var hexadecimal = NumeralAlphabet.ForBase(16);

if (NumeralAlphabet.TryForBase(58, out var base58))
{
    // ...
}
```

`PredefinedBases` contains the ordered list `2, 8, 10, 16, 32, 36, 58, 62,
64`.

Base64 here is an integer radix alphabet. It does not add RFC 4648 byte
grouping or `=` padding; use `StandardBaseCodec.EncodeBase64` when the
standardized byte transport is required. Likewise, this Base32 alphabet is
Crockford-style while the standard byte codec uses RFC 4648 `A-Z2-7`.

## Deterministic alphabets for other bases

`CreateDefault(base)` returns a predefined alphabet when one exists. For every
other positional base it creates fixed-width decimal symbols:

```csharp
var base12 = NumeralAlphabet.CreateDefault(12);

Console.WriteLine(base12[0]);  // 00
Console.WriteLine(base12[11]); // 11
```

All symbols have the same width, so concatenated text remains unambiguous. This
also gives `Numeral.ToString()` and `NumeralSystem.Parse(string)` a deterministic
default for arbitrary supported bases.

## Encoding and decoding integers

```csharp
var alphabet = NumeralAlphabet.Base36;

var encoded = alphabet.Encode(-123456789);
var decoded = alphabet.Decode(encoded);

Console.WriteLine(encoded);
Console.WriteLine(decoded == -123456789); // True
```

An optional separator is useful for human-readable custom alphabets:

```csharp
var words = new NumeralAlphabet(new[] { "zero", "one", "two" });
var encoded = words.Encode(5, separator: "|"); // one|two
var decoded = words.Decode(encoded, separator: "|");
```

`TryDecode` has overloads that report an error position without throwing.
`Decode` throws `FormatException` for invalid text.

## Parsing a Numeral with diagnostics

The new `NumeralSystem.TryParse` overload returns `ParseResult`:

```csharp
var hexadecimal = Numeral.System.OfBase(16);
var parsed = hexadecimal.TryParse("1G", NumeralAlphabet.Base16);

Console.WriteLine(parsed.Success);  // False
Console.WriteLine(parsed.Reason);   // UnknownSymbol
Console.WriteLine(parsed.Position); // 1
Console.WriteLine(parsed.Message);
```

Positions are zero-based UTF-16 offsets, which can be used directly with normal
.NET string indexing.

`ParseResult` exposes:

| Property | Meaning |
| --- | --- |
| `Success` | whether parsing completed |
| `Value` | parsed `Numeral`, or `null` on failure |
| `Position` | error position, or `-1` on success |
| `ErrorLength` | number of UTF-16 code units associated with the error |
| `Reason` | machine-readable `ParseErrorReason` |
| `Message` | human-readable explanation |

`ParseErrorReason` distinguishes:

- null and empty input;
- alphabet/base size mismatch;
- invalid separator/sign configuration;
- unknown symbols and missing digits;
- unexpected or missing digit separators;
- misplaced negative signs;
- repeated decimal separators.

The throwing `Parse(value, alphabet, ...)` overload includes the reason and
position in its `InvalidOperationException`.

## Formatting and parsing Numeral values

```csharp
var system = Numeral.System.OfBase(62);
system.AdjustToFitIntegralLength = false;

var numeral = system[BigInteger.Parse("123456789012345678901234567890")];
var text = numeral.ToString(NumeralAlphabet.Base62);
var roundTrip = system.Parse(text, NumeralAlphabet.Base62);

Console.WriteLine(roundTrip.BigInteger == numeral.BigInteger); // True
```

The alphabet size must equal `NumeralSystem.Size`. This prevents accidentally
parsing base-16 text with a base-10 mapping.

`SerializationInfo.OfBase` now exposes both:

- `Alphabet`, the preferred immutable representation;
- `Identity`, the legacy mutable list for source compatibility.

If legacy code modifies `Identity`, parsing and formatting continue to honor
that modified list. New code should set and retain `Alphabet`.

## Migrating from HashSet

This overload is deprecated:

```csharp
Value.FromString(text, HashSet<string> symbols)
```

`HashSet<T>` describes membership, not numeric order. The legacy overload is
marked `Obsolete` and sorts symbols ordinally only to make its remaining
behavior deterministic.

Replace it with:

```csharp
var alphabet = new NumeralAlphabet(orderedSymbols);
var value = Value.FromString(text, alphabet);
var roundTrip = value.ToString(alphabet);
```

Do not construct `orderedSymbols` from a set. Keep the application/domain order
in a list, array, immutable collection, or static `NumeralAlphabet`.

## Round-trip guarantees

For a valid alphabet and formatting configuration:

```text
value == alphabet.Decode(alphabet.Encode(value))
```

This holds for signed `BigInteger` values, every predefined alphabet, and
deterministically generated alphabets. Converting an integral `Numeral` between
two systems and then formatting/parsing with the destination alphabet also
preserves its exact `BigInteger` value.

Fractional conversion has a separate precision contract: a fraction that
repeats in the destination base may require a bounded, inexact expansion. See
[Arithmetic](arithmetic.md) and [Numeral systems](numeral-systems.md).

## Test coverage

The repository verifies:

- generated signed values across every base from 2 through 128;
- pairwise conversion among bases 2, 8, 10, 16, 32, 36, 58, 62, and 64;
- values larger than 256 bits;
- leading-zero preservation in `Value`;
- multi-character fixed-width alphabets;
- every structured parsing error reason;
- immutability, ordinal lookup, duplicates, prefixes, and token conflicts.
