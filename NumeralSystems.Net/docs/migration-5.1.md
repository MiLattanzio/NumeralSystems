# Migrating from 5.0.0 to 5.1.0

[Documentation home](index.md) ·
[API reference](api-reference.md) ·
[Formatting and JSON](formatting-and-serialization.md)

Version 5.1 removes the warning-based compatibility layer introduced in 5.0,
moves JSON integration into an optional package, and adds CLI/browser surfaces.
The exact rational model and persisted JSON shape are unchanged.

## Removed obsolete APIs

All members previously carrying `ObsoleteAttribute` have been deleted. The
library no longer emits migration warnings and reflection over its public API
finds no obsolete types or members.

| Removed 5.0 API | 5.1 replacement |
| --- | --- |
| setters on `Numeral.Positive`, digit collections, and primitive views | create a new value with `FromRational`, `FromRepresentation`, or `WithExactValue` |
| `Numeral.TrySetValue` | `WithExactValue` with explicit conversion options |
| `Numeral.To(NumeralSystem)` | `To(NumeralSystem, NumeralConversionOptions)` |
| public list-based `NumeralValue` constructor | `FromDigits` or `FromRational` |
| `NumeralValue.ToBase(int, bool)` and `ToBase(int, int, bool)` | `ToBase(int, NumeralConversionOptions)` |
| `NumeralValue.TryToBase` | choose an explicit infinite-expansion behavior and call `ToBase` |

The digit getters still return defensive copies. Mutating a returned list never
changes the numeral.

## Move JSON registration to its package

5.0 attached `NumeralJsonConverter` to `Numeral` through an attribute. In 5.1,
install and register the optional package explicitly:

```console
dotnet add package NumeralSystems.Net.Json --version 5.1.0
```

```csharp
using System.Text.Json;
using NumeralSystems.Net.Json;

var options = new JsonSerializerOptions().AddNumeralSystems();
var json = JsonSerializer.Serialize(numeral, options);
var restored = JsonSerializer.Deserialize<Numeral>(json, options);
```

The converter remains in namespace `NumeralSystems.Net.Serialization` for
applications that instantiate it directly. Existing 4.8 digit-only payloads
and 5.0 exact payloads remain readable.

Without explicit registration, `System.Text.Json` treats `Numeral` as a normal
object; do not rely on that incidental shape for storage or protocols.

## Install the global tool

```console
dotnet tool install --global dotnet-numeralsystems --version 5.1.0
numsys convert FF --from 16 --to 2
```

See [Global tool and WebAssembly playground](tool-and-playground.md) for command
syntax, exit codes, and static playground publication.

## Package and release layout

| Package/artifact | Role |
| --- | --- |
| `NumeralSystems.Net` | portable core library (`netstandard2.1`, `net8.0`) |
| `NumeralSystems.Net.Json` | optional .NET 8 `System.Text.Json` integration |
| `dotnet-numeralsystems` | .NET 8 global/local tool providing `numsys` |
| playground release ZIP | static Blazor WebAssembly site |
| benchmark release archive | Markdown/JSON BenchmarkDotNet results |

Projects using only core conversion or bit-pattern APIs need no new package.
