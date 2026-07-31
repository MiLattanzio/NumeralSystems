# NumeralSystems.Net documentation

[Getting started](getting-started.md) ·
[Numeral systems](numeral-systems.md) ·
[Bitwise values](bitwise-values.md) ·
[Incomplete values](incomplete-values.md) ·
[String encoding](string-encoding.md) ·
[API reference](api-reference.md) ·
[Releasing](releasing.md)

NumeralSystems.Net is a .NET Standard 2.1 library for working with positional
number systems and bit-level representations. The repository documentation is
plain Markdown: every guide can be read directly on GitHub and changed without a
documentation generator.

## Choose the right abstraction

| Need | Start with |
| --- | --- |
| Convert and format a .NET number in another base | `NumeralSystem` and `Numeral` |
| Convert an existing sequence of non-negative digits | `Value` |
| Preserve a sign and fractional digits while changing base | `NumeralValue` |
| Run logical operations on primitive representations | `NumeralSystems.Net.Type.Base` |
| Represent or solve for unknown bits | `NumeralSystems.Net.Type.Incomplete` |
| Convert string character values into another positional base | `NumeralSystems.Net.Type.Base.String` |

## Minimal example

```csharp
using NumeralSystems.Net;

var hexadecimal = Numeral.System.OfBase(16);
var numeral = hexadecimal[255];

Console.WriteLine(numeral);          // FF
Console.WriteLine(numeral.Integer);  // 255

var binary = numeral.To(Numeral.System.OfBase(2));
Console.WriteLine(binary.Integer);   // 255
```

`Numeral` owns a reference to its `NumeralSystem`, an integral digit list, an
optional fractional digit list, and a sign. A digit is stored as an integer
index from `0` through `base - 1`.

## Important defaults

- `NumeralSystem.AdjustToFitIntegralLength` defaults to `true`. New numerals can
  be padded to the system's byte-oriented `Length`. Set it to `false` for a
  minimal-width representation.
- Parameterless `Parse` and `ToString` use a generated alphabet and the current
  culture's sign and decimal separator.
- For stable storage or a wire format, always pass an explicit alphabet,
  digit separator, negative sign, and decimal separator.
- Primitive-wrapper `Binary` arrays are indexed from the least-significant bit.
- Incomplete values use `bool?`: `false` means zero, `true` means one, and
  `null` means unknown.

## Guide map

- [Getting started](getting-started.md) covers cloning, building, testing, and
  referencing the project.
- [Numeral systems](numeral-systems.md) covers bases, alphabets, parsing,
  formatting, and conversion.
- [Bitwise values](bitwise-values.md) covers primitive wrappers and logical
  operations.
- [Incomplete values](incomplete-values.md) covers unknown bits, candidate
  enumeration, and reverse `AND`/`OR`.
- [String encoding](string-encoding.md) explains the two string-related APIs and
  their constraints.
- [API reference](api-reference.md) catalogs the public namespaces, types, and
  common member families.
- [Releasing](releasing.md) documents package versioning and automated
  publication to NuGet.org.

## Project status

The solution is a community-maintained library. Consult the test suite for
additional executable examples and verify edge cases that are important to your
application, especially precision limits for repeating fractions and very large
candidate sets.
