using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

// ReSharper disable once CheckNamespace
namespace NumeralSystems.Net.Type.Base
{
    public partial class Decimal
    {
        public static (decimal[] Integral, decimal[] Fractional, bool positive) ToIndicesOfBase(decimal val, int destinationBase)
        {
            var absoluteValue = System.Math.Abs(val);
            var integralPart = absoluteValue;
            var fractionalPart = absoluteValue - integralPart;
            var intFractional = GetFractionalPart(fractionalPart, out var zeroCount);
            var zeros = Enumerable.Repeat(0m, zeroCount).ToArray();
            return (ToIndicesOfBaseInternal(integralPart, destinationBase), zeros.Concat(ToIndicesOfBaseInternal(intFractional, destinationBase)).ToArray(), val>=0);
        }
        
        private static decimal[] ToIndicesOfBaseInternal(decimal val, int destinationBase)
        {
            if (destinationBase <= 0)
            {
                throw new ArgumentException("Destination base must be greater than 0");
            }
            if (destinationBase == 1)
            {
                if (val == 0) return new decimal[] { 0 }; // Special handling for 0 in base 1
                var array = new List<decimal>();
                for (ulong i = 0; i < val; i++)
                {
                    array.Add(1);
                }
                return array.ToArray();
            }
            if (val == 0) return new decimal[] { 0 };

            var result = new List<decimal>();
            while (val != 0)
            {
                var remainder = val % destinationBase;
                val /= (ulong)destinationBase;
                result.Insert(0, remainder); // Prepend operation using Insert at index 0
            }
            return result.ToArray();
        }
        public static decimal FromIndicesOfBase(decimal[] integral, decimal[] fractional, bool positive, int sourceBase)
        {
            var integralPart = FromIndicesOfBase(integral, sourceBase);
            var fractionalPart = FromIndicesOfBase(fractional, sourceBase);
            var fractionalPats = ToIndicesOfBaseInternal(fractionalPart, 10);
            var result = fractionalPart / Pow(10, fractionalPats.Length);
            var zeros = 0;
            while (fractional.Length > zeros && fractional[zeros] == 0)
            {
                zeros++;
            }
            if (zeros > 0)
            {
                result /= Pow(10, zeros);
            }
            result += integralPart;
            return positive ? result : -result;
        }
        
        private static decimal GetFractionalPart(decimal number, out int numberOfZeros)
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
            var multiplier = 1m;
            while ((fractionalPart * multiplier) % 1 != 0)
            {
                multiplier *= 10;
            }

            // Convertiamo la parte frazionaria in intero
            return (fractionalPart * multiplier);
        }
        
        public static decimal FromIndicesOfBase(decimal[] val, int sourceBase)
        {
            if (sourceBase <= 0)
            {
                throw new ArgumentException("Source base must be greater than 0");
            }
            if (sourceBase == 1)
            {
                return val.Length;
            }

            var result = 0m;
            for (var i = 0; i < val.Length; i++)
            {
                result += val[i] * Pow(sourceBase, val.Length - i - 1);
            }
            return (ulong)result;
        }
        
        
        public static decimal From(double val)
        {
            //Using string to avoid precision loss
            return decimal.Parse(val.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        }
        
        public static decimal Pow(decimal value, int power) {
            if(power == 0) return 1.0M;
            var ret = value;
            if(power > 0) {
                for(var _ = 1; _ < power; _++) ret *= value;
            } else {
                for(var _ = 0; _ <= -power; _++) ret /= value;
            }
            return ret;
        }
        
        public static decimal Log(decimal value, decimal precision = 1e-28m)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be positive.");
            }

            if (value == 1)
            {
                return 0;
            }

            // Initial approximation using change of base formula
            var x = (value - 1) / (value + 1);
            var x2 = x * x;

            decimal result = 0;
            var term = x;
            var n = 1;

            // Iterate to sum the series
            while (term > precision || term < -precision)
            {
                result += term / n;
                term *= x2;
                n += 2;
            }

            return 2 * result;
        }
        
        public static decimal Ceiling(decimal value)
        {
            // Extract the integer part of the value
            decimal integerPart = Math.Truncate(value);
        
            // Check if the value has a fractional part
            if (value > integerPart)
            {
                // If it does, return the integer part + 1
                return integerPart + 1;
            }
        
            // If it doesn't, return the integer part itself
            return integerPart;
        }
        
        
    }
}