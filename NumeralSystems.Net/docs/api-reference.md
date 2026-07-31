# API reference

[Documentation home](index.md) ·
[Getting started](getting-started.md) ·
[Numeral systems](numeral-systems.md) ·
[Numeral alphabets](numeral-alphabets.md) ·
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
| `FromString(string, HashSet<string>)` | Obsolete compatibility overload; ordinally sorts the set |
| `FromString(string, bool fit = false)` | Uses UTF-16 character values as digit indices |
| `FromBigInteger(BigInteger, int baseValue = 10)` | Creates a digit sequence without an integer-size limit |
| `ToBigInteger()` | Returns the complete non-negative integer value |
| `ToString(NumeralAlphabet, separator)` | Formats stored digits with an ordered alphabet |
| `ToBase(int, bool removeFirstZeros = false)` | Returns the same integral value in another base |

`Value` does not represent a sign or fractional digits.

### `NumeralValue`

Represents integral and fractional digit lists, a sign, and a base.

| Member family | Members |
| --- | --- |
| State | `Integral`, `Decimals`, `Negative`, `Base`, `IsZero` |
| Precision | `DefaultMaxFractionalDigits` |
| Construction | constructor, `FromDecimal`, `FromBigInteger` (optionally with a base), `FromInt`, `FromFloat`, `FromDouble`, `FromValue` |
| Primitive conversion | `ToBigInteger`, `ToDecimal`, `ToInt`, `ToFloat`, `ToDouble`, `ToValue` |
| Base conversion | `ToBase`, `TryToBase` |
| Arithmetic | `Add`, `Subtract`, `Multiply`, `Divide`, `Negate`, `Abs` |
| Comparison | `CompareTo`, `NumericallyEquals` |
| Operators | binary `+`, `-`, `*`, `/`; unary `-`; `<`, `>`, `<=`, `>=` |

`TryToBase` returns `false` when a repeating fractional expansion reaches the
requested digit limit. Its output still contains the truncated conversion.

Arithmetic methods have:

- a short overload that returns a result in the current instance's base;
- a precision-aware overload with `out bool exact`, optional `resultBase`, and
  `maxFractionalDigits`.

Operators use the left operand's base and
`DefaultMaxFractionalDigits`. Division by zero throws
`DivideByZeroException`. `NumericallyEquals` compares exact numeric magnitude
without changing the reference-equality behavior of `object.Equals`.

### `NumeralSystem`

Defines a base and the rules used to construct, parse, and format `Numeral`
instances.

| Member family | Members |
| --- | --- |
| Configuration | `Size`, `Length`, `SkipUnknownValues`, `AdjustToFitIntegralLength` |
| Parsing | `Parse`, structured and Boolean `TryParse`, `TrySplitNumberIndices` |
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
| State | `Positive`, `Base`, `IntegralIndices`, `FractionalIndices` |
| Text components | `GetIntegralStrings`, `GetIntegralString`, `GetFractionalStrings`, `GetFractionalString` |
| Primitive views | `BigInteger`, `Integer`, `Char`, `Double`, `Decimal`, `Float`, `Bytes` |
| Mutation | property setters, `TrySetValue` |
| Conversion | `To(NumeralSystem)` |
| Formatting | `ToString()`, `ToString(identity, separator, negativeSign, decimalSeparator)` |

`Numeral` also provides `NumeralAlphabet` overloads for digit access and
formatting, plus `ToString(SerializationInfo)`.

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

`Numeral.System.OfBase(int)` is the convenience factory. The nested
`Numeral.System.Characters` type exposes:

- `Numbers`, `UpperLetters`, `LowerLetters`, and `Symbols`;
- `Alphanumeric`, `AlphanumericUpper`, `AlphanumericLower`, and
  `AlphanumericSymbols`;
- `Printable`, `NotPrintable`, `All`, and `WhiteSpaces`;
- `Point`, `Comma`, `Minus`, and `Semicolon`.

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
| `String` | `EncodeToBase`, `DecodeFromBase`, `ToIndicesOfBase`, `FromIndicesOfBase`, `GetSmallestBase` |

### `String`

The instance API implements `IList<Type.Base.Char>` and includes the normal
mutable collection members plus `ToString()` and `ToString(format)`. Its static
base-conversion methods are described in [String encoding](string-encoding.md).

## `NumeralSystems.Net.Type.Incomplete`

### `BitPattern`

The immutable shared engine uses least-significant-bit-first `bool?` values and
implements `IReadOnlyList<bool?>`.

| Member family | Members |
| --- | --- |
| Construction | constructors from `IEnumerable<bool?>` or `IEnumerable<bool>`, `FromUnsigned`, `Unknown` |
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
