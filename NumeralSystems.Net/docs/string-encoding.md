# String encoding

[Documentation home](index.md) ·
[Numeral systems](numeral-systems.md) ·
[Cookbook](cookbook.md) ·
[Troubleshooting](troubleshooting.md) ·
[API reference](api-reference.md)

NumeralSystems.Net exposes two unrelated types named `String`. Use aliases to
make the intent clear:

```csharp
using IdentityBuilder = NumeralSystems.Net.Encoding.String;
using BaseString = NumeralSystems.Net.Type.Base.String;
```

## Extract an identity

`NumeralSystems.Net.Encoding.String.GetIdentity` returns the distinct
characters in first-occurrence order:

```csharp
var builder = new IdentityBuilder();
var identity = builder.GetIdentity("Hello World");

Console.WriteLine(string.Concat(identity)); // Helo Wrd
```

This can be used to inspect an input alphabet. The returned identity contains
`char` values, while `NumeralSystem.Parse` expects `IList<string>`:

```csharp
var symbols = identity.Select(c => c.ToString()).ToList();
```

An identity derived from one message is not a stable interchange format unless
it is stored or agreed on separately.

## The mutable string wrapper

`NumeralSystems.Net.Type.Base.String` implements
`IList<NumeralSystems.Net.Type.Base.Char>`:

```csharp
var value = new BaseString("Hello");

Console.WriteLine(value.Count);    // 5
Console.WriteLine(value[0].Value); // H

value.Add(new NumeralSystems.Net.Type.Base.Char { Value = '!' });
Console.WriteLine(value); // Hello!
```

It supports the normal mutable list operations plus `ToString()` and
`ToString(format)`.

## Encode characters into another base

The static partial `Base.String` API can convert each UTF-16 code unit into a
fixed-width sequence of digits in another base:

```csharp
var encoded = BaseString.EncodeToBase(
    s: "Hello",
    destinationBase: 64,
    size: out var width);

var decoded = BaseString.DecodeFromBase(
    s: encoded,
    sourceBase: 64,
    size: width);

Console.WriteLine(decoded); // Hello
```

`width` is the number of base digits allocated to every input character. It is
required for decoding and must be stored with the encoded value.

### This is not Base64

The encoded string stores digit values directly as UTF-16 characters. It can
therefore contain nulls, control characters, separators, or characters that are
altered by text transports. Do not send the result through systems that assume
printable Unicode without an additional binary-safe encoding.

If you need standard Base64, use `System.Convert.ToBase64String`.

## Work with digit arrays directly

Use the lower-level methods when a textual container is inappropriate:

```csharp
var digits = BaseString.ToIndicesOfBase("Hi", destinationBase: 16).ToArray();
var decoded = BaseString.FromIndicesOfBase(digits, sourceBase: 16);
```

Each element in `digits` corresponds to one UTF-16 code unit.

## Constraints

- The implementation accepts bases up to `char.MaxValue` (65,535).
- Use a positional base of at least 2.
- `DecodeFromBase` needs the exact base and width used by `EncodeToBase`.
- The API operates on UTF-16 `char` values, not Unicode scalar values. A
  supplementary character is encoded as its two surrogate code units.
- `GetSmallestBase` reports the largest character value encountered by the
  current implementation. A positional base must be greater than every digit,
  so validate or increment that result before using it as a base.
- Empty input is not accepted by `EncodeToBase` because it computes the maximum
  encoded width.
