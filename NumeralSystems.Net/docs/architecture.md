# Architecture and contributor notes

[Documentation home](index.md) ·
[API reference](api-reference.md) ·
[Migration guide](migration-4.7.md) ·
[Contributing](../../CONTRIBUTING.md)

## Solution layout

```text
NumeralSystems.Net/
├── NumeralSystems.Net/             Library targeting .NET Standard 2.1
├── NumeralSystem.Net.NUnit/        NUnit regression and behavior tests
├── NumeralSystems.Net.Benchmarks/  BenchmarkDotNet performance suite
├── docs/                           Versioned Markdown documentation
├── global.json                     SDK selection policy
└── NumeralSystems.Net.sln          Solution entry point
```

The library intentionally keeps runtime dependencies small. Repository tooling,
tests, and benchmarks target .NET 8, while the package remains consumable from
any .NET Standard 2.1-compatible runtime.

## Domain layers

The public API has four main layers:

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

### `Numeral`

Combines a `NumeralSystem` reference with:

- integral digits;
- fractional digits;
- a sign.

It is the appropriate type when parsing, formatting, and custom alphabets are
part of the operation.

### `Value`

Stores a non-negative integral digit sequence and its base. It is intentionally
smaller than `Numeral`:

- no sign;
- no fraction;
- no alphabet;
- arbitrary-precision integral conversion.

Use it for encoded identifiers and raw digit transformations.

### `NumeralValue`

Stores signed integral and fractional digits without formatting concerns. It is
the arithmetic type:

- exact rational intermediate calculations;
- bounded result expansion;
- arbitrary-precision integral views;
- base-independent comparison.

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

Candidate count grows exponentially. New APIs should avoid eager enumeration
unless the caller explicitly requests it.

Reverse logical operations produce an incomplete operand that describes every
compatible complete value.

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
- large cross-base comparison.

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
