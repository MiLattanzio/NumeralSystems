# Migrating from 5.2.0 to 5.3.0

[Documentation home](index.md) ·
[Global tool and playground](tool-and-playground.md) ·
[Releasing](releasing.md)

Version 5.3 is additive for library consumers. The core numeral, rational, and
bit-constraint public APIs are source- and binary-compatible with 5.2. The
observable changes are in the global tool, playground, documentation, and
deployment workflow.

## Update aligned packages

```console
dotnet add package NumeralSystems.Net --version 5.3.0
dotnet add package NumeralSystems.Net.Json --version 5.3.0
dotnet tool update --global dotnet-numeralsystems --version 5.3.0
```

## CLI compatibility

Existing commands retain their text output and exit codes:

```console
numsys convert FF --from 16 --to 2
numsys inspect 1100???? --type byte
numsys solve "x & 10101010 = 10001000"
```

The `convert` command now reads redirected standard input when `VALUE` is
omitted. An interactive terminal still fails immediately instead of waiting for
input, because `Program` passes `Console.In` only when input is redirected.

Use `--input FILE` for explicit files and `--input -` for an explicit standard
input contract. Do not combine `VALUE` and `--input`.

## JSON contract

Add `--output json` anywhere in the argument list. Success and error documents
contain `schemaVersion`, `command`, `success`, and `exitCode`. Successful
documents place command data in `result`; errors place `code` and `message` in
`error` and are written to standard error.

Every `BigInteger`-backed value is encoded as a JSON string. Consumers should
parse these strings with an arbitrary-precision integer type instead of a
floating-point JSON number.

## Playground links and exports

Old playground URLs continue to open with defaults. New query parameters can
reproduce current inputs and resource limits. Share links contain no result
payload: the destination recomputes results locally with the same library.

JSON downloads include the complete workspace. Period graph SVG exports contain
the rendered chart; CSV exports contain the underlying data and are preferable
for analysis.

## Deployment

The release archive remains available, but it is no longer the only hosted
artifact. Configure **Settings > Pages > Source: GitHub Actions** once. A
successful `master` build then publishes both the playground root and the live
documentation under `/docs/`.
