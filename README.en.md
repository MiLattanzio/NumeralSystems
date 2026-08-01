# NumeralSystems.Net

[![Build](https://github.com/MiLattanzio/NumeralSystems/actions/workflows/dotnet.yml/badge.svg)](https://github.com/MiLattanzio/NumeralSystems/actions/workflows/dotnet.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.txt)

English · [Italiano](README.md)

NumeralSystems.Net is a .NET library for representing, converting, and
formatting values in arbitrary numeral systems and performing cross-base
rational arithmetic. It also provides bit-oriented primitive wrappers, values
with unknown bits, and reverse logical operations.

Use the library when you need to:

- convert integral or fractional values between bases;
- calculate and compare signed values written in different bases;
- format digits with an ordered, validated, immutable alphabet;
- obtain structured parsing errors with an exact UTF-16 position;
- encode bytes with standard Base16, Base32, or Base64, including streams;
- process UTF-16 code units or Unicode scalar values explicitly;
- format through `IFormatProvider` and serialize exact numerals with the
  optional `NumeralSystems.Net.Json` package;
- inspect and modify the binary representation of primitive values;
- describe partial values whose bits can be `0`, `1`, or unknown;
- recover possible operands of `AND`, `OR`, `XOR`, and `NAND`;
- combine bit constraints, apply masks, and safely enumerate bounded candidates.

## Requirements

- .NET 8 SDK to build the solution and run its tests;
- a .NET Standard 2.1-compatible runtime to consume the portable library API;
- .NET 8 for Rune, Span, the WebAssembly playground, and `NumeralSystems.Net.Json`.

The repository contains the library, JSON package, global tool, playground,
examples, benchmarks, and NUnit suite. All three distributable packages are
built and published automatically for a valid GitHub Release.

## Quick start

```bash
git clone https://github.com/MiLattanzio/NumeralSystems.git
cd NumeralSystems/NumeralSystems.Net
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
```

Install the packages and global tool from NuGet:

```bash
dotnet add package NumeralSystems.Net --version 5.2.0
dotnet add package NumeralSystems.Net.Json --version 5.2.0
dotnet tool install --global dotnet-numeralsystems --version 5.2.0

numsys convert FF --from 16 --to 2
numsys inspect "1100????" --type byte
numsys solve "x & 10101010 = 10001000"
```

To consume the source project from another solution:

```xml
<ItemGroup>
  <ProjectReference Include="path/to/NumeralSystems.Net.csproj" />
</ItemGroup>
```

## First example

`NumeralSystem` defines a base. Its indexers create a `Numeral` from a .NET
value.

```csharp
using NumeralSystems.Net;

var hex = Numeral.System.OfBase(16);

var encoded = hex[255];
Console.WriteLine(encoded);          // FF
Console.WriteLine(encoded.Integer);  // 255

var parsed = hex.Parse("FF");
Console.WriteLine(parsed.Integer);   // 255
```

Default serialization uses the current culture for its sign and decimal
separator. Specify the alphabet and separators explicitly for persistent
formats and protocols.

## Main APIs

| Area | Types | Purpose |
| --- | --- | --- |
| Numeral systems | `NumeralSystem`, `Numeral` | Create, parse, format, and convert values between bases |
| Ordered alphabets | `NumeralAlphabet`, `ParseResult` | Encode deterministically, validate symbols, and diagnose parsing |
| Non-negative digits | `Value` | Store integral digit sequences, including arbitrary-precision integers |
| Exact rational values | `RationalValue`, `NumeralValue` | Preserve `BigInteger/BigInteger` values and project them into any base |
| Expansion policy | `NumeralConversionOptions`, `NumeralExpansion` | Bound digits, round, reject infinity, or preserve a repeating period |
| Bitwise primitives | `Type.Base.*` | Wrap bytes, integers, characters, and floating-point values |
| Unknown bits | `BitPattern`, `BitConstraint`, `BitConstraintSet` | Represent ternary patterns, compose constraints, explain solutions, and enumerate with an explicit limit |
| Standard byte codecs | `StandardBaseCodec` | RFC Base16/Base32/Base64 with in-memory, Span, and streaming APIs |
| Character processing | `CharacterIdentity`, `CharacterRadixTransform` | Explicit UTF-16 or Rune identities and experimental radix transforms |
| Formatting | `NumeralFormatInfo` | Provider-driven text, `G`/`R` formats, and Span |
| Optional JSON | `NumeralSystems.Net.Json`, `NumeralJsonConverter` | Explicit registration and exact structured JSON |

### Custom alphabet

Each position in `identity` defines one digit. The alphabet must have at least
as many entries as the numeral system's base.

```csharp
using NumeralSystems.Net;

var dozenal = Numeral.System.OfBase(12);
dozenal.AdjustToFitIntegralLength = false;

var alphabet = new NumeralAlphabet(
    "0123456789XY".Select(character => character.ToString()));

var value = dozenal[143];
var text = value.ToString(alphabet, separator: "", negativeSign: "-", numberDecimalSeparator: ".");

Console.WriteLine(text); // YY
Console.WriteLine(dozenal.Parse(text, alphabet, "", "-", ".").Integer); // 143
```

`NumeralAlphabet` rejects duplicate, empty, and prefix-ambiguous symbols as
well as conflicts with separators and signs.

### Exact alphabet round trips

```csharp
BigInteger value = BigInteger.Pow(2, 256) + 42;
var text = NumeralAlphabet.Base62.Encode(value);
var decoded = NumeralAlphabet.Base62.Decode(text);

Console.WriteLine(decoded == value); // True
```

Predefined alphabets are available for bases 2, 8, 10, 16, 32, 36, 58, 62,
and 64. Structured parsing returns `ParseResult` with `Reason`, `Position`,
`ErrorLength`, and `Message`.

### Standard byte encodings and Unicode units

Standard byte codecs are separate from numeral alphabets:

```csharp
using NumeralSystems.Net.Encoding;

var encoded = StandardBaseCodec.EncodeBase64(bytes);
var decoded = StandardBaseCodec.DecodeBase64(encoded);
```

For the experimental character-radix transformation, choose the unit
explicitly: `EncodeUtf16` preserves .NET `char` units, while .NET 8
`EncodeRunes` treats supplementary characters as single Unicode scalars. Both
families also have constant-memory reader/writer or stream APIs.

`Numeral` implements `IFormattable` with provider-driven `G` and invariant `R`
formats. The .NET 8 asset adds Span overloads. Exact `System.Text.Json`
serialization lives in the separate `NumeralSystems.Net.Json` package and is
enabled explicitly through `options.AddNumeralSystems()`.

### Tool and playground

`numsys` exposes conversion, bounded candidate inspection, and composed AND,
OR, XOR, and NAND solving from a shell. `NumeralSystems.Net.Playground` is a
backend-free Blazor WebAssembly app with a converter, fraction-period graph,
unknown-bit viewer, and per-bit constraint explanations:

```bash
dotnet run --project NumeralSystems.Net/NumeralSystems.Net.Playground
dotnet run --project NumeralSystems.Net/NumeralSystems.Net.Examples -- all
```

### Cross-base arithmetic

`NumeralValue` stores a normalized exact rational value. Positional digits are
an immutable projection, so a truncated display never corrupts later arithmetic:

```csharp
var binaryHalf = NumeralValue.FromRational(1, 2, baseValue: 2);

var decimalQuarter = NumeralValue.FromDecimal(0.25m);
var sum = binaryHalf.Add(
    decimalQuarter,
    NumeralConversionOptions.Default,
    resultBase: 2);

Console.WriteLine(sum.Base);        // 2
Console.WriteLine(sum.ToDecimal()); // 0.75
```

`NumeralConversionOptions` makes the digit limit, rounding rule, period
detection, and infinite-expansion behavior explicit. For example, decimal
`0.1` expands in base 2 as exact `0.0(0011)`, while `1/3` is terminating
`0.1` in base 3. Operators `+`, `-`, `*`, and `/` use the left operand's base
and retain the exact rational state.

### Reverse bitwise operations

Reverse operations return an incomplete value because multiple operands can
produce the same result.

```csharp
using IntValue = NumeralSystems.Net.Type.Base.Int;

var left = new IntValue { Value = 0b1100 };
var right = new IntValue { Value = 0b1010 };
var result = left.And(right);

if (result.ReverseAnd(right, out var possibleLeft))
{
    Console.WriteLine(result.Value);          // 8
    Console.WriteLine(possibleLeft.Contains(left)); // True
}
```

### Immutable bit patterns and constraints

`BitPattern` is the shared engine used by all `Incomplete*` wrappers. Candidate
counts and encoded bounds use `BigInteger`, while enumeration always accepts an
explicit limit:

```csharp
using NumeralSystems.Net.Type.Incomplete;

var constraints = BitConstraintSet.Parse(
    "x & 10101010 = 10001000; " +
    "x | 00001111 = 10001111");
var solution = constraints.Solve(new BitConstraintSolverOptions(
    maximumEnumeratedCandidates: 4,
    timeout: TimeSpan.FromSeconds(1)));

Console.WriteLine(solution.GetPatternOrThrow()); // 10001?0?
Console.WriteLine(solution.CandidateCount);      // 4
foreach (var explanation in solution.Explanations)
    Console.WriteLine(explanation.Message);
```

The engine also provides compatibility/intersection, reverse XOR/NAND, logical
and arithmetic shifts, rotate-left/right, and three-valued masks.

## Documentation

The complete guide lives in [`NumeralSystems.Net/docs`](NumeralSystems.Net/docs/index.md):

- [getting started and integration](NumeralSystems.Net/docs/getting-started.md);
- [numeral systems and alphabets](NumeralSystems.Net/docs/numeral-systems.md);
- [ordered numeral alphabets, presets, and parse diagnostics](NumeralSystems.Net/docs/numeral-alphabets.md);
- [formatting providers, Span, and JSON](NumeralSystems.Net/docs/formatting-and-serialization.md);
- [global tool and WebAssembly playground](NumeralSystems.Net/docs/tool-and-playground.md);
- [executable examples and notebooks](NumeralSystems.Net/docs/examples-and-notebooks.md);
- [arithmetic, precision, operators, and comparison](NumeralSystems.Net/docs/arithmetic.md);
- [exact rational values, repeating periods, and rounding](NumeralSystems.Net/docs/exact-rationals.md);
- [task-oriented cookbook](NumeralSystems.Net/docs/cookbook.md);
- [primitive wrappers and bitwise operations](NumeralSystems.Net/docs/bitwise-values.md);
- [the immutable BitPattern engine and constraint solving](NumeralSystems.Net/docs/bit-patterns.md);
- [composable bitwise constraints, explanations, and limits](NumeralSystems.Net/docs/bit-constraints.md);
- [incomplete values and reverse operations](NumeralSystems.Net/docs/incomplete-values.md);
- [string encoding](NumeralSystems.Net/docs/string-encoding.md);
- [troubleshooting](NumeralSystems.Net/docs/troubleshooting.md);
- [API reference](NumeralSystems.Net/docs/api-reference.md);
- [architecture and contributor notes](NumeralSystems.Net/docs/architecture.md);
- [migration to 4.7.0](NumeralSystems.Net/docs/migration-4.7.md);
- [migration to 4.8.0](NumeralSystems.Net/docs/migration-4.8.md);
- [migration to 4.8.1](NumeralSystems.Net/docs/migration-4.8.1.md);
- [migration from 4.8.1 to 5.0.0](NumeralSystems.Net/docs/migration-5.0.md);
- [migration from 5.0.0 to 5.1.0](NumeralSystems.Net/docs/migration-5.1.md);
- [migration from 5.1.0 to 5.2.0](NumeralSystems.Net/docs/migration-5.2.md);
- [release and NuGet publishing process](NumeralSystems.Net/docs/releasing.md).

All documentation is maintained as Markdown and versioned with the code. No
documentation generator or additional tool is required to read or edit it.

## Benchmarks

Performance benchmarks live in a separate project so they do not affect test
discovery or execution. They cover formatting, parsing, conversion, rational
arithmetic, repeating division, large-value comparison, and constraint solving:

```bash
dotnet run --configuration Release \
  --project NumeralSystems.Net.Benchmarks/NumeralSystems.Net.Benchmarks.csproj
```

Every GitHub Release attaches complete Markdown/JSON benchmark exports and a
ready-to-host static archive of the WebAssembly playground.

## Important behavior

- A positional base must be 2 or greater.
- Every digit is an integer index in the range `0..base-1`.
- Fractional digits have positional meaning in their declared base; exact
  rational state survives periodic, truncated, and rounded projections.
- `NumeralConversionOptions` makes digit limits, rounding, period detection,
  and infinite-expansion behavior explicit.
- `BigInteger` indexers and views avoid primitive integer-size limits.
- `Value` does not preserve a sign or a fractional part; use `NumeralValue` or
  `Numeral` when those are required.
- Primitive-wrapper `Binary` arrays are indexed from the least-significant bit;
  `ToString()` provides a human-readable view.
- `NumeralAlphabet.Base64`, standard RFC Base64, and the experimental
  character transform are separate APIs with different data models.
- Rune and Span are available in the .NET 8 package asset; portable UTF-16 and
  streaming APIs remain in .NET Standard 2.1. JSON is a separate .NET 8 package
  so serialization dependencies are not imposed on the core package.

## Contributing and security

Read [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request. Do not
report vulnerabilities in public issues; follow [SECURITY.md](SECURITY.md).
Private project contact: [mi@polecola.it](mailto:mi@polecola.it).

The project follows its [Code of Conduct](CODE_OF_CONDUCT.md) and is available
under the [MIT License](LICENSE.txt).
