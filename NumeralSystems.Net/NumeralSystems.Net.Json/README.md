# NumeralSystems.Net.Json

Explicit `System.Text.Json` integration for `NumeralSystems.Net` 5.3.

```csharp
using System.Text.Json;
using NumeralSystems.Net.Json;

var options = new JsonSerializerOptions().AddNumeralSystems();
var json = JsonSerializer.Serialize(numeral, options);
var restored = JsonSerializer.Deserialize<Numeral>(json, options);
```

The converter preserves the base, sign, exact arbitrary-precision numerator
and denominator, integral digits, fractional digits, and leading zeroes. It
also reads the digit-only JSON shape produced by NumeralSystems.Net 4.8.

The package targets .NET 8 and depends on the matching `NumeralSystems.Net`
package. The core package no longer references `System.Text.Json` and does not
register converters implicitly.
