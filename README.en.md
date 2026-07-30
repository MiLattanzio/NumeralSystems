# NumeralSystems.Net

[![Build](https://github.com/MiLattanzio/NumeralSystems/actions/workflows/dotnet.yml/badge.svg)](https://github.com/MiLattanzio/NumeralSystems/actions/workflows/dotnet.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE.txt)

English · [Italiano](README.md)

NumeralSystems.Net is a .NET library for representing, converting, and
formatting values in arbitrary numeral systems. It also provides bit-oriented
primitive wrappers, values with unknown bits, and reverse logical operations.

Use the library when you need to:

- convert integral or fractional values between bases;
- format digits with a custom alphabet;
- inspect and modify the binary representation of primitive values;
- describe partial values whose bits can be `0`, `1`, or unknown;
- recover possible operands of an `AND` or `OR` operation.

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
| Non-negative digits | `Value` | Store integral digit sequences and convert their base |
| Signed and fractional values | `NumeralValue` | Store integral/fractional parts and convert to or from .NET values |
| Bitwise primitives | `Type.Base.*` | Wrap bytes, integers, characters, and floating-point values |
| Unknown bits | `Type.Incomplete.*` | Represent ternary bit patterns, enumerate candidates, and test them with `Contains` |
| Encoding | `Type.Base.String`, `Encoding.String` | Convert strings to digits in another base and derive symbol identities |

### Custom alphabet

Each position in `identity` defines one digit. The alphabet must have at least
as many entries as the numeral system's base.

```csharp
using NumeralSystems.Net;

var dozenal = Numeral.System.OfBase(12);
dozenal.AdjustToFitIntegralLength = false;

var digits = "0123456789XY"
    .Select(character => character.ToString())
    .ToList();

var value = dozenal[143];
var text = value.ToString(digits, separator: "", negativeSign: "-", numberDecimalSeparator: ".");

Console.WriteLine(text); // YY
Console.WriteLine(dozenal.Parse(text, digits, "", "-", ".").Integer); // 143
```

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

## Documentation

The complete guide lives in [`NumeralSystems.Net/docs`](NumeralSystems.Net/docs/index.md):

- [getting started and integration](NumeralSystems.Net/docs/getting-started.md);
- [numeral systems and alphabets](NumeralSystems.Net/docs/numeral-systems.md);
- [primitive wrappers and bitwise operations](NumeralSystems.Net/docs/bitwise-values.md);
- [incomplete values and reverse operations](NumeralSystems.Net/docs/incomplete-values.md);
- [string encoding](NumeralSystems.Net/docs/string-encoding.md);
- [API reference](NumeralSystems.Net/docs/api-reference.md);
- [release and NuGet publishing process](NumeralSystems.Net/docs/releasing.md).

All documentation is maintained as Markdown and versioned with the code. No
documentation generator or additional tool is required to read or edit it.

## Benchmarks

Performance benchmarks live in a separate project so they do not affect test
discovery or execution:

```bash
dotnet run --configuration Release \
  --project NumeralSystems.Net.Benchmarks/NumeralSystems.Net.Benchmarks.csproj
```

## Important behavior

- A positional base must be 2 or greater.
- Every digit is an integer index in the range `0..base-1`.
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
