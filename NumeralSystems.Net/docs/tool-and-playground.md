# Global tool and WebAssembly playground

[Documentation home](index.md) ·
[Getting started](getting-started.md) ·
[BitPattern engine](bit-patterns.md) ·
[Exact rationals](exact-rationals.md)

Version 5.3 exposes the same ordered-alphabet, exact-rational, unknown-bit, and
composable constraint engines through a command-line tool and a browser-only
playground. Neither surface implements its own parser or numeric rules. The CLI
is suitable for pipelines and CI, while playground state and exports are
portable between browsers, documentation, and reports.

## Install `numsys`

The NuGet package ID is `dotnet-numeralsystems`; the installed command is
`numsys`:

```console
dotnet tool install --global dotnet-numeralsystems --version 5.3.0
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
alphabets keep their documented order; other bases use deterministic fixed-width
decimal symbols. Values are decoded into `BigInteger`, so the command is not
limited to 32-bit or 64-bit integers.

### Standard input and files

Omit the positional value to read one value per line from redirected standard
input. Empty and whitespace-only lines are ignored; every other line produces
exactly one output line:

```powershell
Get-Content values.txt | numsys convert --from 16 --to 2
```

```bash
numsys convert --from 16 --to 2 < values.txt
```

For scripts that should not depend on shell redirection, use an explicit file:

```console
numsys convert --input values.txt --from 16 --to 2
numsys convert --input - --from 16 --to 2
```

A positional value and `--input` cannot be combined. More than one positional
value is rejected so an accidentally split shell argument cannot silently
produce a different batch.

## Text, JSON, and explanations

`--output text|json` is global and may appear before or after the command. Text
is the default. JSON uses a stable versioned envelope:

```console
numsys --output json convert FF --from 16 --to 2
```

```json
{
  "schemaVersion": "1.0",
  "command": "convert",
  "success": true,
  "exitCode": 0,
  "result": {
    "fromBase": 16,
    "toBase": 2,
    "count": 1,
    "results": [
      { "input": "FF", "result": "11111111", "decimalValue": "255" }
    ]
  }
}
```

Candidate counts, ranges, candidate values, and other arbitrary-precision
integers are strings in JSON. This preserves exact values in JavaScript and in
tools whose JSON number type is an IEEE 754 `double`. Invalid input and timeout
diagnostics use the same envelope on standard error with `error.code` set to
`invalid_input` or `timeout`.

`--explain` is supported by all three commands:

- `convert` shows the exact decimal bridge used for the display;
- `inspect` identifies every fixed or unknown bit from MSB to LSB;
- `solve` reports the rules responsible for every forced, unknown, or
  contradictory position.

Do not enable `--explain` when downstream text processing expects one result
per line. JSON keeps explanations in structured properties.

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
accidentally enumerating `2^64` values. Add `--explain` to print one conclusion
for each bit.

## Solve and compose bit constraints

```console
> numsys solve "x & 10101010 = 10001000"
x = 1?0?1?0?
Candidates: 16
Unsigned range: 136..221
```

AND (`&`), OR (`|`), XOR (`^`), and NAND (`nand`) use one shared library
grammar. Both patterns may contain `0`, `1`, or `?` and must have equal width.
The result is the most precise `BitPattern` describing every solution.

Separate rules with semicolons inside one shell argument or with line breaks in
a file/UI:

```console
> numsys solve "x & 10101010 = 10001000; x | 00001111 = 10001111" --explain
x = 10001?0?
Candidates: 4
Unsigned range: 136..141
Explanation (MSB to LSB):
  bit    7: 1  The bit must be 1 because ...
```

`--limit COUNT` requests a concrete candidate preview and is capped at 10,000
values. `--timeout MS` sets the solving and enumeration timeout; the default is
5,000 milliseconds. Solving itself never enumerates candidates.

A contradiction prints `No solution.` and returns exit code 3. Add `--explain`
to identify the conflicting bit and source rules.

Exit code 0 means success, 2 means invalid command/input or an exceeded size
limit, 3 means a valid constraint with no solution, and 4 means timeout.
Diagnostics go to standard error; successful results go to standard output so
conversion can be composed in scripts.

## Run the browser playground

```console
dotnet run --project NumeralSystems.Net.Playground
```

The standalone Blazor WebAssembly application contains four interactive views:

- an arbitrary-base `BigInteger` converter;
- an exact fraction explorer that highlights the repeating block and charts
  the period length of `1/d` for denominators 2 through 40;
- an unknown-bit visualizer with exact candidate count, signed/unsigned ranges,
  colored bits, and a bounded 16-value preview;
- a composed constraint solver with the shared parser, exact solution pattern,
  contradiction state, bounded preview, and one explanation per bit.

Version 5.3 adds a portable workspace toolbar:

- **Copy share link** stores converter, fraction, pattern, constraints, limits,
  and timeout in the URL query string;
- **Copy results** writes a readable summary to the clipboard;
- **Download JSON** exports a complete versioned snapshot, representing
  arbitrary-precision values as strings;
- **Export SVG** downloads the rendered period chart as a standalone vector;
- **Export CSV** downloads denominator, period length, and terminating state.

Query parameters are optional and individually validated. Invalid numeric
limits fall back to safe defaults, while invalid domain input remains visible
as a normal panel error. The `embed` parameter (`convert`, `fraction`, `bits`,
or `constraints`) renders one focused panel for live documentation.

All calculations execute locally in the browser. There is no server API, font
CDN, telemetry endpoint, or JavaScript numeric reimplementation. JavaScript is
limited to clipboard and browser-download integration.

## Publish static files

```console
dotnet publish NumeralSystems.Net.Playground \
  --configuration Release \
  --output artifacts/playground
```

Serve the generated `artifacts/playground/wwwroot` directory from any static
host. `index.html` uses a relative base path, `.nojekyll` preserves Blazor's
`_framework` directory on GitHub Pages, and `staticwebapp.config.json` provides
a navigation fallback for hosts that understand that convention.

Every GitHub Release also includes a ready-to-host
`NumeralSystems.Net-playground-<version>.zip` asset.

The same output is deployed automatically to
[GitHub Pages](https://milattanzio.github.io/NumeralSystems/) after successful
build and tests on `master`, on a published release, or during manual release
recovery. Repository setup must select **GitHub Actions** as the Pages source.
The deployed [`docs/`](https://milattanzio.github.io/NumeralSystems/docs/)
page embeds one focused WebAssembly instance and lets readers switch among
conversion, repeating-fraction, unknown-bit, and composed-constraint examples.
Only one runtime is active at a time, keeping the page responsive.

## Resource limits

The fraction explorer caps generated digits at 2,048 and enables period
detection. The bit viewer always calls `EnumerateCandidates(16)`. The
constraint view accepts at most 64 rules of up to 1,024 bits, exposes a preview
limit from 0 through 256, and exposes a timeout from 1 through 5,000 ms. These
limits are part of the UI behavior, not hidden changes to the core defaults.
