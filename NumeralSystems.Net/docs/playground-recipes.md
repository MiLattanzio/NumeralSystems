# Playground recipes and shareable links

[Documentation home](index.md) ·
[Getting started](getting-started.md) ·
[Tool and playground](tool-and-playground.md) ·
[Composable bit constraints](bit-constraints.md)

The public [NumeralSystems.Net playground](https://milattanzio.github.io/NumeralSystems/)
runs entirely in the browser. It exposes the same numeral, rational,
`BitPattern`, and constraint engines as the library and `numsys`, without a
backend or a separate JavaScript implementation of the calculations.

## Ready-to-open examples

| Scenario | Open it | Expected result |
| --- | --- | --- |
| Convert hexadecimal `FF` to binary | [Converter](https://milattanzio.github.io/NumeralSystems/?value=FF&from=16&to=2&embed=convert) | `11111111` |
| Expand `1/7` in base 10 | [Fraction explorer](https://milattanzio.github.io/NumeralSystems/?numerator=1&denominator=7&fractionBase=10&embed=fraction) | repeating block `142857` |
| Inspect four unknown low bits | [Bit visualizer](https://milattanzio.github.io/NumeralSystems/?pattern=1100%3F%3F%3F%3F&embed=bits) | 16 candidates, `192` through `207` unsigned |
| Solve two constraints together | [Constraint solver](https://milattanzio.github.io/NumeralSystems/?constraints=x%20%26%2010101010%20%3D%2010001000%3B%20x%20%7C%2000001111%20%3D%2010001111&limit=16&timeout=250&embed=constraints) | pattern `10001?0?`, 4 candidates |

Remove the `embed` parameter from any example to display the complete
playground workspace instead of one focused panel.

## Create a share link

Set up one or more panels, choose safe preview and timeout limits, and select
**Copy share link**. The generated URL stores input state, not calculated
output. A recipient recomputes the results locally with the deployed library
version, which keeps links compact and makes calculation behavior explicit.

The share link preserves all current panels, even when an `embed` value focuses
the page on only one of them. Do not put confidential values in a share link:
query strings can be retained in browser history, bookmarks, proxy logs, and
screenshots.

## Query-string reference

| Parameter | Applies to | Accepted value |
| --- | --- | --- |
| `value` | Converter | Text encoded with the alphabet selected by `from` |
| `from` | Converter | Source base from 2 through 256 |
| `to` | Converter | Destination base from 2 through 256 |
| `numerator` | Fraction explorer | Signed integer numerator |
| `denominator` | Fraction explorer | Non-zero integer denominator |
| `fractionBase` | Fraction explorer and chart | Base from 2 through 64 |
| `pattern` | Unknown-bit visualizer | `0`, `1`, or `?`; spaces and `_` are ignored |
| `constraints` | Solver | One or more shared-grammar rules separated by `;` or a line break |
| `limit` | Solver | Candidate preview limit from 0 through 256 |
| `timeout` | Solver | Timeout from 1 through 5,000 milliseconds |
| `embed` | Layout | `convert`, `fraction`, `bits`, or `constraints` |

Unknown parameters are ignored. Invalid bounded numeric parameters fall back to
the playground defaults. Invalid numeral, fraction, pattern, or constraint
input stays visible and produces the same validation error as manually entered
text.

When constructing a URL outside the playground, percent-encode reserved
characters such as `&`, `?`, `=`, spaces, semicolons, and line breaks. Using
**Copy share link** is less error-prone because it performs this encoding.

## Copy and export results

- **Copy results** writes a readable summary of all panels to the clipboard.
- **Download JSON** produces a versioned snapshot containing inputs, derived
  results, limits, and errors. Arbitrary-precision integers are JSON strings so
  JavaScript consumers cannot lose precision.
- **Export SVG** downloads the fraction-period chart as a standalone vector
  image suitable for documentation and presentations.
- **Export CSV** downloads one row per denominator with its period length and
  terminating state.

JSON is the right export for reproducible tooling; CSV is convenient for data
analysis; SVG preserves the visual chart without rasterizing it. Exported files
are created locally and are not uploaded by the playground.

## Embed a focused example

The `embed` parameter hides the workspace header, toolbar, and unrelated
panels. A documentation site can host a focused live example with an iframe:

```html
<iframe
  src="https://milattanzio.github.io/NumeralSystems/?value=FF&amp;from=16&amp;to=2&amp;embed=convert"
  title="Convert FF from hexadecimal to binary"
  loading="lazy"
  width="100%"
  height="560">
</iframe>
```

Use a descriptive `title` and enough height for validation messages and result
details. The hosting site's Content Security Policy must permit frames from
`milattanzio.github.io`. The project's own
[interactive documentation](https://milattanzio.github.io/NumeralSystems/docs/)
uses this focused mode and activates only one WebAssembly instance at a time.

## Run and verify locally

From the solution directory:

```console
dotnet run --project NumeralSystems.Net.Playground
```

Open the local URL printed by ASP.NET Core. To verify the exact static files
used by GitHub Pages:

```console
dotnet publish NumeralSystems.Net.Playground \
  --configuration Release \
  --output artifacts/playground
```

Serve `artifacts/playground/wwwroot` from an HTTP server. WebAssembly resources
may not load correctly when `index.html` is opened directly through a `file:`
URL.

## Limits and trust boundary

The bit-pattern preview is fixed at 16 candidates. The constraint solver
accepts at most 64 constraints with a width up to 1,024 bits, previews at most
256 candidates, and enforces a timeout no greater than 5 seconds. Fractional
expansions generate at most 2,048 digits. These limits prevent a shared URL
from triggering accidental unbounded work.

All calculations occur after the WebAssembly application has loaded. No input
or result is sent to a NumeralSystems.Net server, and the application includes
no telemetry endpoint. Normal browser and static-host logging can still record
the requested URL, including its query string.
