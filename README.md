# NumeralSystem

This library is inteded to simplify the creation/usage of numeral systems.


## Usage

### Creation of numeral system

To create a numeral system by It's construnctor you must define an identity of unique elements and a string separator.

    public NumeralSystem(HashSet<TElement> identity, string separator = ";")

Otherwhise you can create a numeral system with the identity dimension by calling

    public static NumeralSystem<string> Numeral.System.OfBase(int value, string separator = "")

### UnitTest

To test the functionalities you can use NumeralSystem.Net.NUnit

### Test
To test the implementations you can use the UI project NumeralConverter that features a conversion from base to base.

## Missing features
### Floating numbers

At this moment floating point numbers are not yet implemented.
They will be a new features of this library soon

### Math

Math will be implemented as a separate library with NumeralSystem.Net as a dependency

### Incomplete numeral

Incomplete numerals will be implemented in this library as a new feature after the introduction of the math library

## Nuget
You can use the library here from [Nuget.org](https://www.nuget.org/packages/NumeralSystems.Net/0.5.0)