# NumeralSystems.Net

[![Build](https://github.com/MiLattanzio/NumeralSystems/actions/workflows/dotnet.yml/badge.svg)](https://github.com/MiLattanzio/NumeralSystems/actions/workflows/dotnet.yml)
[![Licenza: MIT](https://img.shields.io/badge/licenza-MIT-blue.svg)](LICENSE.txt)

[English](README.en.md) · Italiano

NumeralSystems.Net è una libreria .NET per rappresentare, convertire e formattare
valori in sistemi di numerazione arbitrari ed eseguire aritmetica razionale tra
basi diverse. Include inoltre tipi primitivi orientati ai bit, valori con bit
indeterminati e operazioni logiche inverse.

La libreria è adatta quando serve:

- convertire numeri interi o frazionari tra basi diverse;
- calcolare e confrontare valori con segno scritti in basi differenti;
- usare alfabeti personalizzati per rappresentare le cifre;
- ispezionare e modificare la rappresentazione binaria dei tipi primitivi;
- descrivere valori parziali con bit `0`, `1` o sconosciuti;
- ricavare i possibili operandi di `AND`, `OR`, `XOR` e `NAND`;
- combinare vincoli sui bit, applicare maschere ed enumerare candidati con un
  limite esplicito.

## Requisiti

- .NET 8 SDK per compilare la soluzione ed eseguire i test;
- un runtime compatibile con .NET Standard 2.1 per usare la libreria.

Il repository contiene il progetto della libreria e la suite NUnit. Un pacchetto
NuGet viene prodotto e pubblicato automaticamente quando viene pubblicata una
GitHub Release valida, ma non è necessario installarlo per provare il progetto.

## Avvio rapido

```bash
git clone https://github.com/MiLattanzio/NumeralSystems.git
cd NumeralSystems/NumeralSystems.Net
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
```

Per usare il progetto direttamente da un'altra soluzione:

```xml
<ItemGroup>
  <ProjectReference Include="percorso/al/NumeralSystems.Net.csproj" />
</ItemGroup>
```

## Primo esempio

`NumeralSystem` definisce una base; l'indicizzatore crea un `Numeral` a partire da
un valore .NET.

```csharp
using NumeralSystems.Net;

var hex = Numeral.System.OfBase(16);

var encoded = hex[255];
Console.WriteLine(encoded);          // FF
Console.WriteLine(encoded.Integer);  // 255

var parsed = hex.Parse("FF");
Console.WriteLine(parsed.Integer);   // 255
```

La serializzazione predefinita usa le impostazioni culturali correnti per segno
e separatore decimale. Per formati persistenti o protocolli, specificare
esplicitamente alfabeto e separatori.

## API principali

| Area | Tipi | Scopo |
| --- | --- | --- |
| Sistemi numerici | `NumeralSystem`, `Numeral` | Creazione, parsing, formattazione e conversione tra basi |
| Cifre non negative | `Value` | Sequenze intere, inclusi valori a precisione arbitraria |
| Valori con segno e frazioni | `NumeralValue` | Conversione, calcolo e confronto con precisione limitata e verificabile |
| Primitive bitwise | `Type.Base.*` | Wrapper per byte, interi, caratteri e numeri floating point |
| Bit indeterminati | `BitPattern`, `Type.Incomplete.*` | Pattern ternari, vincoli, operazioni inverse ed enumerazione limitata |
| Codifica | `Type.Base.String`, `Encoding.String` | Conversione di stringhe in cifre di un'altra base ed estrazione dell'alfabeto |

### Alfabeto personalizzato

Ogni posizione di `identity` è una cifra. L'alfabeto deve contenere almeno tante
voci quante sono le cifre della base.

```csharp
using NumeralSystems.Net;

var dozenal = Numeral.System.OfBase(12);
dozenal.AdjustToFitIntegralLength = false;

var digits = "0123456789XY"
    .Select(character => character.ToString())
    .ToList();

var value = dozenal[143];
var text = value.ToString(digits, separator: "", negativeSign: "-", numberDecimalSeparator: ".");

Console.WriteLine(text); // YY
Console.WriteLine(dozenal.Parse(text, digits, "", "-", ".").Integer); // 143
```

### Aritmetica tra basi diverse

`NumeralValue` calcola usando valori razionali intermedi esatti. Gli operandi
possono avere basi differenti:

```csharp
var metaBinaria = new NumeralValue(
    new List<int> { 0 },
    new List<int> { 1 },
    false,
    2);

var quartoDecimale = NumeralValue.FromDecimal(0.25m);
var somma = metaBinaria.Add(quartoDecimale, out var esatto);

Console.WriteLine(esatto);            // True
Console.WriteLine(somma.Base);        // 2
Console.WriteLine(somma.ToDecimal()); // 0.75
```

Gli operatori `+`, `-`, `*` e `/` usano la base dell'operando sinistro. I
metodi con precisione esplicita segnalano quando un'espansione periodica deve
essere troncata.

### Operazioni bitwise inverse

Le operazioni inverse restituiscono un valore incompleto perché più operandi
possono produrre lo stesso risultato.

```csharp
using IntValue = NumeralSystems.Net.Type.Base.Int;

var left = new IntValue { Value = 0b1100 };
var right = new IntValue { Value = 0b1010 };
var result = left.And(right);

if (result.ReverseAnd(right, out var possibleLeft))
{
    Console.WriteLine(result.Value);          // 8
    Console.WriteLine(possibleLeft.Contains(left)); // True
}
```

### Pattern immutabili e vincoli

`BitPattern` è il motore condiviso da tutti i wrapper `Incomplete*`. Conteggio
dei candidati e limiti codificati usano `BigInteger`; l'enumerazione richiede
sempre un limite esplicito:

```csharp
using NumeralSystems.Net.Type.Incomplete;

var mask = BitPattern.FromUnsigned(0b1111_0000, width: 8);
var required = BitPattern.FromUnsigned(0b1010_0000, width: 8);

if (BitPattern.TrySolveAnd(mask, required, out var input))
{
    Console.WriteLine(input);                // 1010????
    Console.WriteLine(input.CandidateCount); // 16

    foreach (var candidate in input.EnumerateCandidates(limit: 4))
        Console.WriteLine(candidate);
}
```

Il motore comprende anche compatibilità e intersezione, reverse XOR/NAND, shift
logici e aritmetici, rotate-left/right e maschere con logica ternaria.

## Documentazione

La guida completa si trova in [`NumeralSystems.Net/docs`](NumeralSystems.Net/docs/index.md):

- [avvio e integrazione](NumeralSystems.Net/docs/getting-started.md);
- [sistemi numerici e alfabeti](NumeralSystems.Net/docs/numeral-systems.md);
- [aritmetica, precisione, operatori e confronto](NumeralSystems.Net/docs/arithmetic.md);
- [ricettario con esempi pratici](NumeralSystems.Net/docs/cookbook.md);
- [primitive e operazioni bitwise](NumeralSystems.Net/docs/bitwise-values.md);
- [motore immutabile BitPattern e risoluzione dei vincoli](NumeralSystems.Net/docs/bit-patterns.md);
- [valori incompleti e operazioni inverse](NumeralSystems.Net/docs/incomplete-values.md);
- [codifica delle stringhe](NumeralSystems.Net/docs/string-encoding.md);
- [risoluzione dei problemi](NumeralSystems.Net/docs/troubleshooting.md);
- [riferimento API](NumeralSystems.Net/docs/api-reference.md);
- [architettura e note per i contributori](NumeralSystems.Net/docs/architecture.md);
- [migrazione alla 4.7.0](NumeralSystems.Net/docs/migration-4.7.md);
- [processo di release e pubblicazione NuGet](NumeralSystems.Net/docs/releasing.md).

Tutta la documentazione è scritta in Markdown e viene versionata insieme al
codice. Non sono necessari generatori o tool aggiuntivi per leggerla e
modificarla.

## Benchmark

I benchmark prestazionali vivono in un progetto separato, così non influenzano
la scoperta o l'esecuzione dei test. Coprono formattazione, parsing, conversione,
aritmetica razionale, divisioni periodiche e confronto di grandi valori:

```bash
dotnet run --configuration Release \
  --project NumeralSystems.Net.Benchmarks/NumeralSystems.Net.Benchmarks.csproj
```

## Note importanti

- Una base posizionale deve essere maggiore o uguale a 2.
- Le cifre sono indici interi nell'intervallo `0..base-1`.
- Le cifre frazionarie hanno il significato posizionale della base dichiarata;
  `TryToBase` segnala quando un'espansione periodica raggiunge il limite.
- Gli indexer e le viste `BigInteger` non hanno i limiti dei tipi interi primitivi.
- `Value` non memorizza segno o parte frazionaria; usare `NumeralValue` o
  `Numeral` quando servono.
- Gli array `Binary` dei wrapper primitivi sono indicizzati dal bit meno
  significativo; `ToString()` produce invece una vista leggibile.
- La codifica di `Type.Base.String` non è Base64 e può produrre caratteri di
  controllo. Conservare sempre base e larghezza insieme al testo codificato.

## Contribuire e sicurezza

Leggere [CONTRIBUTING.md](CONTRIBUTING.md) prima di aprire una pull request.
I problemi di sicurezza non devono essere segnalati in un'issue pubblica:
seguire [SECURITY.md](SECURITY.md).

Il progetto adotta il [codice di condotta](CODE_OF_CONDUCT.md) ed è distribuito
con licenza [MIT](LICENSE.txt).
