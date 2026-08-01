# Architecture and contributor notes

[Documentation home](index.md) ·
[API reference](api-reference.md) ·
[Numeral alphabets](numeral-alphabets.md) ·
[Formatting and JSON](formatting-and-serialization.md) ·
[BitPattern engine](bit-patterns.md) ·
[Migration guide](migration-4.8.md) ·
[Contributing](../../CONTRIBUTING.md)

## Solution layout

```text
NumeralSystems.Net/
├── NumeralSystems.Net/             Library targeting .NET Standard 2.1
├── NumeralSystems.Net.Json/        Explicit .NET 8 JSON integration
├── NumeralSystems.Net.Tool/        Global `numsys` command
├── NumeralSystems.Net.Playground/  Standalone Blazor WebAssembly UI
├── NumeralSystems.Net.Examples/    Compiled executable examples
├── NumeralSystem.Net.NUnit/        NUnit regression and behavior tests
├── NumeralSystems.Net.Benchmarks/  BenchmarkDotNet performance suite
├── docs/                           Versioned Markdown documentation
├── global.json                     SDK selection policy
└── NumeralSystems.Net.sln          Solution entry point
```

The library intentionally keeps runtime dependencies small. Repository tooling,
tests, and benchmarks target .NET 8. The package contains a portable .NET
Standard 2.1 asset plus a .NET 8 asset for Rune, Span, and JSON integration.

## Domain layers

The numeral API has four main layers:

```text
Text and alphabets
        │
        ▼
NumeralSystem + Numeral
        │
        ▼
Value + NumeralValue
        │
        ▼
Positional conversion core
```

Text and binary encoding form explicit sibling branches rather than flowing
through numeral representation:

```text
bytes ---- StandardBaseCodec ---- RFC Base16 / Base32 / Base64 text

text ---- CharacterIdentity ----- ordered UTF-16 units or Runes
  |
  +------ CharacterRadixTransform ---- experimental raw radix digits
```

This boundary prevents a numeral alphabet named `Base64` from being mistaken
for an RFC Base64 byte codec. `NumeralAlphabet` maps digit values to symbols;
`StandardBaseCodec` groups bits from bytes; `CharacterRadixTransform` maps
character numeric values to fixed-width raw digits.

Bitwise primitive wrappers and incomplete values form a separate branch:

```text
Type.Base.* ── logical operations ── Type.Incomplete.*
      │                                  │
      └──────── reverse AND / OR ────────┘
```

### `NumeralSystem`

Owns base-level behavior:

- base validation;
- parsing and formatting;
- default serialization settings;
- numeric and digit-sequence indexers;
- conversion from digit indices to integer views.

It does not own a particular numeric value.

### Formatting and serialization

`NumeralFormatInfo` is the immutable bridge to `IFormatProvider`. `Numeral`
implements `IFormattable` on all targets and `ISpanFormattable` on .NET 8.
Provider parsing routes back through the same validated `NumeralAlphabet`
scanner, so there is no second text grammar.

On .NET 8, the optional `NumeralSystems.Net.Json` package serializes numeric
structure rather than formatted text through `NumeralJsonConverter`:

```text
Numeral -> { base, positive, numerator, denominator, integral[], fractional[] }
```

That representation preserves leading/trailing digit zeros and the exact
rational state independently of culture. Numerator and denominator are JSON
strings so they are not limited by a consumer's numeric precision.

### Streaming encodings

`StandardBaseCodec` maintains only a small byte/character buffer plus the
partial bit group spanning reads. Character-transform streaming requires a
caller-selected fixed width because a one-pass reader cannot inspect the
complete input to infer its maximum scalar first. All streaming APIs leave
caller-owned streams, readers, and writers open.

### `NumeralAlphabet`

Owns the ordered text-to-digit mapping:

- immutable ordinal symbols;
- duplicate and prefix validation;
- separator/sign conflict validation;
- exact signed `BigInteger` encoding and decoding;
- predefined and deterministic generated alphabets.

Parsing scans the input against this mapping and returns a `ParseResult` with a
UTF-16 position and `ParseErrorReason`. Legacy list-based numeral overloads
remain available, but unordered `HashSet<string>` alphabet input was removed in
4.8.1.

```text
text + NumeralAlphabet + formatting tokens
                    |
                    v
          validated token scanner
                    |
          +---------+---------+
          |                   |
          v                   v
       Numeral          ParseResult error
```

### `Numeral`

Combines a `NumeralSystem` reference with:

- integral digits;
- fractional digits;
- a sign;
- an exact `RationalValue` snapshot when created through the 5.0 factories.

It is the appropriate type when parsing, formatting, and custom alphabets are
part of the operation. Digit getters return copies. The 4.x mutation surface
was removed in 5.1; create replacement values with `FromRational`,
`FromRepresentation`, and `WithExactValue`.

### `Value`

Stores a non-negative integral digit sequence and its base. It is intentionally
smaller than `Numeral`:

- no sign;
- no fraction;
- no alphabet;
- arbitrary-precision integral conversion.

Use it for encoded identifiers and raw digit transformations.

### `NumeralValue`

Stores a normalized exact rational value plus an immutable positional digit
projection without formatting concerns. It is the arithmetic type:

- exact rational state and calculations;
- explicit bounded, periodic, truncated, or rounded expansion;
- arbitrary-precision integral views;
- base-independent comparison.

### `RationalValue`, options, and expansion

`RationalValue` owns normalized `BigInteger/BigInteger` state and exact
arithmetic. `NumeralConversionOptions` is immutable policy. `NumeralExpansion`
owns read-only digits and termination/period/rounding metadata. This split
prevents a finite display buffer from becoming the numeric source of truth.

```text
RationalValue + base + NumeralConversionOptions
                         |
                         v
              remainder expansion loop
                         |
             +-----------+------------+
             |                        |
             v                        v
      terminating digits       period / bounded tail
                         |
                         v
                 NumeralExpansion
```

## Positional conversion core

`Utils/PositionalNotation.cs` centralizes the mathematical rules used by the
public types.

An integral digit sequence is accumulated with:

```text
result = result × base + nextDigit
```

This avoids floating-point powers and intermediate primitive overflow.

A fractional sequence is converted to a reduced rational:

```text
fraction = digitMagnitude / base^digitCount
```

To emit fractional digits in another base, the core repeatedly:

1. multiplies the remainder by the destination base;
2. divides by the denominator;
3. emits the quotient as the next digit;
4. retains the new remainder.

The expansion is exact when the remainder reaches zero. Otherwise it stops at
`maxFractionalDigits`.

## Arithmetic flow

For two operands:

```text
left digits  ──► reduced rational ──┐
                                    ├─► rational operation
right digits ──► reduced rational ──┘
                                             │
                                             ▼
                              bounded destination-base expansion
                                             │
                                             ▼
                                      NumeralValue result
```

The operation itself is exact. Only the final finite digit representation can
be truncated.

## Sign conventions

`Numeral.Positive` uses a positive flag. `NumeralValue.Negative` uses a negative
flag. Conversion code must invert the flag when moving between these two types.

Arithmetic normalizes zero to non-negative. Constructors preserve the supplied
sign so callers can still model signed zero explicitly when needed.

## Primitive conversion boundaries

The digit model and rational arithmetic use `BigInteger`, but public primitive
views remain bounded:

| View | Primary limitation |
| --- | --- |
| `ToBigInteger` | Fraction is truncated |
| `ToInt` / `Numeral.Integer` | 32-bit range |
| `ToDecimal` / `Numeral.Decimal` | `decimal` range and precision |
| `ToDouble` / `Numeral.Double` | IEEE 754 range and precision |
| `ToFloat` / `Numeral.Float` | IEEE 754 single-precision range |

Do not introduce primitive intermediates into an arbitrary-precision path.

## Bit ordering

The primitive wrapper layer follows the `Polecola.Primitive` representation:

```text
Binary[0] = least-significant bit
```

Human-readable `ToString()` output reverses that storage-oriented perspective.
Tests should assert both numeric values and bit positions when changing this
area.

## Incomplete values

Incomplete types use `bool?`:

| Value | Meaning |
| --- | --- |
| `false` | known zero |
| `true` | known one |
| `null` | unknown bit |

`BitPattern` owns the shared set logic, exact `BigInteger` cardinality,
bounded candidate enumeration, compatibility/intersection, shifts, rotations,
and reverse constraints. `IncompleteBitPattern<TSelf>` adapts that immutable
engine to every legacy mutable `Incomplete*` wrapper.
`CompleteBitPattern<TSelf, TIncomplete>` exposes reverse XOR/NAND on the
complete primitive wrappers through the same engine.

```text
IncompleteInt / IncompleteLong / ...
                 |
                 v
      IncompleteBitPattern<TSelf>
                 |
                 v
             BitPattern
```

Candidate count grows exponentially. New APIs use
`EnumerateCandidates(limit)` so callers must choose an explicit upper bound.
The historical unbounded `Enumerable` properties remain for compatibility.

Reverse logical operations produce a pattern containing the per-bit projection
of every compatible complete value. The representation intentionally cannot
encode correlations between different bit positions.

### Composable constraints

Version 5.2 places the grammar and solver in the portable core assembly. The
CLI and playground are presentation adapters over exactly the same types:

```text
constraint text
      |
      v
BitConstraintParser ---> BitConstraint
                              |
                              v
                       BitConstraintSet
                              |
                   per-bit truth-table intersection
                              |
                   +----------+-----------+
                   |                      |
                   v                      v
              BitPattern       bit explanations / conflict
```

The solver is linear in bit width times constraint count. It tests whether zero
and one remain possible at each position and never walks the exponential
candidate space. `BitConstraintSolverOptions` bounds untrusted count, width,
enumeration, and elapsed time; cancellation is checked inside both solving and
candidate enumeration.

## Test organization

The NUnit project groups tests by concern:

| Area | Representative tests |
| --- | --- |
| Numeral parsing and formatting | `NumeralTests` |
| Positional conversion | `PositionalConversionTests` |
| Rational arithmetic | `NumeralValueArithmeticTests` |
| Constructor and input validation | `ValidationTests` |
| Primitive wrappers | `Type/*` |
| Incomplete values | `Type/Incomplete/*` |
| Constraint engine | `BitConstraintTests`, `ToolTests` |
| Logical operations | `Math/*`, `BinaryOperationsTests` |
| String conversion | `Encoding/*`, `Utils/EncodeTests` |

Randomized tests use fixed seeds so CI failures can be reproduced.

When fixing a bug:

1. add a focused regression test with a named mathematical value;
2. add a boundary test;
3. keep randomized coverage deterministic;
4. run the full suite in `Release`.

## Benchmarks

BenchmarkDotNet is isolated from NUnit to keep test discovery fast and stable.

Current benchmark groups cover:

- numeral formatting and parsing;
- base conversion;
- same-base arithmetic;
- cross-base arithmetic;
- repeating division;
- large cross-base comparison;
- bit-constraint parsing, single-rule solving, and composed solving.

Run a specific benchmark:

```bash
dotnet run --configuration Release \
  --project NumeralSystems.Net.Benchmarks/NumeralSystems.Net.Benchmarks.csproj \
  --filter '*NumeralValueArithmeticBenchmarks*'
```

Do not treat one local run as a regression gate. Record hardware, runtime,
configuration, and baseline when publishing performance results.

## Documentation contract

Public API XML comments are packaged in `NumeralSystems.Net.xml`.
Conceptual documentation lives in `docs/`.

When adding a public member:

1. document parameters, return value, precision, and exceptions in XML;
2. update `api-reference.md`;
3. add a conceptual example when the behavior is not obvious;
4. add a cookbook recipe for common user-facing workflows;
5. update the changelog.

The compiler ignores only missing-comment warning `CS1591`; malformed XML
documentation still fails the build.

## Release flow

The release pipeline:

1. checks out the published release tag;
2. builds and runs tests;
3. validates semantic version syntax;
4. packs `.nupkg` and `.snupkg`;
5. uploads the artifacts;
6. exchanges a GitHub OIDC token for a short-lived NuGet key;
7. pushes to NuGet.org.

See [Releasing](releasing.md) for configuration and recovery steps.
