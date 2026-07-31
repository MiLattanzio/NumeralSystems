# API reference

[Documentation home](index.md) ·
[Getting started](getting-started.md) ·
[Numeral systems](numeral-systems.md) ·
[Arithmetic](arithmetic.md) ·
[Cookbook](cookbook.md) ·
[Bitwise values](bitwise-values.md) ·
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
| `FromString(string, HashSet<string>)` | Maps symbols to indices using set enumeration order |
| `FromString(string, bool fit = false)` | Uses UTF-16 character values as digit indices |
| `FromBigInteger(BigInteger, int baseValue = 10)` | Creates a digit sequence without an integer-size limit |
| `ToBigInteger()` | Returns the complete non-negative integer value |
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
| Parsing | `Parse`, `TryParse`, `TrySplitNumberIndices` |
| Formatting | `TryFromIndices` |
| Validation/conversion | `Contains`, `TryBigIntegerOf`, `TryIntegerOf`, `TryCharOf` |
| Numeric indexers | `BigInteger`, `int`, `double`, `decimal`, `long`, `ulong`, `uint`, `short`, `ushort`, `sbyte`, `byte` |
| Sequence indexers | `IEnumerable<byte>`, `IEnumerable<char>`, `IEnumerable<int>`, `IList<byte>`, `IList<char>`, `List<int>` |

`NumeralSystem.SerializationInfo` holds `Identity`, `Separator`,
`NegativeSign`, and `NumberDecimalSeparator`. `OfBase(int)` creates
culture-aware defaults.

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
| `ReverseAnd`, `ReverseOr` | Solve for possible left operands |
| `Incomplete` | Convert to the corresponding fully known incomplete value |
| `ToString()` | Format the binary representation |
| `ToString(format)` | Delegate formatting to the underlying primitive |

Overloads accept either a complete wrapper or the matching incomplete type.

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

Incomplete primitive types store `bool?[] Binary`. Most expose:

| Member | Purpose |
| --- | --- |
| `Binary` | Ternary bit array: zero, one, or unknown |
| `IsComplete` | Whether every bit is known |
| `Permutations` | Candidate count implied by unknown bits |
| `this[index]` | Materialize one candidate |
| `Enumerable` | Enumerate all candidates |
| `ByteArray` / `ToByteArray` | Split into incomplete bytes |
| `Contains` | Test complete or incomplete compatibility |
| `Not`, `And`, `Or`, `Xor` | Three-valued logical operations |
| `ReverseAnd`, `ReverseOr` | Solve a logical equation when possible |
| `ToString(missingSeparator)` | Format unknown bits with a chosen marker |

`IncompleteByteArray` groups and converts arrays of ternary bits.

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
