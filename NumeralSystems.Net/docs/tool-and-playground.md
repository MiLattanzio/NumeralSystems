# Global tool and WebAssembly playground

[Documentation home](index.md) ·
[Getting started](getting-started.md) ·
[BitPattern engine](bit-patterns.md) ·
[Exact rationals](exact-rationals.md)

Version 5.1 exposes the same ordered-alphabet, exact-rational, and unknown-bit
engines through a command-line tool and a browser-only playground. Neither
surface implements its own numeric conversion rules.

## Install `numsys`

The NuGet package ID is `dotnet-numeralsystems`; the installed command is
`numsys`:

```console
dotnet tool install --global dotnet-numeralsystems --version 5.1.0
numsys --help
```

Update or remove it with the normal .NET tool commands:

```console
dotnet tool update --global dotnet-numeralsystems
dotnet tool uninstall --global dotnet-numeralsystems
```

During repository development, run the project without installing it:

```console
dotnet run --project NumeralSystems.Net.Tool -- convert FF --from 16 --to 2
```

## Convert integers

```console
> numsys convert FF --from 16 --to 2
11111111
```

`convert` uses `NumeralAlphabet.CreateDefault` for both bases. Named predefined
alphabets therefore keep their documented order; other bases use deterministic
fixed-width decimal symbols. Values are decoded into `BigInteger`, so the
command is not limited to 32-bit or 64-bit integers.

## Inspect an unknown-bit value

```console
> numsys inspect 1100???? --type byte
Pattern: 1100????
Type: byte (8 bits)
Unknown bits: 4
Candidates: 16
Unsigned range: 192..207
Signed range: -64..-49
```

Supported type names are `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`,
`long`, and `ulong`, plus the equivalent `int16`, `uint16`, `int32`, `uint32`,
`int64`, and `uint64` aliases. The pattern must have exactly the selected
width. `_` and whitespace are ignored inside a pattern.

The command prints at most the first 16 candidates. The exact total is still a
`BigInteger`, so a 64-bit all-unknown pattern is safe to inspect without
accidentally enumerating `2^64` values.

## Solve an AND constraint

```console
> numsys solve "x & 10101010 = 10001000"
x = 1?0?1?0?
Candidates: 16
Unsigned range: 136..221
```

Both operands may contain `0`, `1`, or `?` and must have equal width. The
result is the most precise `BitPattern` describing every solution. A
contradiction prints `No solution.` and returns exit code 3.

Exit code 0 means success, 2 means invalid command/input, and 3 means a valid
constraint with no solution. Diagnostics go to standard error; successful
results go to standard output so conversion can be composed in scripts.

## Run the browser playground

```console
dotnet run --project NumeralSystems.Net.Playground
```

The standalone Blazor WebAssembly application contains three interactive views:

- an arbitrary-base `BigInteger` converter;
- an exact fraction explorer that highlights the repeating block and charts
  the period length of `1/d` for denominators 2 through 40;
- an unknown-bit visualizer with exact candidate count, signed/unsigned ranges,
  colored bits, and a bounded 16-value preview.

All calculations execute locally in the browser. There is no server API, font
CDN, telemetry endpoint, or JavaScript numeric reimplementation.

## Publish static files

```console
dotnet publish NumeralSystems.Net.Playground \
  --configuration Release \
  --output artifacts/playground
```

Serve the generated `artifacts/playground/wwwroot` directory from any static
host. `index.html` uses a relative base path and `staticwebapp.config.json`
provides a navigation fallback for hosts that understand that convention.

Every GitHub Release also includes a ready-to-host
`NumeralSystems.Net-playground-<version>.zip` asset.

## Resource limits

The fraction explorer caps generated digits at 2,048 and enables period
detection. The bit viewer always calls `EnumerateCandidates(16)`. These limits
are part of the UI behavior, not hidden changes to the core library defaults.
