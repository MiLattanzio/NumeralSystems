# dotnet-numeralsystems

Global .NET tool for NumeralSystems.Net.

```bash
dotnet tool install --global dotnet-numeralsystems

numsys convert FF --from 16 --to 2
numsys inspect 1100???? --type byte
numsys solve "x & 10101010 = 10001000"
```

`convert` uses the immutable predefined or generated alphabet for each base.
`inspect` reports unknown bits, exact candidate count, unsigned/signed ranges,
and a bounded candidate preview. `solve` currently accepts the exact constraint
form `x & mask = result` and returns an immutable `BitPattern` solution.
