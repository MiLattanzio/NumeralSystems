﻿using System.Collections.Generic;
using System.Linq;

namespace NumeralSystems.Net.Utils.Encode
{
    public static class Float
    {
        public static (uint[] Integral, uint[] Fractional, bool positive) ToIndicesOfBase(float val, int destinationBase)
        {
            var absoluteValue = System.Math.Abs(val);
            var integralPart = (int)absoluteValue;
            var fractionalPart = absoluteValue - integralPart;
            var intFractional = GetFractionalPart(fractionalPart, out var zeroCount);
            var zeros = Enumerable.Repeat(0u, zeroCount).ToArray();
            return (UInt.ToIndicesOfBase(integralPart, destinationBase, out var positive), zeros.Concat(UInt.ToIndicesOfBase(intFractional, destinationBase, out _)).ToArray(), positive);
        }
        
        public static float FromIndicesOfBase(uint[] integral, uint[] fractional, bool positive, int sourceBase)
        {
            var integralPart = UInt.FromIndicesOfBase(integral.ToArray(), sourceBase, positive);
            var fractionalPart = UInt.FromIndicesOfBase(fractional, sourceBase, true);
            var fractionalPats = UInt.ToIndicesOfBase(fractionalPart, 10, out _);
            var result = integralPart + fractionalPart / System.Math.Pow(10, fractionalPats.Length);
            return System.Convert.ToSingle(positive ? result : - result);
        }

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
            return (int)(fractionalPart * multiplier);
        }
    }
}