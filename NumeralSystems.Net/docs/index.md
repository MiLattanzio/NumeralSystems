# NumeralSystems.Net documentation

[Exact rational values and positional expansions](exact-rationals.md)

[Getting started](getting-started.md) ·
[Numeral systems](numeral-systems.md) ·
[Numeral alphabets](numeral-alphabets.md) ·
[Formatting and JSON](formatting-and-serialization.md) ·
[Arithmetic](arithmetic.md) ·
[Cookbook](cookbook.md) ·
[Bitwise values](bitwise-values.md) ·
[BitPattern engine](bit-patterns.md) ·
[Incomplete values](incomplete-values.md) ·
[String encoding](string-encoding.md) ·
[Tool and playground](tool-and-playground.md) ·
[Examples and notebooks](examples-and-notebooks.md) ·
[Troubleshooting](troubleshooting.md) ·
[API reference](api-reference.md) ·
[Architecture](architecture.md) ·
[Releasing](releasing.md)

NumeralSystems.Net targets .NET Standard 2.1 and .NET 8 for working with
positional number systems, text/binary encodings, cross-base rational
arithmetic, and bit-level representations.
The repository documentation is plain Markdown: every guide can be read
directly on GitHub and changed without a documentation generator.

## Choose the right abstraction

| Need | Start with |
| --- | --- |
| Convert and format a .NET number in another base | `NumeralSystem` and `Numeral` |
| Encode text with a stable ordered symbol mapping | `NumeralAlphabet` |
| Encode bytes with standard Base16, Base32, or Base64 | `StandardBaseCodec` |
| Process UTF-16 units or Unicode scalars explicitly | `CharacterIdentity` and `CharacterRadixTransform` |
| Convert an existing sequence of non-negative digits | `Value` |
| Preserve a sign and fractional digits while changing base | `NumeralValue` |
| Preserve an exact fraction and inspect a repeating expansion | `RationalValue`, `NumeralExpansion` |
| Choose limits, period detection, rounding, and infinite behavior | `NumeralConversionOptions` |
| Calculate or compare signed values in different bases | `NumeralValue` arithmetic |
| Run logical operations on primitive representations | `NumeralSystems.Net.Type.Base` |
| Represent or solve for unknown bits | `NumeralSystems.Net.Type.Incomplete` |
| Work with bounded, immutable unknown-bit sets | `BitPattern` |
| Format with culture/custom tokens | `NumeralFormatInfo` and `Numeral` |
| Serialize an exact numeral as JSON | `NumeralSystems.Net.Json` |
| Convert or inspect from a shell | the `numsys` global tool |
| Explore periods or unknown bits in a browser | the WebAssembly playground |

## Minimal example

```csharp
using NumeralSystems.Net;

var hexadecimal = Numeral.System.OfBase(16);
var numeral = hexadecimal[255];

Console.WriteLine(numeral);          // FF
Console.WriteLine(numeral.Integer);  // 255

var binary = numeral.To(
    Numeral.System.OfBase(2),
    NumeralConversionOptions.Default);
Console.WriteLine(binary.Integer);   // 255
```

`Numeral` owns a reference to its `NumeralSystem`, copied integral/fractional
digits, a sign, and an exact rational snapshot when built through the 5.0
factories. A digit is stored as an integer index from `0` through `base - 1`.

## Important defaults

- `NumeralSystem.AdjustToFitIntegralLength` defaults to `true`. New numerals can
  be padded to the system's byte-oriented `Length`. Set it to `false` for a
  minimal-width representation.
- Parameterless `Parse` and `ToString` use a generated alphabet and the current
  culture's sign and decimal separator.
- For stable storage or a wire format, always pass an explicit alphabet,
  digit separator, negative sign, and decimal separator.
- Exact-first fractional conversion generates at most 128 digits and preserves
  a detected period by default. It throws when an exact expansion cannot be
  completed inside the limit. Use an explicit `Truncate` or `Round` policy at
  display and protocol boundaries.
- Primitive-wrapper `Binary` arrays are indexed from the least-significant bit.
- Incomplete values use `bool?`: `false` means zero, `true` means one, and
  `null` means unknown.

## Guide map

- [Getting started](getting-started.md) covers cloning, building, testing, and
  referencing the project.
- [Numeral systems](numeral-systems.md) covers bases, alphabets, parsing,
  formatting, and conversion.
- [Numeral alphabets](numeral-alphabets.md) covers immutable symbol order,
  presets, validation, exact integer round trips, and structured parse errors.
- [Formatting and JSON](formatting-and-serialization.md) covers
  `IFormatProvider`, standard formats, Span APIs, JSON, and the target matrix.
- [Arithmetic](arithmetic.md) covers exact rational operations, result bases,
  bounded fractional expansions, operators, and numeric comparison.
- [Exact rationals](exact-rationals.md) covers normalized `BigInteger` ratios,
  terminating and repeating expansions, policies, rounding, and exact JSON.
- [Cookbook](cookbook.md) provides task-oriented recipes across the library.
- [Bitwise values](bitwise-values.md) covers primitive wrappers and logical
  operations.
- [BitPattern engine](bit-patterns.md) covers immutable patterns, exact
  candidate counts, bounded enumeration, shifts, rotations, intersections, and
  constraint solving.
- [Incomplete values](incomplete-values.md) covers unknown bits, candidate
  enumeration, wrapper compatibility, and reverse operations.
- [String encoding](string-encoding.md) separates numeral text, RFC byte
  codecs, UTF-16 code units, Unicode Runes, and streaming.
- [Global tool and WebAssembly playground](tool-and-playground.md) documents
  `numsys`, its exit codes, interactive visualizations, and static publishing.
- [Executable examples and notebooks](examples-and-notebooks.md) maps runnable
  console topics and educational `.dib` notebooks.
- [Troubleshooting](troubleshooting.md) maps common symptoms and exceptions to
  concrete fixes.
- [API reference](api-reference.md) catalogs the public namespaces, types, and
  common member families.
- [Architecture](architecture.md) explains internal layers, mathematical flow,
  tests, benchmarks, and contributor expectations.
- [Migrating to 4.7.0](migration-4.7.md) covers fractional behavior,
  `BigInteger`, arithmetic, comparison, and upgrade tests.
- [Migrating to 4.8.0](migration-4.8.md) covers explicit Unicode units,
  standard codecs, smallest-base behavior, providers, JSON, and multi-targeting.
- [Migrating to 4.8.1](migration-4.8.1.md) maps every removed compatibility
  API to its explicit ordered-alphabet, UTF-16, Rune, or byte-codec replacement.
- [Migrating to 5.0.0](migration-5.0.md) maps the warning-based 4.x layer to
  exact rational factories, immutable replacement, and explicit policies.
- [Migrating to 5.1.0](migration-5.1.md) covers removal of obsolete members,
  explicit JSON registration, and the new package/tool layout.
- [Releasing](releasing.md) documents package versioning and automated
  publication to NuGet.org.

## Project status

The solution is a community-maintained library. Consult the test suite for
additional executable examples and verify edge cases that are important to your
application, especially precision limits for repeating fractions and very large
candidate sets.
