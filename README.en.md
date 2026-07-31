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
- inspect and modify the binary representation of primitive values;
- describe partial values whose bits can be `0`, `1`, or unknown;
- recover possible operands of `AND`, `OR`, `XOR`, and `NAND`;
- combine bit constraints, apply masks, and safely enumerate bounded candidates.

## Requirements

- .NET 8 SDK to build the solution and run its tests;
- a .NET Standard 2.1-compatible runtime to consume the library.

The repository contains the library project and its NUnit test suite. A NuGet
package is built and published automatically for a valid published GitHub
Release, but installing it is not required to try the project.

## Quick start

```bash
git clone https://github.com/MiLattanzio/NumeralSystems.git
cd NumeralSystems/NumeralSystems.Net
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
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
| Signed and fractional values | `NumeralValue` | Convert, calculate, and compare values with bounded, observable precision |
| Bitwise primitives | `Type.Base.*` | Wrap bytes, integers, characters, and floating-point values |
| Unknown bits | `BitPattern`, `Type.Incomplete.*` | Represent ternary patterns, combine constraints, solve reverse operations, and enumerate with an explicit limit |
| Encoding | `Type.Base.String`, `Encoding.String` | Convert strings to digits in another base and derive symbol identities |

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

### Cross-base arithmetic

`NumeralValue` calculates through exact rational intermediates. Operands may use
different bases:

```csharp
var binaryHalf = new NumeralValue(
    new List<int> { 0 },
    new List<int> { 1 },
    false,
    2);

var decimalQuarter = NumeralValue.FromDecimal(0.25m);
var sum = binaryHalf.Add(decimalQuarter, out var exact);

Console.WriteLine(exact);           // True
Console.WriteLine(sum.Base);        // 2
Console.WriteLine(sum.ToDecimal()); // 0.75
```

Operators `+`, `-`, `*`, and `/` use the left operand's base. Precision-aware
methods report when the result requires a truncated repeating expansion.

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

var mask = BitPattern.FromUnsigned(0b1111_0000, width: 8);
var required = BitPattern.FromUnsigned(0b1010_0000, width: 8);

if (BitPattern.TrySolveAnd(mask, required, out var input))
{
    Console.WriteLine(input);                 // 1010????
    Console.WriteLine(input.CandidateCount);  // 16

    foreach (var candidate in input.EnumerateCandidates(limit: 4))
        Console.WriteLine(candidate);
}
```

The engine also provides compatibility/intersection, reverse XOR/NAND, logical
and arithmetic shifts, rotate-left/right, and three-valued masks.

## Documentation

The complete guide lives in [`NumeralSystems.Net/docs`](NumeralSystems.Net/docs/index.md):

- [getting started and integration](NumeralSystems.Net/docs/getting-started.md);
- [numeral systems and alphabets](NumeralSystems.Net/docs/numeral-systems.md);
- [ordered numeral alphabets, presets, and parse diagnostics](NumeralSystems.Net/docs/numeral-alphabets.md);
- [arithmetic, precision, operators, and comparison](NumeralSystems.Net/docs/arithmetic.md);
- [task-oriented cookbook](NumeralSystems.Net/docs/cookbook.md);
- [primitive wrappers and bitwise operations](NumeralSystems.Net/docs/bitwise-values.md);
- [the immutable BitPattern engine and constraint solving](NumeralSystems.Net/docs/bit-patterns.md);
- [incomplete values and reverse operations](NumeralSystems.Net/docs/incomplete-values.md);
- [string encoding](NumeralSystems.Net/docs/string-encoding.md);
- [troubleshooting](NumeralSystems.Net/docs/troubleshooting.md);
- [API reference](NumeralSystems.Net/docs/api-reference.md);
- [architecture and contributor notes](NumeralSystems.Net/docs/architecture.md);
- [migration to 4.7.0](NumeralSystems.Net/docs/migration-4.7.md);
- [release and NuGet publishing process](NumeralSystems.Net/docs/releasing.md).

All documentation is maintained as Markdown and versioned with the code. No
documentation generator or additional tool is required to read or edit it.

## Benchmarks

Performance benchmarks live in a separate project so they do not affect test
discovery or execution. They cover formatting, parsing, conversion, rational
arithmetic, repeating division, and large-value comparison:

```bash
dotnet run --configuration Release \
  --project NumeralSystems.Net.Benchmarks/NumeralSystems.Net.Benchmarks.csproj
```

## Important behavior

- A positional base must be 2 or greater.
- Every digit is an integer index in the range `0..base-1`.
- Fractional digits have positional meaning in their declared base;
  `TryToBase` reports when a repeating expansion reaches its precision limit.
- `BigInteger` indexers and views avoid primitive integer-size limits.
- `Value` does not preserve a sign or a fractional part; use `NumeralValue` or
  `Numeral` when those are required.
- Primitive-wrapper `Binary` arrays are indexed from the least-significant bit;
  `ToString()` provides a human-readable view.
- `Type.Base.String` encoding is not Base64 and may produce control characters.
  Always retain the base and width together with the encoded text.

## Contributing and security

Read [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request. Do not
report vulnerabilities in public issues; follow [SECURITY.md](SECURITY.md).

The project follows its [Code of Conduct](CODE_OF_CONDUCT.md) and is available
under the [MIT License](LICENSE.txt).
