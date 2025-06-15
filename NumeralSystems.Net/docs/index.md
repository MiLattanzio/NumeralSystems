# NumeralSystems.Net

NumeralSystems.Net is a .NET library for working with values in arbitrary numeral systems. It provides types to parse, convert and operate on numbers represented with custom bases.

## Example

The snippet below converts a decimal string to a hexadecimal representation using the `Value` class.

```csharp
using NumeralSystems.Net;

var decimalValue = Value.FromString("255", baseIndices: new HashSet<string>("0123456789".Select(c => c.ToString())));
var hexValue = decimalValue.ToBase(16);
Console.WriteLine(string.Join("", hexValue.Indices.Select(i => i.ToString("X")))); // prints FF
```
