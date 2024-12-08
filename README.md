Il progetto `NumeralSystems.Net` contiene due classi principali che gestiscono valori numerici rappresentati in diversi sistemi numerici. Ecco una panoramica delle classi `Value` e `NumeralValue` e delle loro funzionalità.
#### Classe `Value`
La classe `Value` rappresenta un valore numerico in un sistema numerico specifico, con una base definita. Include:
- **Costruttore `Value(List<int> indices, int baseValue)`**: Inizializza un oggetto `Value` con una lista di indici e una base. Assicura che tutti gli indici siano validi per la base specificata, sollevando eccezioni se la base è zero o negativa o se gli indici non rientrano nell'intervallo consentito.
- **Metodo statico `FromString`**: Permette la creazione di un oggetto `Value` da una rappresentazione stringa utilizzando una base di caratteri specificata. Fornisce due overload: uno accetta un insieme di indici di base, mentre l'altro accetta un booleano per determinare se adattare il valore alla base minimale necessaria.
- **Metodo `ToBase`**: Converte il valore corrente in una base specificata, restituendo un nuovo oggetto `Value`.
- **Metodi Privati `DivideByBase` e `IsZero`**: Utilizzati per calcoli interni durante la conversione di basi, dividendo una lista di indici per una base e determinando se una lista di numeri rappresenta il valore zero rispettivamente.

#### Classe `NumeralValue`
La classe `NumeralValue` rappresenta valore numerico con separazione incorporata tra parte intera e decimale:
- **Proprietà**:
  - `Integral` e `Decimals`: Liste di lettura sola che rappresentano le parti intera e decimale del numero in un sistema numerico specificato.
  - `Negative` e `Base`: Indicatore della negatività del numero e base del sistema numerico rispettivamente.

- **Costruttore `NumeralValue(List<int> integral, List<int> decimals, bool negative, int baseValue)`**: Inizializza il valore numerico con parti definite e controlla la validità delle parti rispetto alla base.
- **Metodi Statici di Conversione (`FromDecimal`, `FromBigInteger`, ecc.)**: Convertiscono tipi numerici standard come `decimal`, `BigInteger`, `int`, `float`, e `double` in istanze di `NumeralValue`.
- **Metodi di Conversione (`ToBigInteger`, `ToDecimal`, `ToInt`, ecc.)**: Convertono un oggetto `NumeralValue` in rappresentazioni numeriche di sistemi standard.
- **Metodo `ToBase`**: Permette la conversione dell'attuale valore in una base differente, sia per la parte intera che per quella decimale, assicurando un'accurata rappresentazione del valore.
- **Metodi Privati `DivideByBase` e `IsZero`**: Funzioni di supporto per la conversione e verifica del valore zero analoghe a quelle nella classe `Value`.

### Conclusione
Entrambe le classi offrono un'ampia gamma di funzionalità per rappresentare, convertire e gestire numeri in basi diverse, supportando operazioni complesse e conversioni da e verso formati numerici standard. Troverete queste classi particolarmente utili quando lavorate con numeri in sistemi numerici alternativi, come i sistemi di numerazione posizionale.

## Classe `Byte`
La classe `Byte` rappresenta un singolo byte con varie operazioni binarie. Questa classe include funzionalità per manipolazioni bitwise, conversioni, e operazioni logiche.
### Principali Metodi e Proprietà:
- **Value**: La proprietà che contiene il valore del byte.
- **Binary**: Rappresentazione binaria del byte.
- **Bytes**: Rappresentazione come array di byte.
- **ReverseAnd/Or/Nand/Xor**: Metodi per eseguire operazioni logiche inverse e ottenere risultati in formato `IncompleteByte`.
- **Not, And, Or, Xor, Nand**: Operazioni logiche standard.
- **ToString()**: Converte il valore a una stringa di `1` e `0`.

## Classe `Char`
La classe `Char` rappresenta un carattere con operazioni logiche similmente complesse come nella classe `Byte`.
### Principali Metodi e Proprietà:
- **Value**: Valore del carattere.
- **Binary/Bytes**: Rappresentazioni binarie e byte.
- **ReverseAnd/Or**: Operazioni logiche inverse, che restituiscono un `IncompleteChar`.
- **Not, And, Or, Xor, Nand**: Operazioni logiche standard.
- **ToString()**: Formatta il carattere o la sua rappresentazione binaria in una stringa.

## Classe `Int`
Rappresenta un numero intero a 32-bit, fornendo varie operazioni binarie.
### Principali Metodi e Proprietà:
- **Value**: Valore dell’intero.
- **Binary/Bytes**: Conversioni tra rappresentazioni binarie e di array di byte.
- **ReverseAnd/Or**: Eseguono le operazioni AND/OR inverse, restituendo un `IncompleteInt`.
- **Not, And, Or, Xor, Nand**: Classiche operazioni bitwise.
- **ToString()**: Restituisce la rappresentazione del numero intero come stringa.

## Classe `Long`
Questa classe rappresenta un numero intero a 64-bit con operazioni bitwise simili alla classe `Int`.
### Principali Metodi e Proprietà:
- **Value**: Il valore dell’intero a 64-bit.
- **Bytes/Binary**: Conversione del valore intero a un array di byte o a una rappresentazione binaria.
- **ReverseAnd/Or**: Operazioni per ottenere risultati logici inversi.
- **Not, And, Or, Xor, Nand**: Implementazione di varie operazioni logiche.
- **ToString()**: Metodo che restituisce la rappresentazione stringa del valore `Long`.

## Classe `Float`
Rappresenta un numero a virgola mobile a singola precisione.
### Principali Metodi e Proprietà:
- **Value**: Il valore del numero in virgola mobile.
- **Bytes/Binary**: Conversioni da e verso rappresentazioni binarie.
- **ReverseAnd/Or**: Operazioni logiche inverse su Float.
- **Not, And, Or, Xor, Nand**: Varie operazioni logiche applicabili al tipo Float.
- **ToString()**: Metodo per restituire la rappresentazione formattata del numero.

## Classe `Double`
Un numero a virgola mobile a doppia precisione con operazioni binarie richiede.
### Principali Metodi e Proprietà:
- **Value**: Rappresentazione del numero double.
- **Bytes/Binary**: Rappresentazioni alternative del valore double.
- **ReverseAnd/Or**: Esecuzione di operazioni AND/OR inverse personalizzate.
- **Not, And, Or, Xor, Nand**: Operazioni bitwise.
- **ToString()**: Converte il valore numerico in una stringa.

### Osservazioni
Le classi `Decimal` e altri tipi possono essere preventivisti, ma sono privi di implementazione dettagliata nel file corrente. Ogni classe include operazioni binarie standard basandosi su varie interfacce logiche per garantire flessibilità e operazioni estese.

## Classe `UShort`
La classe `UShort` rappresenta un intero senza segno a 16 bit con molteplici operazioni binarie.
### Principali Metodi e Proprietà:
- **Value**: Valore dell'unsigned short.
- **Bytes**: Rappresentazione in byte del valore.
- **Binary**: Rappresentazione binaria del valore.
- **BitLength**: Lunghezza in bit del valore.
- **ReverseAnd/ReverseOr**: Operazioni logiche inverse che restituiscono un `IncompleteUShort`.
- **Not, And, Or, Xor, Nand**: Esecuzione di operazioni logiche con variabili dello stesso tipo o `IncompleteUShort`.
- **ToString(String)**: Converti `UShort` in una stringa formattata.

## Classe `ULong`
La classe `ULong` rappresenta un intero senza segno a 64 bit con operazioni binarie avanzate.
### Principali Metodi e Proprietà:
- **Value**: Valore dell'unsigned long.
- **Bytes**: Rappresentazione in byte.
- **Binary**: Rappresentazione binaria.
- **BitLength**: Lunghezza in bit del valore.
- **ReverseAnd/ReverseOr**: Usa operazioni inverse per ottenere il risultato come `IncompleteULong`.
- **Not, And, Or, Xor, Nand**: Funzioni principali per operazioni logiche tra `ULong` e `IncompleteULong`.
- **ToString() e ToString(string)**: Metodi per restituire o formattare il valore numerico in una stringa.

## Classe `UInt`
La classe `UInt` affronta un intero senza segno a 32 bit, offrendo simili operazioni bit-wise.
### Principali Metodi e Proprietà:
- **Value**: Il valore numerico del tipo unsigned integer.
- **Bytes**: Rappresentazione in byte.
- **Binary**: Rappresentazione binaria del valore.
- **BitLength**: Lunghezza in bit.
- **ReverseAnd/ReverseOr**: Metodi per operazioni logiche inverse.
- **Not, And, Or, Xor, Nand**: Implementazioni per la manipolazione binaria.
- **ToString() e ToString(string)**: Formattazione e conversione a stringa.

## Classe `Short`
La classe `Short` consente la manipolazione di un intero firmato a 16 bit con operazioni binarie complesse.
### Principali Metodi e Proprietà:
- **Value**: Il valore `short`.
- **Bytes**: La rappresentazione in byte di questo valore.
- **Binary**: Array booleano che rappresenta il valore.
- **BitLength**: Misura della lunghezza in bit.
- **ReverseAnd/ReverseOr**: Permettono il calcolo di operazioni logiche inverse.
- **Not, And, Or, Xor, Nand**: Operazioni logiche per il tipo `Short` e `IncompleteShort`.
- **ToString() e ToString(string)**: Metodi per ottenere la rappresentazione stringa del valore.

## Classe `String`
La classe `String` è un tipo stringa personalizzato che implementa l'interfaccia `IList<Char>` per oggetti `Char`.
### Principali Metodi e Proprietà:
- **Constructors**: Costruisce una stringa standard o da un valore `string`.
- **GetEnumerator()**: Fornisce un enumeratore per attraversare la collezione di caratteri.
- **Add, Clear, Contains, Insert, Remove, RemoveAt**: Metodi per modificare gli elementi `Char` contenuti.
- **Count**: Numero di elementi nella stringa personalizzata.
- **IndexOf**: Ricerca l'indice di un oggetto `Char`.
- **ToString() e ToString(string)**: Ritorna la rappresentazione stringa della collezione `Char`.

## IncompleteInt
- **Rappresentazione:** Rappresenta un intero incompleto con varie operazioni bit a bit.
- **Proprietà `Binary`:** Memorizza la rappresentazione binaria dell'intero come array di `bool?`, lunghezza predefinita è `32` bit.
- **Proprietà `IsComplete`:** Indica se la rappresentazione binaria è completa, cioè senza bit nulli.
- **Proprietà `Permutations`:** Calcola il numero di permutazioni possibili con i bit nulli.
- **Indicizzazione:** Fornisce la rappresentazione intera per un valore specifico.
- **Metodi principali:**
  - Operazioni bit a bit: AND, OR, XOR, NOT, e operazioni inverse su altri tipi `Int` o `IncompleteInt`.
  - Controllo di contenimento per verificare se una rappresentazione binaria contiene un dato intero o intero incompleto.

## IncompleteFloat
- **Rappresentazione:** Simile a `IncompleteInt`, ma rappresenta un valore float incompleto.
- **Proprietà e Metodi:** Mantiene strutture e operazioni analoghe a `IncompleteInt`, adattate per tipi float (`float`, `IncompleteFloat`).

## IncompleteDouble
- **Rappresentazione:** Rappresenta un double incompleto.
- **Proprietà e Metodi Principali:** Simili a `IncompleteFloat`, utilizzando tipi `double` e `IncompleteDouble` per le operazioni.

## IncompleteChar
- **Rappresentazione:** Rappresenta un carattere che non è completamente determinato.
- **Proprietà `Binary`:** Rappresentazione a `16` bit (per caratteri).
- **Metodi e Operazioni:** Implementa operazioni bit a bit come AND, OR, XOR, NOT con carattere e carattere incompleto (`Char`, `IncompleteChar`).

## IncompleteByte
- **Rappresentazione:** Gestisce un byte singolo incompleto.
- **Proprietà `Binary`:** Rappresentazione di lunghezza `8` bit.
- **Operazioni Bit a Bit:** Simili a quelle delle altre classi, specifiche per byte.
- **Controllo di contenimento e permutazioni:** Verifica il contenimento di byte dati e calcola le permutazioni delle rappresentazioni.

## IncompleteByteArray
- **Gestione Array:** Offre la gestione di array di byte incompleti con proprietà `Binary`.
- **Operazioni su Array:** Converte rappresentazioni binarie in array di byte incompleti, controlla le permutazioni e gestisce la dimensione in byte.

## IncompleteUShort
- **Descrizione:** Rappresenta un valore `ushort` non completamente determinato, con bit mancanti rappresentati come valori null in un array di booleani nullable.
- **Implementa:** Interfaccia `IIRregularOperable<IncompleteUShort, UShort, ushort, uint>`.
- **Proprietà:**
  - `Binary`: Array di `bool?` che rappresenta la rappresentazione binaria.
  - `IsComplete`: Indica se tutti i bit sono determinati (non null).
  - `Permutations`: Numero di permutazioni possibili con i bit mancanti.

- **Metodi:**
  - Indicizzazione per ottenere rappresentazioni di `UShort`.
  - Conversioni a `ByteArray`.
  - Rappresentazioni stringhe dei valori incompleti.
  - Operazioni bitwise come `Or`, `And`, `Not`, `Xor`.
  - Operazioni di reverse bitwise come `ReverseAnd`, `ReverseOr`.

## IncompleteULong
- **Descrizione:** Gestisce una rappresentazione binaria incompleta di un intero `ulong`.
- **Implementa:** Interfacce `IIncompleteValue`, `IRregularReversible`, `IIRregularOperable`.
- **Proprietà:**
  - `Binary`: Array di `bool?`, memorizza la rappresentazione binaria incompleta.
  - `IsComplete`: Controlla la completezza della rappresentazione binaria.
  - `Permutations`: Numero di permutazioni dei bit mancanti.

- **Metodi:**
  - Indicizzazione per ottenere valori di `ULong`.
  - Conversioni a `ByteArray` e stringhe.
  - Operazioni bitwise (`Or`, `And`, `Not`, `Xor`).
  - Reverse bitwise (`ReverseAnd`, `ReverseOr`).

## IncompleteUInt
- **Descrizione:** Rappresenta un valore unsigned int incompleto con operazioni bitwise.
- **Implementa:** `IIRregularOperable<IncompleteUInt, UInt, uint, uint>`.
- **Proprietà:**
  - `Binary`: Memorizza la rappresentazione binaria con bit mancanti come null.
  - `IsComplete`: Indica la completezza.
  - `Permutations`: Conteggio delle permutazioni possibili.

- **Metodi:**
  - Conversione e operazioni bitwise (`Or`, `And`, `Not`, `Xor`).
  - Reverse operazioni bitwise (`ReverseAnd`, `ReverseOr`).

## IncompleteShort
- **Descrizione:** Modella un intero breve con operazioni bitwise.
- **Implementa:** `IIRregularOperable<IncompleteShort, Short, short, uint>`.
- **Proprietà:**
  - `Binary`: Array di booleani nullable per rappresentare valori binari.
  - `IsComplete`: Verifica se la rappresentazione è completa.
  - `Permutations`: Numero totale di permutazioni possibili.

- **Metodi:**
  - Indicizzazione, conversioni e operazioni bitwise come `Or`, `Not`, `Xor`, `And`.
  - Reverse operazioni bitwise.

## IncompleteLong
- **Descrizione:** Gestisce un intero lungo incompleto con diverse operazioni bitwise.
- **Implementa:** Interfaccia `IIRregularOperable<IncompleteLong, Long, long, ulong>`.
- **Proprietà:**
  - `Binary`: Stoccaggio di rappresentazione binaria incompleta.
  - `IsComplete`: Verifica di completezza della rappresentazione.
  - `Permutations`: Conteggio delle permutazioni.

- **Metodi:**
  - Indicizzazione per ottenere valori `Long`.
  - Conversione a `ByteArray` e stringhe testuali.
  - Esegue operazioni bitwise tradizionali e reverse.


Ogni classe in questo modulo fornisce strumenti avanzati per la manipolazione di tipi dati incompleti, includendo capacità di operazioni bitwise e controllo delle rappresentazioni binarie parziali. Risultano utili in contesti dove i valori non sono completamente determinati, come grafica, crittografia o elaborazioni simili.

# Descrizione delle Classi del Namespace `NumeralSystems.Net.Type.Base`
## Classi e Metodi
### Classe `ULong`
- **Scopo:** Fornisce metodi per convertire valori `ulong` in rappresentazioni di indici basati su un sistema numerico specificato e viceversa.
- **Metodi Principali:**
  - `ToIndicesOfBase(ulong val, int destinationBase)`: Converte un valore `ulong` in un array di indici in una base specificata.
  - `FromIndicesOfBase(ulong[] val, int sourceBase)`: Restituisce un valore `ulong` da una rappresentazione di indici in una base specificata.

### Classe `UInt`
- **Scopo:** Gestisce la conversione di valori `int` in indici di una base numerica specificata e viceversa, tenendo conto della positività del valore.
- **Metodi Principali:**
  - `ToIndicesOfBase(int val, int destinationBase, out bool positive)`: Converte un valore `int` nella sua rappresentazione di indici e determina se è positivo.
  - `FromIndicesOfBase(uint[] val, int sourceBase, bool positive)`: Converte una rappresentazione di indici in un valore `int`, tenendo conto del segno.

### Classe `Float`
- **Scopo:** Converte valori `float` nelle loro rappresentazioni di indici, separando le parti integrale e frazionale.
- **Metodi Principali:**
  - `ToIndicesOfBase(float val, int destinationBase)`: Converte un valore `float` in una rappresentazione distribuita tra partione integrale e frazionale.
  - `FromIndicesOfBase(uint[] integral, uint[] fractional, bool positive, int sourceBase)`: Ricostruisce un valore `float` dalla sua rappresentazione di indici.

### Classe `Double`
- **Scopo:** Similarmente alla classe Float, gestisce la conversione per valori `double`, supportando una maggiore precisione.
- **Metodi Principali:**
  - `ToIndicesOfBase(double val, int destinationBase)`: Converte un valore `double` in indici, gestendo separatamente parte integrale e frazionale.
  - `FromIndicesOfBase(ulong[] integral, ulong[] fractional, bool positive, int sourceBase)`: Ricostruisce un valore `double` utilizzando la rappresentazione degli indici.

### Classe `Decimal`
- **Scopo:** Offrire conversioni numeriche specifiche per valori `decimal`, con alta precisione frazionale.
- **Metodi Principali:**
  - `ToIndicesOfBase(decimal val, int destinationBase)`: Converte un valore `decimal` in una struttura di indici, distinta in parte integrale e frazionale.
  - `FromIndicesOfBase(ulong[] integral, ulong[] fractional, bool positive, int sourceBase)`: Ricostruisce un valore `decimal` dai suoi componenti basati sull'indice.

### Classe `BigInteger`
- **Scopo:** Provides methods for converting and interpreting big integer values based on specified numeral systems.
- **Metodi Principali:**
  - `FromIndicesOfBase(ulong[] integral, ulong[] fractional, bool positive, int sourceBase)`: Decodifica componenti indice a una rappresentazione `BigInteger`.
  - `ToIndicesOfBase(System.Numerics.BigInteger val, int destinationBase)`: Genera una sequenza di indici da un `BigInteger`.

# Classe `String` nel Namespace `NumeralSystems.Net.Type.Base`
La classe `String` offre metodi per codificare e decodificare stringhe attraverso diversi sistemi numerici di rappresentazione, trasformando caratteri in indici numerici e viceversa.
## Metodi Principali
### EncodeToBase
- **Scopo:** Codifica una stringa in un sistema numerico specificato.
- **Parametri:**
  - `s` (string): La stringa da codificare.
  - `destinationBase` (int): La base destinazione per la codifica.
  - `size` (out int): La dimensione della stringa codificata.

- **Ritorna:** La stringa codificata.
- **Eccezioni:** Lancia un `ArgumentException` se la base destinazione è ≤ 0 o > `char.MaxValue`.

### DecodeFromBase
- **Scopo:** Decodifica una stringa precedentemente codificata da un sistema numerico specificato.
- **Parametri:**
  - `s` (string): La stringa da decodificare.
  - `sourceBase` (int): La base dalla quale decodificare.
  - `size` (int): La dimensione della stringa codificata.

- **Ritorna:** La stringa decodificata.
- **Eccezioni:** Lancia un `ArgumentException` se la base sorgente è ≤ 0 o > `char.MaxValue`.

### ToIndicesOfBase
- **Scopo:** Converte una stringa nei suoi indici corrispondenti in una base specificata.
- **Parametri:**
  - `s` (string): La stringa da convertire.
  - `destinationBase` (int): La base destinazione.

- **Ritorna:** Un `IEnumerable` di array `uint` rappresentanti gli indici.

### FromIndicesOfBase
- **Scopo:** Converte una rappresentazione di indici in una base specificata indietro in una stringa.
- **Parametri:**
  - `s` (IEnumerable<uint[]>): Gli indici da convertire.
  - `sourceBase` (int): La base degli indici.

- **Ritorna:** La stringa decodificata.

### GetSmallestBase
- **Scopo:** Determina la minima base necessaria per rappresentare tutti i caratteri in una stringa.
- **Parametri:**
  - `s` (string): La stringa da analizzare.

- **Ritorna:** Il più piccolo numero di base che può rappresentare tutti i caratteri della stringa.

Questa classe è particolarmente utile per operazioni di crittografia e conversione di sistemi numerici, permettendo la trasformazione di dati testuali in formati numerici e viceversa.

# Classe `Convert` nel Namespace `NumeralSystems.Net.Utils`
La classe `Convert` offre una serie di metodi statici per effettuare conversioni tra vari tipi di dati e le loro rappresentazioni binarie.
## Metodi Principali
### Conversione di Bool Array
- **`ToByte(this bool[] s)`**
  - Converte un array di valori booleani rappresentanti bit in un singolo byte.

- **`ToChar(this bool[] s)`**
  - Converte un array di valori booleani in un carattere.

- **`ToByteArray(...)`**
  - Converte diversi tipi di dati (char, short, int, long, ecc.) in array di byte.

### Conversione in Bool Array
- **`ToBoolArray(this byte b)`**
  - Converte un byte in una rappresentazione di array di valori booleani, con ciascun booleano rappresentante un bit.

- **Metodi simili esistono per diversi tipi di dati**, come `char`, `int`, `double`, `decimal`, `ulong`, e `BigInteger`, permettendo una conversione uniforme da tipi diversi a rappresentazioni binarie booleane.

### Manipolazione di Bit
- **`SetBoolAtIndex(...)`**
  - Setta un bit a un determinato indice all'interno di un byte o altro tipo conforme, con la possibilità di definire se il bit deve essere impostato a `true` o `false`.

- **`GetBoolAtIndex(...)`**
  - Ottiene il valore di un bit al determinato indice da vari tipi di dati, supportando diversi tipi come `byte`, `char`, `int`, e `BigInteger`.

## Utilizzo
La classe `Convert` è progettata per essere usata attraverso metodi di estensione in qualsiasi parte del codice che richieda conversioni tra tipi di dati numerici e le loro rappresentazioni binarie dettagliate.
Questa utilità è particolarmente utile per operazioni a basso livello, crittografia, e manipolazione di dati nel contesto di sistemi numerici e rappresentazioni digitali.

# Classe `StringExtensions` nel Namespace `NumeralSystems.Net.Utils`
## Descrizione
La classe `StringExtensions` offre metodi statici che estendono la funzionalità delle stringhe. Questi metodi permettono operazioni avanzate di manipolazione delle stringhe come la rimozione o mantenimento di delimitatori specifici e la suddivisione di stringhe mantenendo i delimitatori.
## Metodi
### `TakeOnly`
- **Descrizione:** Filtra una stringa mantenendo solo i delimitatori specificati.
- **Parametri:**
  - `input` (string): La stringa di input.
  - `delimiters` (params string[]): I delimitatori da mantenere.

- **Ritorno:** Un `IEnumerable<string>` di sottostringhe contenenti i delimitatori specificati.

### `Remove`
- **Descrizione:** Rimuove i delimitatori specificati dalla stringa di input e restituisce le sottostringhe risultanti.
- **Parametri:**
  - `input` (string): La stringa di input da cui rimuovere i delimitatori.
  - `delimiters` (params string[]): I delimitatori da rimuovere.

- **Ritorno:** Un `IEnumerable<string>` di sottostringhe risultanti.

### `SplitAndKeep`
- **Descrizione:** Divide una stringa in base ai delimitatori forniti mantenendo i delimitatori nel risultato.
- **Parametri:**
  - `input` (string): La stringa da dividere.
  - `delimiters` (params string[]): I delimitatori da utilizzare per la divisione.

- **Ritorno:** Un array di stringhe che contiene le parti divise della stringa di input.

### `FindDelimiterIndex`
- **Descrizione:** Trova l'indice della prima occorrenza di un delimitatore a partire da un indice specificato.
- **Parametri:**
  - `input` (string): La stringa di input.
  - `startIndex` (int): L'indice iniziale per la ricerca.
  - `delimiters` (IEnumerable ): I delimitatori da cercare.

- **Ritorno:** L'indice della prima occorrenza o `-1` se nessun delimitatore viene trovato.

# Classe `Sequence` nel Namespace `NumeralSystems.Net.Utils`
## Descrizione
La classe `Sequence` fornisce metodi utili per la generazione e manipolazione di sequenze. È progettata per operare su insiemi e collezioni di dati numerici, permettendo operazioni come la generazione di intervalli, la determinazione di permutazioni e combinazioni, e la suddivisione di collezioni in gruppi.
## Metodi
### `SequenceOfIdentityAtIndex`
- **Descrizione:** Restituisce una sequenza di valori di identità per un dato indice.
- **Parametri:**
  - `identity` (List ): La lista dei valori di identità.
  - `index` (int): L'indice da cui ottenere la sequenza.

- **Ritorno:** Un `IEnumerable<T>` di valori di identità.

### `IdentityEnumerableOfSize`
- **Descrizione:** Genera una sequenza di valori di identità di una certa dimensione.
- **Parametri:**
  - `identity` (List ): La lista dei valori di identità.
  - `size` (int): La dimensione della sequenza di identità.

- **Ritorno:** Un `IEnumerable<IEnumerable<T>>` di sequenze di valori di identità.

### `Range`
- **Descrizione:** Genera una sequenza di interi senza segno entro un intervallo specificato.
- **Overloads:**
  - Può operare su `uint`, `ulong`, e `BigInteger`.

### `CountToUInt` / `CountToULong`
- **Descrizione:** Conta il numero di elementi in una sequenza e restituisce il conteggio come `uint` o `ulong`.
- **Parametri:**
  - `sequence` (IEnumerable): La sequenza di elementi.

- **Ritorno:** Il conteggio degli elementi nella sequenza come `uint`/`ulong`.

### `PermutationsCount`
- **Descrizione:** Calcola il numero di permutazioni di una data dimensione da un dato insieme. Supporta il calcolo con o senza ripetizione.
- **Parametri:**
  - `identity` (Tipo numerico): Rappresenta il numero di elementi distinti.
  - `size` (Tipo numerico): Rappresenta il numero di elementi selezionati.
  - `repetition`(bool): Indica se è permessa la ripetizione.

- **Ritorno:** Il conteggio delle permutazioni.

### `CombinationsCount`
- **Descrizione:** Calcola il numero di combinazioni possibili da un insieme di dati.
- **Parametri:**
  - `identity` (int): Il numero di elementi nel set.
  - `size` (int): La dimensione di ciascuna combinazione.
  - `repetition` (bool): Indica se la ripetizione è permessa.

- **Ritorno:** Il conteggio delle combinazioni.

### `Group`
- **Descrizione:** Raggruppa gli elementi di un array in sottoarray di dimensioni specificate.
- **Parametri:**
  - `sequence` (T[]): L'array di elementi da raggruppare.
  - `count` (int): La dimensione di ciascun gruppo.

- **Ritorno:** Un array bidimensionale con sottoarray di elementi raggruppati.

Questi metodi sono utili per applicazioni matematiche e logiche, coinvolgendo la combinazione, il conteggio e la gestione di sequenze e insiemi.


### Classe Numeral
La classe `Numeral` rappresenta un numero espresso in un sistema numerico specificato. Fornisce metodi per ottenere e impostare le parti integrali e frazionarie del numero, nonché per convertirlo in diversi tipi.
#### Proprietà
- **Positive**: Indica se il numero è positivo o negativo.
- **Base**: Rappresenta il sistema numerico in cui è espresso il numero.
- **FractionalIndices**: Ottiene o imposta la lista degli indici frazionari per il numero.
- **IntegralIndices**: Ottiene o imposta la lista degli indici integrali per il numero.

#### Metodi
- **GetFractionalStrings(IList identity) **: Ritorna la parte frazionaria del numero come una lista di stringhe.
- **GetFractionalString(IList identity, string separator) **: Ritorna la parte frazionaria come stringa, usando un separatore specificato.
- **GetIntegralStrings(IList identity) **: Restituisce le cifre integrali del numero come una lista di rappresentazioni stringa.
- **GetIntegralString(IList identity, string separator) **: Ritorna la parte integrale come stringa usando un separatore specifico.
- **TrySetValue(List value) **: Prova a impostare il valore del numero usando una lista di indici interi.
- **To(NumeralSystem baseSystem)**: Converte il valore numerico nel sistema numerico specificato.

#### Costruttori
- **Numeral()**: Inizializza un nuovo oggetto `Numeral` con base 10.
- **Numeral(NumeralSystem numericSystem)**: Inizializza un nuovo oggetto `Numeral` con il sistema numerico specificato.
- **Numeral(NumeralSystem numericSystem, List integral, List  fractional, bool positive)  **: Inizializza con parti integrale e frazionaria specificate e segno.

#### Altre Proprietà per Conversioni
- **Integer**: Rappresenta un valore intero nel sistema numerico specificato.
- **Char**: Rappresenta il numero come carattere.
- **Double**: Rappresenta il numero come numero in virgola mobile a doppia precisione.
- **Decimal**: Rappresenta il numero come tipo `decimal`.
- **Float**: Rappresenta il numero come tipo `float`.
- **Bytes**: Ottiene o imposta la rappresentazione come array di byte.

### Classe NumeralSystem
La classe `NumeralSystem` definisce un sistema numerico con una base specificata. Offre funzionalità per analizzare, convertire e manipolare numeri in vari formati e rappresentazioni.
#### Proprietà
- **Size**: Rappresenta la dimensione o base del sistema numerico.
- **SkipUnknownValues**: Determina se i valori sconosciuti devono essere saltati durante l'elaborazione.

#### Metodi
- **TrySplitNumberIndices**: Prova a dividere un numero nelle parti integrale e frazionaria restituendo gli indici.
- **TryFromIndices**: Prova a costruire una rappresentazione stringa dagli indici forniti.
- **Parse**: Converte una rappresentazione stringa di un numero in un oggetto `Numeral`.
- **TryParse**: Prova a convertire una stringa in un numero, restituendo un risultato di successo.
- **Contains**: Controlla se una lista di indici è valida nel sistema numerico.
- **TryIntegerOf**: Prova a convertire una lista di indici in un valore intero.
- **TryCharOf**: Prova a convertire una lista di indici in un carattere.

#### Costruttori
- **NumeralSystem(int size)**: Inizializza un sistema numerico con la dimensione specificata.

Utilizza queste descrizioni nel tuo file di documentazione per fornire un riepilogo dettagliato delle due classi e delle loro rispettive responsabilità all'interno del progetto.

