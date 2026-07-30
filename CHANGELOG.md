# Changelog

All notable changes to NumeralSystems.Net are documented in this file.

The project follows [Semantic Versioning](https://semver.org/).

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

[4.5.2]: https://github.com/MiLattanzio/NumeralSystems/releases/tag/v4.5.2
