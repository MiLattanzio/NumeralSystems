﻿using System.Linq;

namespace NumeralSystems.Net.Utils.Encode
{
    public class Decimal
    {
        public static (ulong[] Integral, ulong[] Fractional, bool positive) ToIndicesOfBase(decimal val, int destinationBase)
        {
            var absoluteValue = System.Math.Abs(val);
            var integralPart = (ulong)absoluteValue;
            var fractionalPart = absoluteValue - integralPart;
            var intFractional = GetFractionalPart(fractionalPart, out var zeroCount);
            var zeros = Enumerable.Repeat(0ul, zeroCount).ToArray();
            return (ULong.ToIndicesOfBase(integralPart, destinationBase), zeros.Concat(ULong.ToIndicesOfBase(intFractional, destinationBase)).ToArray(), val>0);
        }
        public static decimal FromIndicesOfBase(ulong[] integral, ulong[] fractional, bool positive, int sourceBase)
        {
            var integralPart = ULong.FromIndicesOfBase(integral, sourceBase);
            var fractionalPart = ULong.FromIndicesOfBase(fractional, sourceBase);
            var fractionalPats = ULong.ToIndicesOfBase(fractionalPart, 10);
            var result = integralPart + fractionalPart / System.Math.Pow(10, fractionalPats.Length);
            return new decimal(positive ? result : - result);
        }
        private static ulong GetFractionalPart(decimal number, out int numberOfZeros)
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
            var multiplier = 1;
            while ((fractionalPart * multiplier) % 1 != 0)
            {
                multiplier *= 10;
            }

            // Convertiamo la parte frazionaria in intero
            return (ulong)(fractionalPart * multiplier);
        }
    }
}