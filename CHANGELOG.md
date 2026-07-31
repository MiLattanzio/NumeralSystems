# Changelog

All notable changes to NumeralSystems.Net are documented in this file.

The project follows [Semantic Versioning](https://semver.org/).

## [5.1.0] - 2026-08-01

### Added

- Add the `dotnet-numeralsystems` global tool with the `numsys convert`,
  `numsys inspect`, and `numsys solve` commands.
- Add `BitPattern.Parse` and `TryParse` for readable MSB-first `0`/`1`/`?`
  patterns with optional whitespace and `_` separators.
- Add a standalone Blazor WebAssembly playground with arbitrary-base integer
  conversion, exact fraction-period exploration and graphing, and a bounded
  unknown-bit candidate visualizer.
- Add the separately versioned `NumeralSystems.Net.Json` package with explicit,
  idempotent `JsonSerializerOptions.AddNumeralSystems()` registration.
- Add a compiled console examples project and educational .NET Interactive
  notebooks for exact rationals, periods, unknown bits, and constraints.
- Add CLI, playground, examples, notebook, JSON, and 5.1 migration guides.
- Add exhaustive CLI tests alongside the existing 256-combination byte tests
  and larger property-oriented bit-pattern tests.

### Changed

- Make `Numeral` state and primitive views read-only; immutable factories and
  replacement methods are now the only mutation path.
- Move `NumeralJsonConverter` out of the core assembly while retaining its
  namespace and its exact, backward-readable JSON shape.
- Extend the release pipeline to validate and publish the core, JSON, and tool
  packages, validate a WebAssembly publish, export BenchmarkDotNet Markdown and
  JSON results, and attach benchmark/playground archives to every release.
- Compile the playground and executable examples as part of the solution and
  CI warning-as-error build.

### Removed

- Remove every API marked obsolete in 5.0: mutating `Numeral` setters,
  `TrySetValue`, the implicit-options `Numeral.To` overload, the list-based
  `NumeralValue` constructor, legacy `ToBase` overloads, and `TryToBase`.
- Remove implicit JSON converter registration from `Numeral`.

## [5.0.0] - 2026-07-31

### Added

- Add immutable, normalized `RationalValue` state backed by a signed
  `BigInteger` numerator and positive `BigInteger` denominator.
- Add exact factories for integers, positional digits, `decimal`, IEEE 754
  `float`, and IEEE 754 `double` values.
- Add immutable `NumeralConversionOptions` with an explicit fractional digit
  limit, six rounding modes, period detection, and `Throw`, `Truncate`,
  `Round`, or `PreservePeriod` infinite-expansion policies.
- Add immutable `NumeralExpansion` results with termination, rounding,
  repeating-start, and repeating-length metadata, plus parenthesized period
  formatting through a `NumeralAlphabet`.
- Add `InfiniteNumeralExpansionException` and
  `NumeralExpansionLimitException` under the common
  `NumeralExpansionException` hierarchy.
- Add `NumeralValue.FromDigits`, `FromRational`, `Expand`, exact numerator and
  denominator properties, period metadata, and option-aware conversion and
  arithmetic overloads.
- Add `Numeral.ExactValue`, `FromRational`, `WithExactValue`, and option-aware
  base conversion.
- Add tests for normalization, exact requested examples, repeating binary
  one tenth, every rounding category, digit carry, expansion failures,
  immutability, exact IEEE values, large-ratio decimal conversion, and exact
  rational JSON round trips.
- Add dedicated exact-rational and 5.0 migration guides, with expanded README,
  arithmetic, architecture, and API documentation.

### Changed

- Make an exact rational value, rather than the currently materialized digits,
  authoritative for `NumeralValue` conversion, arithmetic, comparison, and
  primitive views.
- Preserve the original rational after a truncated or rounded digit
  projection, allowing a later conversion to recover an exact terminating
  representation in another base.
- Make `NumeralValue` sealed and keep all digit collections read-only.
- Return copies from `Numeral.IntegralIndices` and `FractionalIndices`; retain
  the 4.x mutation surface only as a warning-based migration layer.
- Serialize `Numeral` numerator and denominator as arbitrary-precision JSON
  strings on .NET 8 while continuing to accept 4.8 digit-only JSON.
- Convert large rational ratios to `decimal` without casting their numerator
  and denominator separately.
- Add nullable reference annotations to the new 5.0 value, options, expansion,
  exception, and conversion APIs.

### Deprecated

- Deprecate the list-based `NumeralValue` constructor in favor of
  `FromDigits` or `FromRational`.
- Deprecate implicit-precision `NumeralValue.ToBase` and `TryToBase` overloads
  in favor of `NumeralConversionOptions` and `NumeralExpansion` metadata.
- Deprecate `Numeral.To(NumeralSystem)`, mutating digit/sign/primitive setters,
  and `TrySetValue` in favor of immutable replacement APIs.

### Compatibility

- Keep `Polecola.Primitive` 1.0.0 unchanged; the exact-rational layer has no
  required changes in that package.
- Retain forwarding implementations for deprecated 4.x calls so existing
  source continues to compile with migration warnings.

## [4.8.1] - 2026-07-31

### Removed

- Remove every API previously marked `Obsolete`:
  `Value.FromString(string, HashSet<string>)`,
  `Value.FromString(string, bool)`, `NumeralSystems.Net.Encoding.String`, and
  the static character-transformation members of `Type.Base.String`.
- Remove obsolete-specific compatibility tests and warning suppressions.

### Changed

- Set the NuGet author metadata to `Mi Lattanzio`.
- Set the project security and conduct contact address to `mi@polecola.it`.
- Update the API reference, migration guides, README files, recipes, and
  troubleshooting guidance to expose only the explicit 4.8 API families.

## [4.8.0] - 2026-07-31

### Added

- Add explicit UTF-16 code-unit and Unicode `Rune` APIs through
  `CharacterIdentity`, `CharacterRadixTransform`, and `Value.FromRunes`.
- Add RFC-compatible byte-oriented `StandardBaseCodec` support for Base16,
  Base32, and Base64, including padded and unpadded decoding.
- Add constant-memory `Stream`/`TextReader`/`TextWriter` encoding and decoding
  for standard codecs and the experimental character transformation.
- Add immutable `NumeralFormatInfo` support through `IFormatProvider`, with
  general (`G`) and invariant round-trip (`R`) formats.
- Add `System.Text.Json` serialization for `Numeral` on .NET 8, preserving the
  base, sign, integral digits, fractional digits, and leading zeros.
- Add `Span`/`ReadOnlySpan` formatting, parsing, and codec overloads on .NET 8.
- Add RFC vectors, malformed-input checks, supplementary Unicode tests,
  streaming tests, provider tests, JSON tests, and Span tests.

### Changed

- Multi-target the package for .NET Standard 2.1 and .NET 8.0.
- Separate numeral representation (`NumeralAlphabet`), standard binary-to-text
  codecs (`StandardBaseCodec`), and the experimental character-radix transform
  (`CharacterRadixTransform`) into distinct APIs.
- Deprecate ambiguous legacy string entry points while retaining forwarding
  behavior for source and binary compatibility.
- Define empty character and byte inputs as successful empty transformations.
- Make `GetSmallestBase` return a base strictly greater than the maximum digit;
  empty input returns the minimum positional base, 2.
- Make the un-fitted UTF-16 `Value` base 65,536 so U+FFFF is representable.

### Fixed

- Reject unpaired UTF-16 surrogates in Rune-based operations.
- Validate standard Base16/Base32/Base64 final blocks, padding, symbols, and
  unused bits consistently in both in-memory and streaming paths.

## [4.7.0] - 2026-07-31

### Added

- Add ordered immutable `NumeralAlphabet` values with duplicate, prefix, and
  formatting-token conflict validation.
- Add predefined alphabets for bases 2, 8, 10, 16, 32, 36, 58, 62, and 64,
  plus deterministic fixed-width alphabets for other bases.
- Add exact signed `BigInteger` `Encode`/`Decode` round trips and ordered
  alphabet overloads across `Value`, `Numeral`, and `NumeralSystem`.
- Add structured `ParseResult` diagnostics with `ParseErrorReason`, UTF-16
  position, error length, and message.
- Add generated round-trip properties for bases 2 through 128 and pairwise
  conversion properties across every predefined base.
- Add the immutable `BitPattern` engine shared by every `Incomplete*` wrapper.
- Add exact `BigInteger` candidate counts, unsigned and two's-complement bounds,
  membership checks, and explicitly bounded candidate enumeration.
- Add three-valued masks, logical and arithmetic shifts, rotate-left and
  rotate-right operations.
- Add pattern compatibility, intersection, reverse XOR/NAND, and APIs for
  solving constraints such as `x & mask == result`.
- Add exhaustive coverage for every pair of byte values and every byte
  shift/rotation, plus deterministic property tests for 32-bit and 64-bit
  patterns.
- Add exact-rational `Add`, `Subtract`, `Multiply`, and `Divide` operations to
  `NumeralValue`, including overloads that report whether the destination
  expansion terminated within a configurable digit limit.
- Add binary `+`, `-`, `*`, and `/` operators, unary negation, `Negate`, `Abs`,
  and `IsZero`.
- Add base-independent numeric comparison through `CompareTo`,
  `NumericallyEquals`, and relational operators.
- Add BenchmarkDotNet coverage for same-base arithmetic, cross-base arithmetic,
  repeating division, and large-value comparison.
- Add dedicated guides for the `BitPattern` engine, arithmetic, recipes,
  troubleshooting, migration, and project architecture.
- Include the complete Markdown guide set, changelog, contribution policy,
  security policy, code of conduct, and license in the NuGet package.

### Changed

- Deprecate `Value.FromString(string, HashSet<string>)`; its compatibility path
  now sorts symbols ordinally instead of relying on set enumeration order.
- Make default serialization use predefined alphabets when available and
  deterministic fixed-width alphabets otherwise.
- Route nullable Boolean operations and legacy incomplete wrappers through the
  shared set-based ternary logic.
- Make `Binary` setters consistently public across incomplete wrapper types
  while retaining their existing members and construction patterns.
- Perform arithmetic with reduced arbitrary-precision rational intermediates,
  converting to a finite positional representation only for the final result.
- Expand both READMEs, the documentation index, getting-started guide, numeral
  guide, API reference, and cross-links between topic guides.

## [4.6.0] - 2026-07-31

### Added

- Add arbitrary-precision integer conversion to `Value`, `NumeralValue`,
  `Numeral`, and `NumeralSystem`.
- Add bounded fractional conversion through `NumeralValue.TryToBase` and the
  `ToBase` overload that accepts `maxFractionalDigits`.
- Add a precision-aware `Decimal.ToIndicesOfBase` overload that reports whether
  a fractional expansion terminated exactly.

### Changed

- Interpret fractional digits according to their declared positional base, so
  values such as `0.1` in base 2 correctly evaluate to one half.
- Convert `Numeral` instances directly through their digit representation,
  avoiding a lossy intermediate primitive value.
- Use an arbitrary-precision conversion core for integral `Value` conversions.

### Fixed

- Preserve the magnitude of negative `long`, `int`, `short`, and `sbyte`
  minimum values in `NumeralSystem` indexers.
- Preserve the source base in `NumeralValue.FromValue`.
- Preserve leading-zero width without adding an extra zero for all-zero values.
- Make signed numeral property setters update the stored sign.
- Respect the parsed negative sign in textual `TryIntegerOf` conversions.

## [4.5.2] - 2026-07-31

### Fixed

- Reject positional bases smaller than 2 across the public conversion APIs.
- Reject negative digits and digits outside the range of their numeral base.
- Preserve the current base when converting `NumeralValue` to `Value`.
- Preserve the sign while extracting digits from a negative `BigInteger`.
- Simplify incomplete-value permutation indexing and remove unreachable code.
- Repair malformed API documentation comments.

### Changed

- Make randomized tests deterministic and add validation regression tests.
- Move performance benchmarks from the NUnit suite to a dedicated BenchmarkDotNet project.
- Update the NUnit test adapter and test SDK dependencies.
- Produce repository metadata and symbol packages alongside the NuGet package.
- Treat compiler warnings as errors in the library, tests, and benchmarks.

[4.7.0]: https://github.com/MiLattanzio/NumeralSystems/compare/v4.6.0...v4.7.0
[4.6.0]: https://github.com/MiLattanzio/NumeralSystems/compare/v4.5.2...v4.6.0
[4.5.2]: https://github.com/MiLattanzio/NumeralSystems/releases/tag/v4.5.2
