# dotnet-numeralsystems

Global .NET tool for NumeralSystems.Net.

```bash
dotnet tool install --global dotnet-numeralsystems

numsys convert FF --from 16 --to 2
numsys inspect 1100???? --type byte
numsys solve "x & 10101010 = 10001000"
numsys solve "x & 10101010 = 10001000; x | 00001111 = 10001111" --explain --limit 4
```

`convert` uses the immutable predefined or generated alphabet for each base.
`inspect` reports unknown bits, exact candidate count, unsigned/signed ranges,
and a bounded candidate preview. `solve` accepts immutable AND, OR, XOR, and
NAND constraints, composes semicolon- or line-separated rules for one variable,
and returns an exact `BitPattern` without enumerating candidates. Candidate
preview and execution limits are explicit through `--limit` and `--timeout`.
