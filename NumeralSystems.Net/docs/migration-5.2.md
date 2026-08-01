# Migrating from 5.1.0 to 5.2.0

[Documentation home](index.md) ·
[Composable bit constraints](bit-constraints.md) ·
[Global tool and playground](tool-and-playground.md)

Version 5.2 is additive for library consumers. Existing `BitPattern`,
`Incomplete*`, numeral, JSON, and codec APIs remain available. The main change
is that applications no longer need to implement their own parser or combine
single reverse operations manually.

## Upgrade packages together

```console
dotnet add package NumeralSystems.Net --version 5.2.0
dotnet add package NumeralSystems.Net.Json --version 5.2.0
dotnet tool update --global dotnet-numeralsystems --version 5.2.0
```

The JSON package format is unchanged; its version stays aligned with the core
package.

## Replace one-off AND parsing

The 5.1 helper remains valid:

```csharp
BitPattern.TrySolveAnd(mask, result, out var pattern);
```

Use a `BitConstraint` when the expression itself, its operator, diagnostics, or
explanation must be preserved:

```csharp
var constraint = BitConstraint.Parse("x & 10101010 = 10001000");
var pattern = constraint.Solve();
```

## Compose rules through `BitConstraintSet`

Do not enumerate each rule and intersect concrete candidate lists. Parse or
construct an immutable set and solve it directly:

```csharp
var set = BitConstraintSet.Parse(
    "x & 10101010 = 10001000; " +
    "x | 00001111 = 10001111");
var result = set.Solve();
```

Mixed variable names or widths are rejected during set construction.

## Handle contradictions separately from invalid input

Malformed syntax produces a parse error. A well-formed but impossible system
produces `IsSatisfiable == false`, `Pattern == null`, and candidate count zero.
This distinction is useful in editors, APIs, and the CLI.

## Choose resource policy explicitly

Library defaults are bounded, but services and interactive clients should pass
their own `BitConstraintSolverOptions`. Candidate enumeration requires a limit
and is additionally capped by `MaximumEnumeratedCandidates`. Solving and
enumeration observe timeout and cancellation.

The 5.2 CLI accepts:

```console
numsys solve "x ^ 00110011 = 1100????" --explain
numsys solve "x & 10101010 = 10001000; x | 00001111 = 10001111" --limit 4 --timeout 1000
```

Existing `numsys solve "x & MASK = RESULT"` scripts remain valid. Exit code 4
is new and identifies a timeout; codes 0, 2, and 3 retain their 5.1 meanings.
