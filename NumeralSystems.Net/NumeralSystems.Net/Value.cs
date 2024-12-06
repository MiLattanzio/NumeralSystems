using System;
using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;

namespace NumeralSystems.Net
{
    /// <summary>
    /// Represents a numerical value with a specified base and a list of indices corresponding to the digits in that base.
    /// </summary>
    public class Value
    {
        /// <summary>
        /// Represents a value in a specified numeral system with a given base and indices.
        /// </summary>
        public Value(List<int> indices, int baseValue)
        {
            if (baseValue <= 0) throw new ArgumentException("baseValue must be greater than zero."); 
            Indices = indices.AsReadOnly();
            if (!Indices.All(x => baseValue > x)) throw new ArgumentException($"All indices must be within the range [0,{baseValue-1}].");
            Base = baseValue;
        }

        /// <summary>
        /// Represents a read-only list of indices corresponding to a number in a specified numeral system base.
        /// </summary>
        /// <remarks>
        /// Each index represents a digit in a numeral system, where the system's base is defined by the <see cref="Base"/> property.
        /// All indices must be within the range [0, Base-1].
        /// </remarks>
        /// <value>
        /// A read-only list of integers representing the indices of a number in the specified numeral system base.
        /// </value>
        public IReadOnlyList<int> Indices { get; }

        /// <summary>
        /// Gets the base value used in the numeral representation of the indices.
        /// </summary>
        /// <remarks>
        /// The base value determines the range of valid indices. For a given base,
        /// the indices should all be in the range from 0 to base-1.
        /// </remarks>
        public int Base { get; }

        /// <summary>
        /// Creates a Value object from a string representation using a specified set of base indices.
        /// </summary>
        /// <param name="value">The string representation from which to create the Value. If the string is null, an empty Value object will be created.</param>
        /// <param name="baseIndices">A set of valid characters that define the base indices. Each character in the input string is matched against this set to form numerical indices.</param>
        /// <returns>A Value object representing the parsed input string with numerical indices based on the provided base indices set.</returns>
        public static Value FromString(string value, HashSet<string> baseIndices)
        {
            var identity = baseIndices.ToList();
            if (null == value) return new Value(Array.Empty<int>().ToList(), baseIndices.Count);
            var indices = value.SplitAndKeep(baseIndices.ToArray()).Select(x => identity.IndexOf(x)).ToList();
            return new Value(indices, baseIndices.Count);
        }

        /// <summary>
        /// Creates a new <see cref="Value"/> instance from the given string and an optional fit parameter.
        /// </summary>
        /// <param name="value">The string representation to convert into a <see cref="Value"/>.</param>
        /// <param name="fit">A boolean indicating whether to fit the value within the smallest possible base.</param>
        /// <returns>A <see cref="Value"/> instance that represents the given string.</returns>
        public static Value FromString(string value, bool fit = false)
        {
            var indices = value.ToCharArray().Select(x => (int)x).ToList();
            var identity = fit ? indices.Max() + 1 : char.MaxValue;
            return new Value(indices, identity);
        }

        /// <summary>
        /// Converts the current numerical value represented by a list of indices in its original base to a specified base.
        /// </summary>
        /// <param name="baseValue">The base to which the numerical value should be converted. Must be greater than zero.</param>
        /// <returns>A new Value object representing the converted numerical value in the specified base.</returns>
        /// <exception cref="ArgumentException">Thrown when the specified baseValue is less than or equal to zero.</exception>
        public Value ToBase(int baseValue)
        {
            if (baseValue <= 0)
                throw new ArgumentException("baseValue must be greater than zero.");

            var result = new List<int>();
            var currentValue = new List<int>(Indices); // Work with a copy of the indices

            while (currentValue.Count > 0 && !IsZero(currentValue))
            {
                var (quotient, remainder) = DivideByBase(currentValue, baseValue);
                result.Insert(0, remainder); // Insert remainder at the front
                currentValue = quotient; // Continue with the quotient
            }

            // If the value is zero, represent it correctly
            if (result.Count == 0)
                result.Add(0);

            return new Value(result, baseValue);
        }

        /// <summary>
        /// Divides a number, represented as a list of indices, by a specified base value.
        /// </summary>
        /// <param name="number">A list of integers representing the number in the current base system.</param>
        /// <param name="baseValue">The base value to divide the number by. Must be greater than zero.</param>
        /// <returns>A tuple containing the quotient as a list of integers and the remainder as an integer.</returns>
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
        /// Determines if a number, represented as a list of integers in a specific base, is zero.
        /// </summary>
        /// <param name="number">The list of integers representing the number to be checked, where each integer is a digit in the given base.</param>
        /// <returns>True if the number is zero; otherwise, false.</returns>
        private static bool IsZero(List<int> number)
        {
            return !number.Any() || number.TrueForAll(digit => digit == 0);
        }
        
        
    }
}