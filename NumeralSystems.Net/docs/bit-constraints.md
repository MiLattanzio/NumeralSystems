# Composable bit constraints

[Documentation home](index.md) ·
[BitPattern engine](bit-patterns.md) ·
[Global tool and playground](tool-and-playground.md) ·
[API reference](api-reference.md)

Version 5.2 adds an immutable constraint model over `BitPattern`. It solves
AND, OR, XOR, and NAND equations independently at every bit position, combines
multiple rules for the same variable, and returns an exact per-bit projection
without enumerating concrete values.

## Constraint grammar

A single expression has this form:

```text
variable operator operand = expected-result
```

The operator is `&`, `|`, `^`, or the case-insensitive word `nand`. Operands
and results use the normal MSB-first `BitPattern` grammar: `0`, `1`, `?`, `_`,
and whitespace. Both patterns must have the same width.

```text
x & 10101010 = 10001000
x ^ 00110011 = 1100????
x | 00001111 = 10101111
x nand 1111 = 0011
```

`BitConstraint.Parse` throws `FormatException` for malformed syntax.
`BitConstraint.TryParse` provides the conventional Boolean form. Use
`BitConstraintParser.Parse` when a UI or editor needs structured diagnostics:

```csharp
var parsed = BitConstraintParser.Parse("x ^ 101 = 11");
if (!parsed.IsSuccess)
{
    Console.WriteLine(parsed.ErrorReason);   // WidthMismatch
    Console.WriteLine(parsed.ErrorPosition); // UTF-16 position
    Console.WriteLine(parsed.Message);
}
```

## Solving one rule

`BitConstraint` is immutable. The variable name, operator, operand, expected
result, and width are fixed at construction.

```csharp
var constraint = BitConstraint.Parse("x ^ 00110011 = 1100????");
BitPattern solution = constraint.Solve();

Console.WriteLine(solution);                // 1111????
Console.WriteLine(solution.CandidateCount); // 16
```

`TrySolve` returns `false` when no value can satisfy the rule. `Solve` throws
`InvalidOperationException` in that case.

The solver uses set semantics for unknown bits. For example, if an operand bit
is `?`, a value of `x` is accepted when at least one permitted operand value
can produce an allowed result. A `?` result accepts either operation result.
Complete operands are preferable when the mask or protocol value is fixed.

## Composing rules

`BitConstraintSet` is an immutable collection. Every rule must target the same
variable, compared without case sensitivity, and use the same bit width.
Semicolons and line breaks separate expressions when parsing a set.

```csharp
var constraints = BitConstraintSet.Parse(
    "x & 10101010 = 10001000; " +
    "x | 00001111 = 10001111");

BitConstraintSolution solution = constraints.Solve();
Console.WriteLine(solution.GetPatternOrThrow()); // 10001?0?
Console.WriteLine(solution.CandidateCount);      // 4
```

`Add` returns a new set and leaves the original unchanged. The solution is
computed by intersecting the values allowed by every rule at each position.
Its complexity is proportional to `bit width × constraint count`, not to
`2^unknown bits`.

## Contradictions

A valid expression can still be impossible:

```csharp
var constraints = BitConstraintSet.Parse(
    "x ^ 0000 = 0000; " +
    "x | 0000 = 0001");
var solution = constraints.Solve();

Console.WriteLine(solution.IsSatisfiable); // False
Console.WriteLine(solution.Pattern is null); // True
Console.WriteLine(solution.CandidateCount); // 0
```

The solver does not throw for mathematical contradictions. It returns an
unsatisfiable `BitConstraintSolution`. `GetPatternOrThrow` is available when a
caller prefers exception-based control flow.

## Bit-by-bit explanations

`Explanations` contains one `BitConstraintBitExplanation` for every position,
ordered from least to most significant. Each entry reports:

- `BitIndex`;
- whether zero and one remain possible;
- `RequiredValue` when exactly one value is possible;
- `IsContradiction` when neither value is possible;
- the source constraints that forced the conclusion;
- a ready-to-display `Message`.

```csharp
foreach (var explanation in solution.Explanations.Reverse())
{
    Console.WriteLine($"bit {explanation.BitIndex}: {explanation.Message}");
}
```

Explanations are derived from the same truth-table evaluation as the returned
pattern. The CLI and WebAssembly playground do not maintain separate solver or
parser implementations.

## Explicit limits and timeout

Pass immutable `BitConstraintSolverOptions` at trust boundaries:

```csharp
var options = new BitConstraintSolverOptions(
    maximumConstraints: 128,
    maximumBitWidth: 4096,
    maximumEnumeratedCandidates: 32,
    timeout: TimeSpan.FromSeconds(1));

var solution = constraints.Solve(options, cancellationToken);
```

The options limit constraint count, bit width, candidate enumeration, and
elapsed time. Exceeding a size or enumeration limit throws
`BitConstraintLimitException`. Timeout throws
`BitConstraintTimeoutException`; cancellation uses the normal
`OperationCanceledException`.

Solving itself never enumerates candidates. If concrete values are needed,
the caller must provide a limit that is no greater than
`MaximumEnumeratedCandidates`:

```csharp
foreach (BigInteger candidate in solution.EnumerateCandidates(8, cancellationToken))
{
    Console.WriteLine(candidate);
}
```

The exact `CandidateCount` remains available as `BigInteger` even when no
candidates are enumerated.

## Representation boundary

Like `BitPattern`, this engine represents independent choices at each bit
position. It exactly models the supported bitwise equations because none of
the four operators couples different positions. It does not model arithmetic
carry, shifts inside an expression, equality between two variable bits, or
multiple variables. Such constraints require a richer SAT/SMT-style model and
are intentionally outside the 5.2 grammar.
