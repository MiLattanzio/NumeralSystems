# Getting started

[Documentation home](index.md) ·
[Numeral systems](numeral-systems.md) ·
[Numeral alphabets](numeral-alphabets.md) ·
[Arithmetic](arithmetic.md) ·
[Cookbook](cookbook.md) ·
[API reference](api-reference.md)

## Requirements

- .NET 8 SDK for the repository solution and test project;
- a .NET Standard 2.1-compatible runtime for applications that consume the
  library;
- Git if you are building from the repository.

The SDK version policy is recorded in `NumeralSystems.Net/global.json`.

## Clone, build, and test

From a terminal:

```bash
git clone https://github.com/MiLattanzio/NumeralSystems.git
cd NumeralSystems/NumeralSystems.Net
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
```

The solution contains:

```text
NumeralSystems.Net/
├── NumeralSystems.Net/            # .NET Standard 2.1 library
├── NumeralSystem.Net.NUnit/       # NUnit tests on .NET 8
├── NumeralSystems.Net.Benchmarks/ # BenchmarkDotNet performance suite
├── docs/                          # Markdown documentation
├── NumeralSystems.Net.sln
└── global.json
```

## Reference the source project

Add a project reference from your application:

```xml
<ItemGroup>
  <ProjectReference Include="path/to/NumeralSystems.Net/NumeralSystems.Net.csproj" />
</ItemGroup>
```

Or use the CLI from your application's directory:

```bash
dotnet add reference path/to/NumeralSystems.Net/NumeralSystems.Net.csproj
```

To create a package for a local feed:

```bash
dotnet pack NumeralSystems.Net/NumeralSystems.Net.csproj \
  --configuration Release \
  --output artifacts
```

The repository does not require a package installation for development.

## Create a numeral

```csharp
using NumeralSystems.Net;

var base16 = Numeral.System.OfBase(16);
var value = base16[4095];

Console.WriteLine(value);          // FFF
Console.WriteLine(value.Integer);  // 4095
```

For a minimal-width result in bases whose default byte width would introduce
leading zeroes:

```csharp
var binary = Numeral.System.OfBase(2);
binary.AdjustToFitIntegralLength = false;

Console.WriteLine(binary[42]); // 101010
```

## Parse a value

The convenient overload uses the default serialization settings:

```csharp
var hex = Numeral.System.OfBase(16);
var parsed = hex.Parse("FF");

Console.WriteLine(parsed.Integer); // 255
```

Use the `ParseResult` overload when invalid input is expected:

```csharp
var decimalSystem = Numeral.System.OfBase(10);
var parsed = decimalSystem.TryParse(
    "-12.5",
    NumeralAlphabet.Base10);

Console.WriteLine(parsed.Success);       // True
Console.WriteLine(parsed.Value.Decimal); // -12.5
```

Failures include a reason and exact UTF-16 position:

```csharp
var invalid = decimalSystem.TryParse(
    "12X",
    NumeralAlphabet.Base10);

Console.WriteLine(invalid.Reason);   // UnknownSymbol
Console.WriteLine(invalid.Position); // 2
```

## Convert between systems

```csharp
var decimalSystem = Numeral.System.OfBase(10);
var hexadecimal = Numeral.System.OfBase(16);

var decimalValue = decimalSystem[255];
var hexValue = decimalValue.To(hexadecimal);

Console.WriteLine(hexValue); // FF
```

Conversion creates a new `Numeral`; the source object's base is unchanged.

## Calculate with values

`NumeralValue` performs arithmetic without converting through a bounded
primitive type:

```csharp
var left = NumeralValue.FromDecimal(10.5m);
var right = new NumeralValue(
    integral: new List<int> { 10 },
    decimals: new List<int> { 8 },
    negative: false,
    baseValue: 16);

var sum = left.Add(
    right,
    exact: out var exact,
    resultBase: 10);

Console.WriteLine(exact);           // True
Console.WriteLine(sum.ToDecimal()); // 21.0
```

The hexadecimal operand is `A.8`, which is decimal `10.5`. Arithmetic accepts
different operand bases. See [Arithmetic](arithmetic.md) for division,
precision, operators, and comparison.

## Common setup problems

### The value contains unexpected leading zeroes

Set `AdjustToFitIntegralLength` to `false` before creating or parsing the
numeral.

### Parsing changes between machines

The parameterless overload uses predefined/deterministic alphabets but takes
its sign and decimal separator from `CultureInfo.CurrentCulture`. Pass
`NumeralAlphabet` and explicit formatting tokens for deterministic text.

### A custom alphabet is rejected

The alphabet size must equal `NumeralSystem.Size`. Symbols must be non-empty,
unique, and prefix-free, and cannot conflict with separators or the negative
sign.

### A type name conflicts with `System`

Several wrapper types are named `Byte`, `Char`, `Decimal`, `Double`, `Float`,
`Int`, `Long`, `Short`, and `String`. Use an alias:

```csharp
using IntValue = NumeralSystems.Net.Type.Base.Int;

var value = new IntValue { Value = 42 };
```

Continue with:

- [Numeral systems](numeral-systems.md) for custom alphabets and conversion;
- [Arithmetic](arithmetic.md) for calculations and comparison;
- [Cookbook](cookbook.md) for task-oriented examples;
- [Troubleshooting](troubleshooting.md) for common errors.
