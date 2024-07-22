using System.Linq;

namespace NumeralSystems.Net.Utils.Encode
{
    public class Double
    {
        public static (ulong[] Integral, ulong[] Fractional, bool positive) ToIndicesOfBase(double val, int destinationBase)
        {
            var absoluteValue = System.Math.Abs(val);
            var integralPart = (ulong)absoluteValue;
            var fractionalPart = absoluteValue - integralPart;
            var intFractional = GetFractionalPart(fractionalPart, out var zeroCount);
            var zeros = Enumerable.Repeat(0ul, zeroCount).ToArray();
            return (ULong.ToIndicesOfBase(integralPart, destinationBase), zeros.Concat(ULong.ToIndicesOfBase(intFractional, destinationBase)).ToArray(), val>=0);
        }
        
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
            return positive ? result : - result;
        }
        
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
            return (ulong)(Encode.Decimal.From(fractionalPart) * multiplier);
        }
    }
}