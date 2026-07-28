# Getting started

[Documentation home](index.md) ·
[Numeral systems](numeral-systems.md) ·
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

Use `TryParse` when invalid input is expected. The explicit overload requires an
alphabet and formatting tokens:

```csharp
var decimalSystem = Numeral.System.OfBase(10);
var digits = "0123456789".Select(c => c.ToString()).ToList();

var success = decimalSystem.TryParse(
    value: "-12.5",
    identity: digits,
    separator: "",
    negativeSign: "-",
    numberDecimalSeparator: ".",
    result: out var parsed);

Console.WriteLine(success);       // True
Console.WriteLine(parsed.Decimal); // -12.5
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

## Common setup problems

### The value contains unexpected leading zeroes

Set `AdjustToFitIntegralLength` to `false` before creating or parsing the
numeral.

### Parsing changes between machines

The parameterless overload uses `CultureInfo.CurrentCulture`. Use the explicit
serialization overload for deterministic text.

### A custom alphabet is rejected

The alphabet must contain at least `NumeralSystem.Size` entries. Each digit in
the input must map to exactly one entry.

### A type name conflicts with `System`

Several wrapper types are named `Byte`, `Char`, `Decimal`, `Double`, `Float`,
`Int`, `Long`, `Short`, and `String`. Use an alias:

```csharp
using IntValue = NumeralSystems.Net.Type.Base.Int;

var value = new IntValue { Value = 42 };
```

Continue with [Numeral systems](numeral-systems.md) for custom alphabets and
conversion behavior.
