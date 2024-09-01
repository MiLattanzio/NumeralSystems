# NumeralSystem

This library is inteded to simplify the creation/usage of numeral systems and base types.


## Usage

### Creation of numeral system

You can create a numeral system with the identity dimension by calling:

    public static NumeralSystem Numeral.System.OfBase(int value, string separator = "")
Like so:

    var binary = Numeral.System.OfBase(2);
### Indexing a numeral
You can get a numeral from It's index (int or float):

    var valueA = binary[12];
    var valueB = binary[1.2f];
### Parsing a string
You can parse a string to It's numeral representation with the StringParse method:

    var valueC = binary.Parse("1100");
### Conversions
A numeral can be converted to:

- An integer


    var a = valueA.Integer;
- A double


    var b = valueB.Double;
- A decimal


    var d = valueB.Double;
- A string
    

    var c = valueC.ToString();
- A number of another base

    var d = valueC.To(Numeral.System.OfBase(5));
### Base types
If you ever need to perform reverse bitwise operation or access bits of your native data types you can use the wrapper inside NumeralSystems.Net.Type.Base

### Incomplete types

If you ever need an incomplete representation of a native type due missing bits that can be either true or false you can use NumeralSystems.Net.Type.Base.Incomplete 

### UnitTest

To test the functionalities you can use NumeralSystem.Net.NUnit

### Test
To test the implementations you can use the UI project NumeralConverter that features a conversion from base to base.

## Nuget
You can use the library here from [Nuget.org](https://www.nuget.org/packages/NumeralSystems.Net)
