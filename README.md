# NumeralSystems.Net

[![Build](https://github.com/MiLattanzio/NumeralSystems/actions/workflows/dotnet.yml/badge.svg)](https://github.com/MiLattanzio/NumeralSystems/actions/workflows/dotnet.yml)
[![Licenza: MIT](https://img.shields.io/badge/licenza-MIT-blue.svg)](LICENSE.txt)
[![Playground online](https://img.shields.io/badge/playground-online-2ea44f.svg)](https://milattanzio.github.io/NumeralSystems/)

[English](README.en.md) · Italiano

**Prova senza installare nulla:** [apri il playground online](https://milattanzio.github.io/NumeralSystems/)
oppure [esplora gli esempi interattivi](https://milattanzio.github.io/NumeralSystems/docs/).

NumeralSystems.Net è una libreria .NET per rappresentare, convertire e formattare
valori in sistemi di numerazione arbitrari ed eseguire aritmetica razionale tra
basi diverse. Include inoltre tipi primitivi orientati ai bit, valori con bit
indeterminati e operazioni logiche inverse.

La libreria è adatta quando serve:

- convertire numeri interi o frazionari tra basi diverse;
- calcolare e confrontare valori con segno scritti in basi differenti;
- usare alfabeti ordinati, validati e immutabili;
- ottenere errori di parsing strutturati con posizione UTF-16 esatta;
- codificare byte con Base16, Base32 o Base64 standard, anche in streaming;
- elaborare esplicitamente unità UTF-16 o valori scalari Unicode;
- formattare tramite `IFormatProvider` e serializzare numerali esatti con il
  pacchetto opzionale `NumeralSystems.Net.Json`;
- ispezionare e modificare la rappresentazione binaria dei tipi primitivi;
- descrivere valori parziali con bit `0`, `1` o sconosciuti;
- ricavare i possibili operandi di `AND`, `OR`, `XOR` e `NAND`;
- combinare vincoli sui bit, applicare maschere ed enumerare candidati con un
  limite esplicito.

## Requisiti

- .NET 8 SDK per compilare la soluzione ed eseguire i test;
- un runtime compatibile con .NET Standard 2.1 per l'API portabile;
- .NET 8 per Rune, Span, playground WebAssembly e `NumeralSystems.Net.Json`.

Il repository contiene libreria, pacchetto JSON, tool globale, playground,
esempi, benchmark e suite NUnit. I tre pacchetti distribuibili vengono prodotti
e pubblicati automaticamente da una GitHub Release valida.

## Avvio rapido

```bash
git clone https://github.com/MiLattanzio/NumeralSystems.git
cd NumeralSystems/NumeralSystems.Net
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
```

Installazione da NuGet e uso del tool globale:

```bash
dotnet add package NumeralSystems.Net --version 5.3.0
dotnet add package NumeralSystems.Net.Json --version 5.3.0
dotnet tool install --global dotnet-numeralsystems --version 5.3.0

numsys convert FF --from 16 --to 2
numsys inspect "1100????" --type byte
numsys solve "x & 10101010 = 10001000"
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
| Alfabeti ordinati | `NumeralAlphabet`, `ParseResult` | Codifica deterministica, validazione e diagnostica del parsing |
| Cifre non negative | `Value` | Sequenze intere, inclusi valori a precisione arbitraria |
| Valori razionali esatti | `RationalValue`, `NumeralValue` | Conservazione `BigInteger/BigInteger` e proiezione in qualsiasi base |
| Politica di espansione | `NumeralConversionOptions`, `NumeralExpansion` | Limite cifre, arrotondamento, rifiuto dell'infinito o periodo esplicito |
| Primitive bitwise | `Type.Base.*` | Wrapper per byte, interi, caratteri e numeri floating point |
| Bit indeterminati | `BitPattern`, `BitConstraint`, `BitConstraintSet` | Pattern ternari, vincoli componibili, spiegazioni ed enumerazione limitata |
| Codec standard | `StandardBaseCodec` | Base16/Base32/Base64 RFC con API in memoria, Span e streaming |
| Caratteri | `CharacterIdentity`, `CharacterRadixTransform` | Identità UTF-16/Rune e trasformazioni sperimentali esplicite |
| Formattazione | `NumeralFormatInfo` | Provider, formati `G`/`R` e Span |
| JSON opzionale | `NumeralSystems.Net.Json`, `NumeralJsonConverter` | Registrazione esplicita e JSON strutturato esatto |

### Alfabeto personalizzato

Ogni posizione di `identity` è una cifra. L'alfabeto deve contenere almeno tante
voci quante sono le cifre della base.

```csharp
using NumeralSystems.Net;

var dozenal = Numeral.System.OfBase(12);
dozenal.AdjustToFitIntegralLength = false;

var alphabet = new NumeralAlphabet(
    "0123456789XY".Select(character => character.ToString()));

var value = dozenal[143];
var text = value.ToString(alphabet, separator: "", negativeSign: "-", numberDecimalSeparator: ".");

Console.WriteLine(text); // YY
Console.WriteLine(dozenal.Parse(text, alphabet, "", "-", ".").Integer); // 143
```

`NumeralAlphabet` rifiuta simboli duplicati, vuoti o con prefissi ambigui e
conflitti con separatori e segni.

### Round-trip esatti degli alfabeti

```csharp
BigInteger value = BigInteger.Pow(2, 256) + 42;
var text = NumeralAlphabet.Base62.Encode(value);
var decoded = NumeralAlphabet.Base62.Decode(text);

Console.WriteLine(decoded == value); // True
```

Sono disponibili alfabeti predefiniti per le basi 2, 8, 10, 16, 32, 36, 58,
62 e 64. Il parsing strutturato restituisce `ParseResult` con `Reason`,
`Position`, `ErrorLength` e `Message`.

### Codec standard e unità Unicode

I codec byte standard sono separati dagli alfabeti numerici:

```csharp
using NumeralSystems.Net.Encoding;

var encoded = StandardBaseCodec.EncodeBase64(bytes);
var decoded = StandardBaseCodec.DecodeBase64(encoded);
```

Per la trasformazione sperimentale dei caratteri occorre scegliere l'unità:
`EncodeUtf16` conserva i singoli `char`, mentre su .NET 8 `EncodeRunes` tratta
i caratteri supplementari come singoli valori scalari Unicode. Sono disponibili
anche API streaming a memoria costante.

`Numeral` implementa `IFormattable` con formati `G` dipendente dal provider e
`R` invariante. Il target .NET 8 aggiunge overload Span. La serializzazione
`System.Text.Json` esatta vive nel pacchetto separato `NumeralSystems.Net.Json`
ed è attivata esplicitamente con `options.AddNumeralSystems()`.

### Tool e playground

`numsys` rende disponibili conversione, ispezione limitata dei candidati e
composizione di vincoli AND, OR, XOR e NAND dalla shell. Il progetto
`NumeralSystems.Net.Playground` è un'app Blazor WebAssembly senza backend con
convertitore, grafico dei periodi, visualizzatore dei bit sconosciuti e solver
con spiegazioni bit per bit. La 5.3 aggiunge input da pipeline/file, output JSON
e `--explain` alla CLI; il playground offre link condivisibili, copia risultati,
download JSON ed esportazione SVG/CSV:

```bash
dotnet run --project NumeralSystems.Net/NumeralSystems.Net.Playground
dotnet run --project NumeralSystems.Net/NumeralSystems.Net.Examples -- all

Get-Content values.txt | numsys convert --from 16 --to 2
numsys --output json solve "x & 10101010 = 10001000" --explain
```

Il [playground pubblico](https://milattanzio.github.io/NumeralSystems/) e gli
[esempi interattivi](https://milattanzio.github.io/NumeralSystems/docs/) vengono
distribuiti automaticamente su GitHub Pages dopo i test di `master`.

### Aritmetica tra basi diverse

`NumeralValue` conserva internamente un razionale normalizzato esatto. Le cifre
posizionali sono una proiezione immutabile: una visualizzazione troncata non
degrada i calcoli successivi.

```csharp
var metaBinaria = NumeralValue.FromRational(1, 2, baseValue: 2);

var quartoDecimale = NumeralValue.FromDecimal(0.25m);
var somma = metaBinaria.Add(
    quartoDecimale,
    NumeralConversionOptions.Default,
    resultBase: 2);

Console.WriteLine(somma.Base);        // 2
Console.WriteLine(somma.ToDecimal()); // 0.75
```

`NumeralConversionOptions` rende espliciti limite di cifre, arrotondamento,
rilevamento del periodo e comportamento per espansioni infinite. Per esempio
`0.1` decimale diventa esattamente `0.0(0011)` in base 2, mentre `1/3` termina
come `0.1` in base 3. Gli operatori `+`, `-`, `*` e `/` usano la base
dell'operando sinistro e conservano lo stato razionale esatto.

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

var constraints = BitConstraintSet.Parse(
    "x & 10101010 = 10001000; " +
    "x | 00001111 = 10001111");
var solution = constraints.Solve(new BitConstraintSolverOptions(
    maximumEnumeratedCandidates: 4,
    timeout: TimeSpan.FromSeconds(1)));

Console.WriteLine(solution.GetPatternOrThrow()); // 10001?0?
Console.WriteLine(solution.CandidateCount);      // 4
foreach (var explanation in solution.Explanations)
    Console.WriteLine(explanation.Message);
```

Il motore comprende anche compatibilità e intersezione, reverse XOR/NAND, shift
logici e aritmetici, rotate-left/right e maschere con logica ternaria.

## Documentazione

La guida completa si trova in [`NumeralSystems.Net/docs`](NumeralSystems.Net/docs/index.md):

- [avvio e integrazione](NumeralSystems.Net/docs/getting-started.md);
- [sistemi numerici e alfabeti](NumeralSystems.Net/docs/numeral-systems.md);
- [alfabeti ordinati, preset e diagnostica del parsing](NumeralSystems.Net/docs/numeral-alphabets.md);
- [provider di formattazione, Span e JSON](NumeralSystems.Net/docs/formatting-and-serialization.md);
- [tool globale e playground WebAssembly](NumeralSystems.Net/docs/tool-and-playground.md);
- [ricette del playground, link condivisibili ed export](NumeralSystems.Net/docs/playground-recipes.md);
- [esempi eseguibili e notebook](NumeralSystems.Net/docs/examples-and-notebooks.md);
- [aritmetica, precisione, operatori e confronto](NumeralSystems.Net/docs/arithmetic.md);
- [razionali esatti, periodi e arrotondamento](NumeralSystems.Net/docs/exact-rationals.md);
- [ricettario con esempi pratici](NumeralSystems.Net/docs/cookbook.md);
- [primitive e operazioni bitwise](NumeralSystems.Net/docs/bitwise-values.md);
- [motore immutabile BitPattern e risoluzione dei vincoli](NumeralSystems.Net/docs/bit-patterns.md);
- [vincoli bitwise componibili, spiegazioni e limiti](NumeralSystems.Net/docs/bit-constraints.md);
- [valori incompleti e operazioni inverse](NumeralSystems.Net/docs/incomplete-values.md);
- [codifica delle stringhe](NumeralSystems.Net/docs/string-encoding.md);
- [risoluzione dei problemi](NumeralSystems.Net/docs/troubleshooting.md);
- [riferimento API](NumeralSystems.Net/docs/api-reference.md);
- [architettura e note per i contributori](NumeralSystems.Net/docs/architecture.md);
- [migrazione alla 4.7.0](NumeralSystems.Net/docs/migration-4.7.md);
- [migrazione alla 4.8.0](NumeralSystems.Net/docs/migration-4.8.md);
- [migrazione alla 4.8.1](NumeralSystems.Net/docs/migration-4.8.1.md);
- [migrazione dalla 4.8.1 alla 5.0.0](NumeralSystems.Net/docs/migration-5.0.md);
- [migrazione dalla 5.0.0 alla 5.1.0](NumeralSystems.Net/docs/migration-5.1.md);
- [migrazione dalla 5.1.0 alla 5.2.0](NumeralSystems.Net/docs/migration-5.2.md);
- [migrazione dalla 5.2.0 alla 5.3.0](NumeralSystems.Net/docs/migration-5.3.md);
- [processo di release e pubblicazione NuGet](NumeralSystems.Net/docs/releasing.md).

Tutta la documentazione è scritta in Markdown e viene versionata insieme al
codice. Non sono necessari generatori o tool aggiuntivi per leggerla e
modificarla.

## Benchmark

I benchmark prestazionali vivono in un progetto separato, così non influenzano
la scoperta o l'esecuzione dei test. Coprono formattazione, parsing, conversione,
aritmetica razionale, divisioni periodiche, confronto di grandi valori e motore
dei vincoli:

```bash
dotnet run --configuration Release \
  --project NumeralSystems.Net.Benchmarks/NumeralSystems.Net.Benchmarks.csproj
```

Ogni GitHub Release allega gli export Markdown/JSON completi dei benchmark e
un archivio statico pronto da distribuire del playground WebAssembly.

## Note importanti

- Una base posizionale deve essere maggiore o uguale a 2.
- Le cifre sono indici interi nell'intervallo `0..base-1`.
- Le cifre frazionarie hanno il significato posizionale della base dichiarata;
  lo stato razionale esatto sopravvive alle proiezioni periodiche, troncate e
  arrotondate.
- `NumeralConversionOptions` rende espliciti limite, arrotondamento,
  rilevamento del periodo e comportamento per espansioni infinite.
- Gli indexer e le viste `BigInteger` non hanno i limiti dei tipi interi primitivi.
- `Value` non memorizza segno o parte frazionaria; usare `NumeralValue` o
  `Numeral` quando servono.
- Gli array `Binary` dei wrapper primitivi sono indicizzati dal bit meno
  significativo; `ToString()` produce invece una vista leggibile.
- `NumeralAlphabet.Base64`, Base64 RFC standard e la trasformazione
  sperimentale dei caratteri sono API separate con modelli dati differenti.
- Rune e Span sono disponibili nel target .NET 8; UTF-16, provider, codec e
  streaming restano disponibili in .NET Standard 2.1. JSON è un pacchetto
  .NET 8 separato per non imporre dipendenze di serializzazione al core.

## Contribuire e sicurezza

Leggere [CONTRIBUTING.md](CONTRIBUTING.md) prima di aprire una pull request.
I problemi di sicurezza non devono essere segnalati in un'issue pubblica:
seguire [SECURITY.md](SECURITY.md).
Contatto privato del progetto: [mi@polecola.it](mailto:mi@polecola.it).

Il progetto adotta il [codice di condotta](CODE_OF_CONDUCT.md) ed è distribuito
con licenza [MIT](LICENSE.txt).
