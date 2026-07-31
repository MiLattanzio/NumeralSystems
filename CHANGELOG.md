# Changelog

All notable changes to NumeralSystems.Net are documented in this file.

The project follows [Semantic Versioning](https://semver.org/).

## [4.7.0] - 2026-07-31

### Added

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
