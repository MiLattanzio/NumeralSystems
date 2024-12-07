using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Decimal = NumeralSystems.Net.Type.Base.Decimal;

namespace NumeralSystems.Net
{
    /// <summary>
    /// Represents a numeral value with a specified base.
    /// </summary>
    public class NumeralValue
    {
        /// <summary>
        /// Represents the integral part of a numeral value as a read-only list of integers.
        /// </summary>
        /// <remarks>
        /// The integral part consists of digits in a specified numeral base (radix).
        /// Each integer in the list represents a single digit in the base-specific notation.
        /// This property is part of the representation of a numeral value and is used to
        /// perform arithmetic operations and conversions within different numeral systems.
        /// </remarks>
        public IReadOnlyList<int> Integral { get; }

        /// <summary>
        /// Represents the decimal part of a numeral value in a specific base.
        /// This property provides a read-only list of integers that represent
        /// each digit in the decimal part of the number, in the numeral system's base.
        /// Each integer value in the list corresponds to a digit in the base-n representation
        /// of the numeral value. Ensure that all digits in the list are less than the numeral system's base.
        /// </summary>
        public IReadOnlyList<int> Decimals { get; }

        /// <summary>
        /// Gets a value indicating whether the numeral value is negative.
        /// </summary>
        /// <remarks>
        /// This property returns <c>true</c> if the numeral value represents a negative number;
        /// otherwise, it returns <c>false</c>.
        /// </remarks>
        public bool Negative { get; }

        /// <summary>
        /// Gets the base of the numeral system used to represent the numeric value.
        /// </summary>
        /// <remarks>
        /// The base value must be greater than 0. It is used as the numeric base for
        /// the integral and decimal parts of the value, determining how numbers
        /// are interpreted and displayed in their respective numeral systems.
        /// </remarks>
        public int Base { get; }

        /// Represents a numeral value composed of integral and decimal parts, with
        /// an optional negative sign and a specified base. Provides functionality
        /// to convert from various numeric types and between different numeral
        /// bases.
        public NumeralValue(List<int> integral, List<int> decimals, bool negative, int baseValue)
        {
            if (baseValue < 1) throw new ArgumentOutOfRangeException(nameof(baseValue));
            if (null == integral || !integral.TrueForAll(x => baseValue > x)) throw new ArgumentOutOfRangeException(nameof(baseValue));
            if (null == decimals || !decimals.TrueForAll(x => baseValue > x)) throw new ArgumentOutOfRangeException(nameof(baseValue));
            Integral = integral.AsReadOnly();
            Decimals = decimals.AsReadOnly();
            Negative = negative;
            Base = baseValue;
        }

        /// <summary>
        /// Converts a decimal number into a NumeralValue instance with base 10 representation.
        /// </summary>
        /// <param name="decimalValue">The decimal number to be converted.</param>
        /// <returns>A NumeralValue representing the given decimal number in base 10.</returns>
        public static NumeralValue FromDecimal(decimal decimalValue)
        {
            var (integers, fractionals, positive) = Type.Base.Decimal.ToIndicesOfBase(decimalValue, 10);
            var integral = integers.Select(x => (int)x).ToList();
            var fractional = fractionals.Select(x => (int)x).ToList();
            return new NumeralValue(integral, fractional, !positive, 10);
        }

        /// <summary>
        /// Creates a <see cref="NumeralValue"/> instance from a given <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="bigInt">The <see cref="BigInteger"/> value to convert into a <see cref="NumeralValue"/>.</param>
        /// <returns>A <see cref="NumeralValue"/> representing the given <see cref="BigInteger"/>.</returns>
        public static NumeralValue FromBigInteger(BigInteger bigInt)
        {
            var integers = Type.Base.BigInteger.ToIndicesOfBase(bigInt, 10);
            var integral = integers.Select(x => (int)x).ToList();
            return new NumeralValue(integral, new List<int>(), bigInt < 0, 10);
        }

        /// <summary>
        /// Converts an integer value to a <see cref="NumeralValue"/> instance, assuming a base-10 representation.
        /// </summary>
        /// <param name="integerValue">The integer value to convert.</param>
        /// <returns>A <see cref="NumeralValue"/> representing the provided integer.</returns>
        public static NumeralValue FromInt(int integerValue) => FromDecimal(integerValue);

        /// <summary>
        /// Converts a float value to a <see cref="NumeralValue"/> object using a decimal intermediary.
        /// </summary>
        /// <param name="floatValue">The float value to convert.</param>
        /// <returns>A <see cref="NumeralValue"/> object that represents the provided float value.</returns>
        public static NumeralValue FromFloat(float floatValue) => FromDecimal(Decimal.From(floatValue));

        /// <summary>
        /// Converts a double value to a <see cref="NumeralValue"/> instance with a base of 10.
        /// </summary>
        /// <param name="doubleValue">The double value to convert.</param>
        /// <returns>A <see cref="NumeralValue"/> instance representing the given double value.</returns>
        public static NumeralValue FromDouble(double doubleValue) => FromDecimal(Decimal.From(doubleValue));

        /// <summary>
        /// Creates a <see cref="NumeralValue"/> object from a given <see cref="Value"/> object.
        /// </summary>
        /// <param name="value">The <see cref="Value"/> object to convert into a <see cref="NumeralValue"/>.</param>
        /// <returns>A new instance of <see cref="NumeralValue"/> representing the given <see cref="Value"/>.</returns>
        public static NumeralValue FromValue(Value value) => new NumeralValue(value.ToBase(10).Indices.ToList(), new List<int>(), false, 10);

        /// Converts the current NumeralValue to a BigInteger representation.
        /// The conversion takes into account the integral and decimal parts of the NumeralValue,
        /// as well as its base and sign (negative or positive).
        /// <returns>A BigInteger representation of the NumeralValue, computed from its integral parts,
        /// decimal parts, base, and sign.
        public BigInteger ToBigInteger() => Type.Base.BigInteger.FromIndicesOfBase(Integral.Select(x => (ulong)x).ToArray(), Decimals.Select(x => (ulong)x).ToArray(), !Negative, Base);

        /// Converts the current numeral value to its decimal representation.
        /// Uses the integral and fractional parts of the numeral, along with its base and sign,
        /// to determine the corresponding decimal value.
        /// <returns>The decimal representation of the numeral value.</returns>
        public decimal ToDecimal() => Decimal.FromIndicesOfBase(Integral.Select(x => (ulong)x).ToArray(), Decimals.Select(x => (ulong)x).ToArray(), !Negative, Base);

        /// Converts the current numeral value to an integer representation.
        /// <returns>
        /// An integer representation of the current numeral value.
        /// </returns>
        public int ToInt() => (int)ToBigInteger();

        /// <summary>
        /// Converts the current <see cref="NumeralValue"/> instance to a float representation.
        /// </summary>
        /// <returns>
        /// A single-precision floating-point number that represents the current <see cref="NumeralValue"/>.
        /// </returns>
        public float ToFloat() => (float)ToDecimal();

        /// <summary>
        /// Converts the current <see cref="NumeralValue"/> to a double-precision floating-point number.
        /// </summary>
        /// <returns>A double representation of the current <see cref="NumeralValue"/>.</returns>
        public double ToDouble() => (double)ToDecimal();

        /// <summary>
        /// Converts the current NumeralValue instance to a Value object that represents
        /// the numeral system with a base of 10.
        /// </summary>
        /// <returns>A Value object that contains the integral parts of the NumeralValue
        /// as indices in a base-10 system.</returns>
        public Value ToValue() => new Value(Integral.ToList(), 10);

        public NumeralValue ToBase(int baseValue, bool removeFirstZeros = true)
        {
            if (baseValue <= 0)
                throw new ArgumentException("baseValue must be greater than zero.");

            var frontIntegralZeros = Integral.AsEnumerable().TakeWhile(x => x == 0).Count();
            var frontFractionalZeros = Decimals.AsEnumerable().TakeWhile(x => x == 0).Count();

            var integrals = new List<int>();
            var currentIntegrals = new List<int>(Integral); // Work with a copy of the indices

            while (currentIntegrals.Count > 0 && !IsZero(currentIntegrals))
            {
                var (quotient, remainder) = DivideByBase(currentIntegrals, baseValue);
                integrals.Insert(0, remainder); // Insert remainder at the front
                currentIntegrals = quotient; // Continue with the quotient
            }

            // If the value is zero, represent it correctly
            if (integrals.Count == 0)
                integrals.Add(0);
            
            var fractionals = new List<int>();
            var currentFractionals = new List<int>(Decimals); // Work with a copy of the indices

            while (currentFractionals.Count > 0 && !IsZero(currentFractionals))
            {
                var (quotient, remainder) = DivideByBase(currentFractionals, baseValue);
                fractionals.Insert(0, remainder); // Insert remainder at the front
                currentFractionals = quotient; // Continue with the quotient
            }

            // If the value is zero, represent it correctly
            if (fractionals.Count == 0)
                fractionals.Add(0);
            
            if (!removeFirstZeros) integrals = Enumerable.Repeat(0, frontIntegralZeros).Concat(integrals).ToList();
            fractionals = Enumerable.Repeat(0, frontFractionalZeros).Concat(fractionals).ToList();
            
            return new NumeralValue(integrals, fractionals, Negative, baseValue);
        }

        /// <summary>
        /// Divides a number, represented as a list of digits, by the specified base and returns the quotient and remainder.
        /// </summary>
        /// <param name="number">The number to be divided, represented as a list of digits.</param>
        /// <param name="baseValue">The base to divide by.</param>
        /// <returns>A tuple containing the quotient as a list of digits and the remainder as an integer.</returns>
        private (List<int> Quotient, int Remainder) DivideByBase(List<int> number, int baseValue)
        {
            var quotient = new List<int>();
            var remainder = 0;

            foreach (var current in number.Select(digit => remainder * Base + digit))
            {
                quotient.Add(current / baseValue);
                remainder = current % baseValue;
            }

            // Remove leading zeroes from the quotient
            while (quotient.Count > 0 && quotient[0] == 0)
            {
                quotient.RemoveAt(0);
            }

            return (quotient, remainder);
        }

        // Helper method to check if a number is zero in the current base
        /// <summary>
        /// Determines if the specified list of digits represents a zero value in the numeral system.
        /// </summary>
        /// <param name="number">A list of integers representing the digits of the number in a certain base.</param>
        /// <returns>
        /// Returns true if all digits in the list are zero or if the list is empty, representing a zero value.
        /// Returns false otherwise.
        /// </returns>
        private static bool IsZero(List<int> number)
        {
            return !number.Any() || number.TrueForAll(digit => digit == 0);
        }

    }
}