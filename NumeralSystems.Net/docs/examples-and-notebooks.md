# Executable examples and notebooks

[Documentation home](index.md) ·
[Cookbook](cookbook.md) ·
[Global tool and playground](tool-and-playground.md)

Version 5.1 adds examples that can be executed, debugged, and kept in sync with
the source tree. The Markdown cookbook remains the fastest reference; this
project and the notebooks are useful when changing inputs interactively.

## Console examples

Run all topics:

```console
dotnet run --project NumeralSystems.Net.Examples -- all
```

Or select one topic:

```console
dotnet run --project NumeralSystems.Net.Examples -- exact
dotnet run --project NumeralSystems.Net.Examples -- periods
dotnet run --project NumeralSystems.Net.Examples -- bits
dotnet run --project NumeralSystems.Net.Examples -- json
```

| Topic | Demonstrates |
| --- | --- |
| `exact` | Exact `1/10` in base 2, repeating metadata, and terminating `1/3` in base 3 |
| `periods` | A compact text graph of decimal period lengths for `1/d` |
| `bits` | Parsing unknown bits, bounded metadata, and solving an AND constraint |
| `json` | Explicit converter registration and exact rational JSON round-trip |

The examples reference the core and JSON projects directly, so a local run
always tests the current checkout rather than a previously published package.

## Polyglot notebooks

The `examples/notebooks` directory contains two `.dib` notebooks:

- `ExactRationals.dib` explores exact expansion and fraction periods;
- `UnknownBits.dib` inspects a pattern and solves a mask constraint.

Open them with a .NET Interactive/Polyglot Notebooks-compatible editor. Each
notebook references the published `NumeralSystems.Net` package so it is also a
copyable consumer example. Before 5.1.0 is published, change the `#r "nuget:…"`
line to an available prerelease version or use the executable project instead.

## Keeping examples valid

`NumeralSystems.Net.Examples` is part of the solution and therefore compiled by
CI with warnings as errors. Notebook cells are intentionally short and mirror
the compiled examples; update both whenever a public API shown by a notebook
changes.
