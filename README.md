# NumeralSystem

This library is inteded to simplify the creation/usage of numeral systems.


## Usage

### Creation of numeral system

To create a numeral system by It's construnctor you must define an identity of unique elements and a string separator.

    public NumeralSystem(HashSet<TElement> identity, string separator = ";")

Otherwhise you can create a numeral system with the identity dimension by calling:

    public static NumeralSystem<string> Numeral.System.OfBase(int value, string separator = "")
Like so:

    var binary = Numeral.System.OfBase(2);
### Indexing a numeral
You can get a numeral from It's index (int or float):

    var valueA = binary[12];
    var valueB = binary[1.2f];
### Parsing a string
You can parse a string to It's numeral representation with the StringParse method:

    var valueC = binary.StringParse("1100");
### Conversions
A numeral can be converted to:

- An integer


    var a = valueA.Integer;
- A float


    var b = valueB.Float;
- A string
    

    var c = valueC.ToString();
- A number of another base


    var d = valueC.To(Numeral.OfBase(5));
### Customizations
You can create your custom Numeral System by providing:

- A custom string parser.
    
    By calling the setter of "Func<string, Numeral<TElement>> StringParse" in the NumeralSystem class
- The indexing mechanism.

  By calling the setter of "Func<int, bool, List<TElement>, List<TElement>> IntIndexer" in the NumeralSystem class
- The string conversion mechanism.
    
    By calling the setter of "Func<(List<TElement> Integral, List<TElement> Fractional, bool IsPositive), string> StringConverter" in the NumeralSystem class
- The negative simbol.

  By calling the setter of "string NegativeSign" in the NumeralSystem class
- The CultureInfo (That provides the default symbols for negative and floats)

    By calling the setter of "CultureInfo CultureInfo" in the NumeralSystem class
- The Float symbol.
    
    By calling the setter of "string FloatSign" in the NumeralSystem class
- The Separator. (If the string representing the value is longer than 1 char)

    By calling the setter of "string Separator" in the NumeralSystem class

### UnitTest

To test the functionalities you can use NumeralSystem.Net.NUnit

### Test
To test the implementations you can use the UI project NumeralConverter that features a conversion from base to base.

## Missing features
### Math

Math will be implemented as a separate library with NumeralSystem.Net as a dependency

### Incomplete numeral

Incomplete numerals will be implemented in this library as a new feature after the introduction of the math library

## Nuget
You can use the library here from [Nuget.org](https://www.nuget.org/packages/NumeralSystems.Net/0.5.0)