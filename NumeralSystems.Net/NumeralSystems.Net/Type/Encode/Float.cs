using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace NumeralSystems.Net.Type.Base
{
    public sealed partial class Float
    {
        /// <summary>
        /// Converts a float value to its indices representation in a specified base.
        /// </summary>
        /// <param name="val">The float value to convert.</param>
        /// <param name="destinationBase">The base to convert to.</param>
        /// <returns>A tuple containing the integral part, fractional part, and a boolean indicating if the value is positive.</returns>
        public static (uint[] Integral, uint[] Fractional, bool positive) ToIndicesOfBase(float val, int destinationBase)
        {
            var absoluteValue = System.Math.Abs(val);
            var integralPart = (int)absoluteValue;
            var fractionalPart = absoluteValue - integralPart;
            var intFractional = GetFractionalPart(fractionalPart, out var zeroCount);
            var zeros = System.Linq.Enumerable.Repeat(0u, zeroCount).ToArray();
            return (UInt.ToIndicesOfBase(integralPart, destinationBase, out var positive), zeros.Concat(UInt.ToIndicesOfBase(intFractional, destinationBase, out _)).ToArray(), val >= 0);
        }

        /// <summary>
        /// Converts indices representation in a specified base to a float value.
        /// </summary>
        /// <param name="integral">The integral part indices.</param>
        /// <param name="fractional">The fractional part indices.</param>
        /// <param name="positive">Indicates if the value is positive.</param>
        /// <param name="sourceBase">The base of the indices.</param>
        /// <returns>The float value.</returns>
        public static float FromIndicesOfBase(uint[] integral, uint[] fractional, bool positive, int sourceBase)
        {
            var integralPart = UInt.FromIndicesOfBase(integral.ToArray(), sourceBase, positive);
            var fractionalPart = UInt.FromIndicesOfBase(fractional, sourceBase, true);
            var fractionalPats = UInt.ToIndicesOfBase(fractionalPart, 10, out _);
            var result = fractionalPart / System.Math.Pow(10, fractionalPats.Length);
            var zeros = 0;
            while (fractional.Length > zeros && fractional[zeros] == 0)
            {
                zeros++;
            }
            if (zeros > 0)
            {
                result /= System.Math.Pow(10, zeros);
            }
            result += integralPart;
            return System.Convert.ToSingle(result);
        }

        /// <summary>
        /// Gets the fractional part of a float number as an integer.
        /// </summary>
        /// <param name="number">The float number.</param>
        /// <param name="numberOfZeros">The number of leading zeros in the fractional part.</param>
        /// <returns>The fractional part as an integer.</returns>
        private static int GetFractionalPart(float number, out int numberOfZeros)
        {
            // Separiamo la parte intera
            var integerPart = (float)System.Math.Floor(number);

            // Otteniamo la parte frazionaria
            var fractionalPart = number - integerPart;

            // Contiamo il numero di zeri iniziali
            numberOfZeros = 0;
            while (fractionalPart > 0 && fractionalPart * 10 < 1)
            {
                fractionalPart *= 10;
                numberOfZeros++;
            }

            // Calcoliamo il numero di cifre della parte frazionaria moltiplicando per 10 finché non otteniamo un intero
            var multiplier = 1;
            while ((fractionalPart * multiplier) % 1 != 0)
            {
                multiplier *= 10;
            }

            // Convertiamo la parte frazionaria in intero
            return (int)(Decimal.From(fractionalPart) * multiplier);
        }
    }
}