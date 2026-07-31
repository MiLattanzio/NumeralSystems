# API reference

[Exact rational values](exact-rationals.md)

[Documentation home](index.md) ·
[Getting started](getting-started.md) ·
[Numeral systems](numeral-systems.md) ·
[Numeral alphabets](numeral-alphabets.md) ·
[Formatting and JSON](formatting-and-serialization.md) ·
[Arithmetic](arithmetic.md) ·
[Cookbook](cookbook.md) ·
[Bitwise values](bitwise-values.md) ·
[BitPattern engine](bit-patterns.md) ·
[Incomplete values](incomplete-values.md)

This Markdown reference catalogs the public surface by namespace and member
family. Signatures shown here use abbreviated type names; consult the source for
generic constraints and overload-level details.

## `NumeralSystems.Net`

### `Value`

Represents a non-negative integral value as digit indices in one base.

| Member | Description |
| --- | --- |
| `Value(List<int> indices, int baseValue)` | Creates a value after validating the base and digits |
| `IReadOnlyList<int> Indices` | Digits from most significant to least significant |
| `int Base` | Source base |
| `FromString(string, NumeralAlphabet, separator)` | Maps ordered symbols to indices deterministically |
| `FromUtf16String(string, bool fit = false)` | Uses UTF-16 code units as digit indices |
| `FromRunes(string, bool fit = false)` | Uses Unicode scalar values as digits on .NET 8 |
| `FromBigInteger(BigInteger, int baseValue = 10)` | Creates a digit sequence without an integer-size limit |
| `ToBigInteger()` | Returns the complete non-negative integer value |
| `ToString(NumeralAlphabet, separator)` | Formats stored digits with an ordered alphabet |
| `ToUtf16String()` / `ToRuneString()` | Reconstructs explicitly selected text units |
| `ToBase(int, bool removeFirstZeros = false)` | Returns the same integral value in another base |

`Value` does not represent a sign or fractional digits.

### `RationalValue`

Immutable normalized exact value backed by a signed `BigInteger` numerator and
a positive `BigInteger` denominator.

| Member family | Members |
| --- | --- |
| State | `Numerator`, `Denominator`, `IsZero`, `IsInteger`, `Sign`, `Zero`, `One` |
| Construction | constructor, `FromInteger`, `FromDecimal`, `FromSingle`, `FromDouble`, `FromDigits` |
| Conversion | `Expand`, `Truncate`, `ToDecimal`, `ToString` |
| Arithmetic | `Add`, `Subtract`, `Multiply`, `Divide`, `Negate`, `Abs`; `+`, `-`, `*`, `/` |
| Equality/order | `Equals`, `CompareTo`, `==`, `!=` |

Construction reduces the fraction, makes the denominator positive, and
normalizes zero to `0/1`. IEEE factories preserve the exact finite bit value.

### `NumeralConversionOptions`

Immutable policy for rational-to-positional expansion.

| Member | Purpose |
| --- | --- |
| `MaxFractionalDigits` | Bounds generated digits and period-detection state |
| `RoundingMode` | Selects one of six directed or nearest rounding rules |
| `DetectRepeatingPeriod` | Enables remainder-cycle detection |
| `InfiniteBehavior` | `Throw`, `Truncate`, `Round`, or `PreservePeriod` |
| `Default` | Exact-first 128-digit period-preserving policy |
| `Legacy` | 4.x-compatible 128-digit truncation policy |
| `With*` | Creates a modified options instance |

### `NumeralExpansion`

Immutable digits and metadata produced by `RationalValue.Expand` or
`NumeralValue.Expand`. State includes `Value`, `Base`, `IntegralDigits`,
`FractionalDigits`, `Negative`, `IsTerminating`, `IsExact`, `WasRounded`,
`RepeatingStartIndex`, `RepeatingLength`, and `HasRepeatingPeriod`. The alphabet
formatter encloses a preserved period in parentheses by default.

### `NumeralValue`

Sealed immutable exact rational value with a positional digit projection.

| Member family | Members |
| --- | --- |
| Exact state | `ExactValue`, `Numerator`, `Denominator`, `IsZero` |
| Projection | `Integral`, `Decimals`, `Negative`, `Base`, `IsExactRepresentation`, `WasRounded` |
| Period | `RepeatingStartIndex`, `RepeatingLength`, `HasRepeatingPeriod` |
| Construction | `FromDigits`, `FromRational`, `FromDecimal`, `FromBigInteger`, `FromInt`, `FromFloat`, `FromDouble`, `FromValue` |
| Primitive conversion | `ToBigInteger`, `ToDecimal`, `ToInt`, `ToFloat`, `ToDouble`, `ToValue` |
| Base conversion | `Expand`, `ToBase(int, NumeralConversionOptions)` |
| Arithmetic | `Add`, `Subtract`, `Multiply`, `Divide`, `Negate`, `Abs` |
| Comparison | `CompareTo`, `NumericallyEquals` |
| Operators | binary `+`, `-`, `*`, `/`; unary `-`; `<`, `>`, `<=`, `>=` |

The list constructor and 4.x base-conversion overloads remain with migration
warnings. Compatibility arithmetic overloads keep `out bool exact` to report
whether the digit projection terminated, while the returned value retains its
exact rational state. Operators use the left operand's base.

### `NumeralSystem`

Defines a base and the rules used to construct, parse, and format `Numeral`
instances.

| Member family | Members |
| --- | --- |
| Configuration | `Size`, `Length`, `SkipUnknownValues`, `AdjustToFitIntegralLength` |
| Parsing | `Parse`, structured and Boolean `TryParse`, provider and .NET 8 Span overloads, `TrySplitNumberIndices` |
| Formatting | `TryFromIndices`, including `NumeralAlphabet` overloads |
| Validation/conversion | `Contains`, `TryBigIntegerOf`, `TryIntegerOf`, `TryCharOf` |
| Numeric indexers | `BigInteger`, `int`, `double`, `decimal`, `long`, `ulong`, `uint`, `short`, `ushort`, `sbyte`, `byte` |
| Sequence indexers | `IEnumerable<byte>`, `IEnumerable<char>`, `IEnumerable<int>`, `IList<byte>`, `IList<char>`, `List<int>` |

`NumeralSystem.SerializationInfo` holds the preferred `Alphabet`, legacy
`Identity`, `Separator`, `NegativeSign`, and `NumberDecimalSeparator`.
`OfBase(int)` combines a deterministic alphabet with culture-aware sign and
decimal tokens.

### `Numeral`

Stores one value in a `NumeralSystem`.

| Member family | Members |
| --- | --- |
| State | `Positive`, `Base`, copied `IntegralIndices`/`FractionalIndices`, `ExactValue` |
| Text components | `GetIntegralStrings`, `GetIntegralString`, `GetFractionalStrings`, `GetFractionalString` |
| Primitive views | `BigInteger`, `Integer`, `Char`, `Double`, `Decimal`, `Float`, `Bytes` |
| Immutable creation | `FromRational`, `FromRepresentation`, `WithExactValue` |
| Conversion | `To(NumeralSystem, NumeralConversionOptions)` |
| Formatting | `ToString()`, alphabet/identity overloads, `IFormattable.ToString(G/R, provider)`, .NET 8 `TryFormat` |

`Numeral` also provides `NumeralAlphabet` overloads for digit access and
formatting, plus `ToString(SerializationInfo)`. State and primitive views are
read-only; create a replacement value when the exact value or representation
must change.

### `NumeralAlphabet`

An ordered immutable `IReadOnlyList<string>` with ordinal symbol lookup.

| Member family | Members |
| --- | --- |
| Construction | constructor from `IEnumerable<string>`, `CreateDefault` |
| Presets | `Base2`, `Base8`, `Base10`, `Base16`, `Base32`, `Base36`, `Base58`, `Base62`, `Base64` |
| Preset lookup | `PredefinedBases`, `ForBase`, `TryForBase` |
| Symbols | `Count`, indexer, `Symbols`, `IndexOf`, `Contains` |
| Validation | `ValidateFormat` |
| Integer codec | `Encode`, `Decode`, `TryDecode` |
| Modern target | .NET 8 Span-based `TryEncode`, `Decode`, and `TryDecode` |

Construction rejects empty, duplicate, and prefix-ambiguous symbols. Formatting
validation rejects conflicts among symbols, separators, and signs.

### `ParseResult` and `ParseErrorReason`

The structured `NumeralSystem.TryParse` overload returns `ParseResult`:

| Property | Purpose |
| --- | --- |
| `Success` | Reports whether parsing succeeded |
| `Value` | Parsed `Numeral`, or `null` |
| `Position` | Zero-based UTF-16 error position, or `-1` |
| `ErrorLength` | Length of the offending range |
| `Reason` | Machine-readable `ParseErrorReason` |
| `Message` | Human-readable diagnostic |

Reasons distinguish null/empty input, alphabet size/configuration problems,
unknown symbols, missing digits or separators, unexpected separators,
misplaced signs, and repeated decimal separators.

### `NumeralFormatInfo`

Immutable `IFormatProvider` carrying an alphabet, digit separator, negative
sign, and decimal separator. `ForBase` and `ForAlphabet` combine deterministic
alphabets with tokens from another culture/provider. `Numeral` supports `G`
(provider-driven) and `R` (invariant round-trip) standard formats.

`Numeral.System.OfBase(int)` is the convenience factory. The nested
`Numeral.System.Characters` type exposes:

- `Numbers`, `UpperLetters`, `LowerLetters`, and `Symbols`;
- `Alphanumeric`, `AlphanumericUpper`, `AlphanumericLower`, and
  `AlphanumericSymbols`;
- `Printable`, `NotPrintable`, `All`, and `WhiteSpaces`;
- `Point`, `Comma`, `Minus`, and `Semicolon`.

## `NumeralSystems.Net.Encoding`

### `StandardBaseCodec`

RFC-compatible byte encoding, separate from numeral alphabets:

| Member family | Members |
| --- | --- |
| In-memory | `Encode`, `Decode`, `EncodeBase16/32/64`, `DecodeBase16/32/64` |
| Selection | `StandardBaseEncoding.Base16`, `Base32`, `Base64` |
| Streaming | `Encode(Stream, TextWriter, ...)`, `Decode(TextReader, Stream, ...)` |
| .NET 8 Span | `Encode(ReadOnlySpan<byte>)`, `TryEncode`, `Decode(ReadOnlySpan<char>)`, `TryDecode` |

Base32 uses RFC 4648 `A-Z2-7`; Base64 uses the standard `+/` alphabet.
Decoding accepts padded or unpadded input and validates final unused bits.

### `CharacterRadixTransform` and `CharacterIdentity`

`CharacterRadixTransform` exposes explicit UTF-16 code-unit operations on all
targets and Unicode Rune operations on .NET 8. Both variants support in-memory
and reader/writer streaming. `GetSmallestBaseUtf16` and
`GetSmallestBaseRunes` return `maxDigit + 1`, with a minimum of 2.

`CharacterIdentity.GetUtf16CodeUnits` and the .NET 8 `GetRunes` member return
distinct units in first-occurrence order.

## `NumeralSystems.Net.Json` package (.NET 8)

`AddNumeralSystems(JsonSerializerOptions)` registers
`NumeralSystems.Net.Serialization.NumeralJsonConverter`. The converter writes
`base`, `positive`, `numerator`, `denominator`, `integral`, and `fractional`.
Exact integers are strings to avoid JSON precision limits. The reader remains
compatible with 4.8 digit-only JSON. The core assembly has no converter
attribute and does not register JSON behavior implicitly.

## `NumeralSystems.Net.Type.Base`

### Numeric wrappers

| Type | Underlying value | Incomplete counterpart |
| --- | --- | --- |
| `Byte` | `byte` | `IncompleteByte` |
| `Char` | `char` | `IncompleteChar` |
| `Short` | `short` | `IncompleteShort` |
| `UShort` | `ushort` | `IncompleteUShort` |
| `Int` | `int` | `IncompleteInt` |
| `UInt` | `uint` | `IncompleteUInt` |
| `Long` | `long` | `IncompleteLong` |
| `ULong` | `ulong` | `IncompleteULong` |
| `Float` | `float` | `IncompleteFloat` |
| `Double` | `double` | `IncompleteDouble` |
| `Decimal` | `decimal` | `IncompleteDecimal` |

Numeric wrappers share these member families:

| Member | Purpose |
| --- | --- |
| `Value` | Get or set the underlying primitive |
| `Bytes` | Get or set its byte representation |
| `Binary` | Get or set the fixed-width, least-significant-bit-first bit array |
| `BitLength` | Return the primitive width |
| `this[index]` | Access one bit |
| `Not`, `And`, `Or`, `Xor`, `Nand` | Return a new logical result |
| `ReverseAnd`, `ReverseOr`, `ReverseXor`, `ReverseNand` | Solve for possible left operands |
| `Incomplete` | Convert to the corresponding fully known incomplete value |
| `ToString()` | Format the binary representation |
| `ToString(format)` | Delegate formatting to the underlying primitive |

Overloads accept either a complete wrapper or the matching incomplete type.
The wrappers derive reverse XOR/NAND behavior and immutable `Pattern` snapshots
from `CompleteBitPattern<TSelf, TIncomplete>`.

### Base-conversion helpers

The following partial classes expose static conversion methods:

| Type | Methods |
| --- | --- |
| `BigInteger` | `FromIndicesOfBase`, `ToIndicesOfBase` |
| `Decimal` | `FromIndicesOfBase`, `ToIndicesOfBase` (including a precision-aware overload), `From(double)` |
| `Double` | `FromIndicesOfBase`, `ToIndicesOfBase` |
| `Float` | `FromIndicesOfBase`, `ToIndicesOfBase` |
| `UInt` | `FromIndicesOfBase`, `ToIndicesOfBase` |
| `ULong` | `FromIndicesOfBase`, `ToIndicesOfBase` |

### `String`

The instance API implements `IList<Type.Base.Char>` and includes the normal
mutable collection members plus `ToString()` and `ToString(format)`. Character
identity and radix transformation belong to the explicit APIs described in
[String encoding](string-encoding.md).

## `NumeralSystems.Net.Type.Incomplete`

### `BitPattern`

The immutable shared engine uses least-significant-bit-first `bool?` values and
implements `IReadOnlyList<bool?>`.

| Member family | Members |
| --- | --- |
| Construction | constructors from `IEnumerable<bool?>` or `IEnumerable<bool>`, `Parse`, `TryParse`, `FromUnsigned`, `Unknown` |
| Shape | `Count`, indexer, `ToArray`, `ToString` |
| Cardinality | `UnknownBitCount`, `CandidateCount` |
| Bounds | `MinValue`, `MaxValue`, `SignedMinValue`, `SignedMaxValue` |
| Membership | `IsMatch`, `IsSignedMatch` |
| Enumeration | `EnumerateCandidates`, `EnumerateBitArrays` |
| Logic | `Not`, `And`, `ApplyMask`, `Or`, `Xor`, `Nand` |
| Compatibility | `IsCompatibleWith`, `TryIntersect`, `Intersect` |
| Reverse logic | `TryReverseAnd`, `TryReverseOr`, `TryReverseXor`, `ReverseXor`, `TryReverseNand`, `ReverseNand` |
| Constraints | `TrySolveAnd`, `SolveAnd` |
| Shifts | `ShiftLeft`, `LogicalShiftLeft`, `ArithmeticShiftLeft`, `LogicalShiftRight`, `ArithmeticShiftRight` |
| Rotations | `RotateLeft`, `RotateRight` |

Candidate counts, encoded bounds, candidate values, and limits use
`System.Numerics.BigInteger`. See the [BitPattern guide](bit-patterns.md) for
set semantics and examples.

### Incomplete primitive wrappers

Incomplete primitive types store `bool?[] Binary`. Most expose:

| Member | Purpose |
| --- | --- |
| `Binary` | Ternary bit array: zero, one, or unknown |
| `IsComplete` | Whether every bit is known |
| `Permutations` | Candidate count implied by unknown bits |
| `Pattern` | Immutable `BitPattern` snapshot |
| `UnknownBitCount` / `CandidateCount` | Safe cardinality metadata |
| `MinValue` / `MaxValue` | Unsigned encoded bounds |
| `SignedMinValue` / `SignedMaxValue` | Two's-complement bounds |
| `IsMatch` / `IsSignedMatch` | Test encoded candidate membership |
| `EnumerateCandidates(limit)` | Lazily enumerate at most the requested number |
| `this[index]` | Materialize one candidate |
| `Enumerable` | Enumerate all candidates |
| `ByteArray` / `ToByteArray` | Split into incomplete bytes |
| `Contains` | Test complete or incomplete compatibility |
| `Not`, `And`, `Or`, `Xor`, `Nand`, `ApplyMask` | Three-valued logical operations |
| `ReverseAnd`, `ReverseOr`, `ReverseXor`, `ReverseNand` | Solve a logical equation when possible |
| `IsCompatibleWith`, `TryIntersect`, `Intersect` | Compare and combine constraints |
| shift and rotate methods | Transform patterns while retaining their width |
| `TrySolveAnd` | Solve `x AND mask == result` |
| `ToString(missingSeparator)` | Format unknown bits with a chosen marker |

`IncompleteByteArray` groups and converts arrays of ternary bits.
The wrapper family shares the members above through
`IncompleteBitPattern<TSelf>`.

## `NumeralSystems.Net.Encoding`

### `String`

`GetIdentity(string)` returns distinct characters in their first-occurrence
order.

## `NumeralSystems.Net.Interface`

The public generic contracts describe complete and incomplete logical types:

- `INumeralValue<T>`;
- `IIncompleteValue<TValue, TType, TIndexer>`;
- `IIncompletable<TIncomplete, TValue, TType, TIndexer>`;
- `IRegularOperable<TIncomplete, TNumeralValue, TValue, TIndexer>`;
- `IIRregularOperable<TIncomplete, TNumeralValue, TValue, TIndexer>`;
- `IRegularReversible<TIncomplete, TNumeralValue, TValue, TIndexer>`;
- `IRregularReversible<TIncomplete, TNumeralValue, TValue, TIndexer>`.

Use these interfaces when writing algorithms that should work across multiple
wrapper widths.

## `NumeralSystems.Net.Utils`

### `Sequence`

Provides:

- identity sequence generation with `SequenceOfIdentityAtIndex` and
  `IdentityEnumerableOfSize`;
- `Range` overloads for `uint`, `ulong`, and `BigInteger`;
- bounded counting with `CountToUInt` and `CountToULong`;
- permutation and combination counts;
- the `Group` array extension.

### `StringExtensions`

Provides `SplitAndKeep`, `TakeOnly`, and `Remove` for delimiter-aware string
tokenization.

### `Math`

The partial static `Math` class supplies extensions for:

- `And`, `Or`, `Xor`, `Nand`, and `Not` on `bool`, `bool?`, and arrays;
- `ReverseAnd`, `ReverseOr`, `CanReverseAnd`, and `CanReverseOr`;
- bitwise `And` for `decimal`, `double`, and `float`;
- `DigitsInBase` for integer widths.

Array operations require compatible lengths. Nullable Boolean operations use
the unknown-bit semantics described in
[Incomplete values](incomplete-values.md).
