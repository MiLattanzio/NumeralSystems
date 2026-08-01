# NumeralSystems.Net documentation

[Getting started](getting-started.md) ·
[Tool and playground](tool-and-playground.md) ·
[Playground recipes](playground-recipes.md) ·
[Troubleshooting](troubleshooting.md) ·
[API reference](api-reference.md)

NumeralSystems.Net targets .NET Standard 2.1 and .NET 8 for working with
positional number systems, text/binary encodings, cross-base rational
arithmetic, and bit-level representations.
The repository documentation is plain Markdown: every guide can be read
directly on GitHub and changed without a documentation generator.

> **Live resources:** open the [WebAssembly playground](https://milattanzio.github.io/NumeralSystems/)
> to experiment without installing .NET, or use the
> [interactive examples](https://milattanzio.github.io/NumeralSystems/docs/)
> to follow focused, runnable scenarios.

## Start by goal

| Goal | Recommended path |
| --- | --- |
| Evaluate the project without installing anything | [Open the live playground](https://milattanzio.github.io/NumeralSystems/) |
| Learn with focused browser examples | [Open the interactive documentation](https://milattanzio.github.io/NumeralSystems/docs/) |
| Build a first .NET integration | [Getting started](getting-started.md), then [Numeral systems](numeral-systems.md) |
| Automate conversions or constraint solving | [Global tool and playground](tool-and-playground.md) |
| Share or embed a prepared browser example | [Playground recipes](playground-recipes.md) |
| Work with exact fractions and repeating periods | [Exact rational values](exact-rationals.md) |
| Work with unknown bits or equations | [BitPattern](bit-patterns.md), then [composable constraints](bit-constraints.md) |
| Find a type or diagnose a failure | [API reference](api-reference.md) or [troubleshooting](troubleshooting.md) |

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
| Parse, compose, solve, and explain bitwise equations | `BitConstraint` and `BitConstraintSet` |
| Format with culture/custom tokens | `NumeralFormatInfo` and `Numeral` |
| Serialize an exact numeral as JSON | `NumeralSystems.Net.Json` |
| Convert, inspect, or automate from a shell/CI pipeline | the `numsys` global tool with text or JSON output |
| Explore, share, and export periods or unknown bits in a browser | the WebAssembly playground |

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
- [Composable bit constraints](bit-constraints.md) covers the shared grammar,
  AND/OR/XOR/NAND composition, contradictions, explanations, and resource limits.
- [Incomplete values](incomplete-values.md) covers unknown bits, candidate
  enumeration, wrapper compatibility, and reverse operations.
- [String encoding](string-encoding.md) separates numeral text, RFC byte
  codecs, UTF-16 code units, Unicode Runes, and streaming.
- [Global tool and WebAssembly playground](tool-and-playground.md) documents
  `numsys`, pipelines, JSON, exit codes, share links, exports, interactive
  visualizations, and GitHub Pages publishing.
- [Playground recipes and shareable links](playground-recipes.md) provides
  ready-to-open examples, the query-string contract, export semantics,
  embedding guidance, and local-development instructions.
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
- [Migrating to 5.2.0](migration-5.2.md) covers the composable constraint
  model, shared parser, resource options, and CLI additions.
- [Migrating to 5.3.0](migration-5.3.md) covers CLI pipelines and JSON,
  shareable playground state, exports, interactive documentation, and Pages.
- [Releasing](releasing.md) documents package versioning and automated
  publication to NuGet.org.

## Project status

The solution is a community-maintained library. Consult the test suite for
additional executable examples and verify edge cases that are important to your
application, especially precision limits for repeating fractions and very large
candidate sets.
