# NumeralSystems.Net

NumeralSystems.Net is a .NET library that lets you work with values expressed in
arbitrary numeral systems. It provides types for parsing, converting and
manipulating numbers using custom bases. The library includes:

- **Value** and **NumeralValue**: store numeric data with a specified base and
  support base conversion.
- **Numeral** and **NumeralSystem**: represent full numeral systems and offer
  parsing utilities.
- Wrappers for primitives like `Byte`, `Char`, `Int`, `Long`, `Float` and
  `Double` with additional bitwise operations.
- Utility methods for counting permutations, combinations and grouping
  sequences.

To build or run the tests, move into the `NumeralSystems.Net` folder and use:

```bash
cd NumeralSystems.Net
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet restore
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet build
DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet test
```

Documentation can be generated with `docfx build`.
