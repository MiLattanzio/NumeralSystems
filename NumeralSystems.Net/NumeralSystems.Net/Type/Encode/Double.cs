using System.Linq;

// ReSharper disable once CheckNamespace
namespace NumeralSystems.Net.Type.Base
{
    public sealed partial class Double
    {
        /// <summary>
        /// Converts a double value to its indices representation in a specified base.
        /// </summary>
        /// <param name="val">The double value to convert.</param>
        /// <param name="destinationBase">The base to convert to.</param>
        /// <returns>A tuple containing the integral part, fractional part, and a boolean indicating if the value is positive.</returns>
        public static (ulong[] Integral, ulong[] Fractional, bool positive) ToIndicesOfBase(double val, int destinationBase)
        {
            var absoluteValue = System.Math.Abs(val);
            var integralPart = (ulong)absoluteValue;
            var fractionalPart = absoluteValue - integralPart;
            var intFractional = GetFractionalPart(fractionalPart, out var zeroCount);
            var zeros = System.Linq.Enumerable.Repeat(0ul, zeroCount).ToArray();
            return (ULong.ToIndicesOfBase(integralPart, destinationBase), zeros.Concat(ULong.ToIndicesOfBase(intFractional, destinationBase)).ToArray(), val >= 0);
        }

        /// <summary>
        /// Converts indices representation in a specified base to a double value.
        /// </summary>
        /// <param name="integral">The integral part indices.</param>
        /// <param name="fractional">The fractional part indices.</param>
        /// <param name="positive">Indicates if the value is positive.</param>
        /// <param name="sourceBase">The base of the indices.</param>
        /// <returns>The double value.</returns>
        public static double FromIndicesOfBase(ulong[] integral, ulong[] fractional, bool positive, int sourceBase)
        {
            var integralPart = ULong.FromIndicesOfBase(integral, sourceBase);
            var fractionalPart = ULong.FromIndicesOfBase(fractional, sourceBase);
            var fractionalPats = ULong.ToIndicesOfBase(fractionalPart, 10);
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
            return positive ? result : -result;
        }

        /// <summary>
        /// Gets the fractional part of a double number as an unsigned long.
        /// </summary>
        /// <param name="number">The double number.</param>
        /// <param name="numberOfZeros">The number of leading zeros in the fractional part.</param>
        /// <returns>The fractional part as an unsigned long.</returns>
        private static ulong GetFractionalPart(double number, out int numberOfZeros)
        {
            // Separiamo la parte intera
            var integerPart = System.Math.Floor(number);

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
            var multiplier = 1ul;
            while ((fractionalPart * multiplier) % 1 != 0)
            {
                multiplier *= 10;
            }

            // Convertiamo la parte frazionaria in intero
            return (ulong)(Decimal.From(fractionalPart) * multiplier);
        }
    }
}