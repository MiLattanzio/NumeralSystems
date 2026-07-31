using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NumeralSystems.Net.Utils
{
    internal static class PositionalNotation
    {
        internal const int DefaultMaxFractionalDigits = 128;

        internal static BigInteger FromDigits(IEnumerable<int> digits, int baseValue)
        {
            ValidateBase(baseValue);
            if (digits is null) throw new ArgumentNullException(nameof(digits));

            var result = BigInteger.Zero;
            foreach (var digit in digits)
            {
                ValidateDigit(digit, baseValue, nameof(digits));
                result = result * baseValue + digit;
            }

            return result;
        }

        internal static List<int> ToDigits(BigInteger value, int baseValue)
        {
            ValidateBase(baseValue);
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "The digit magnitude cannot be negative.");
            if (value.IsZero) return new List<int> { 0 };

            var result = new List<int>();
            while (value > 0)
            {
                value = BigInteger.DivRem(value, baseValue, out var remainder);
                result.Add((int)remainder);
            }

            result.Reverse();
            return result;
        }

        internal static (BigInteger Numerator, BigInteger Denominator) FractionToRatio(
            IEnumerable<int> digits,
            int baseValue)
        {
            ValidateBase(baseValue);
            if (digits is null) throw new ArgumentNullException(nameof(digits));

            var digitList = digits.ToList();
            var numerator = FromDigits(digitList, baseValue);
            var denominator = BigInteger.Pow(baseValue, digitList.Count);
            return Reduce(numerator, denominator);
        }

        internal static List<int> FractionFromRatio(
            BigInteger numerator,
            BigInteger denominator,
            int baseValue,
            int maxFractionalDigits,
            out bool exact)
        {
            ValidateBase(baseValue);
            ValidateFractionalDigitLimit(maxFractionalDigits);
            if (numerator < 0)
                throw new ArgumentOutOfRangeException(nameof(numerator), "The fractional numerator cannot be negative.");
            if (denominator <= 0)
                throw new ArgumentOutOfRangeException(nameof(denominator), "The denominator must be positive.");

            numerator %= denominator;
            var result = new List<int>();
            while (!numerator.IsZero && result.Count < maxFractionalDigits)
            {
                numerator *= baseValue;
                var digit = BigInteger.DivRem(numerator, denominator, out numerator);
                result.Add((int)digit);
            }

            exact = numerator.IsZero;
            return result;
        }

        internal static decimal ToDecimal(
            IEnumerable<int> integral,
            IEnumerable<int> fractional,
            bool positive,
            int baseValue)
        {
            ValidateBase(baseValue);
            if (integral is null) throw new ArgumentNullException(nameof(integral));
            if (fractional is null) throw new ArgumentNullException(nameof(fractional));

            decimal integralValue = 0;
            foreach (var digit in integral)
            {
                ValidateDigit(digit, baseValue, nameof(integral));
                integralValue = checked(integralValue * baseValue + digit);
            }

            decimal fractionalValue = 0;
            foreach (var digit in fractional.Reverse())
            {
                ValidateDigit(digit, baseValue, nameof(fractional));
                fractionalValue = (fractionalValue + digit) / baseValue;
            }

            var result = integralValue + fractionalValue;
            return positive ? result : -result;
        }

        internal static double ToDouble(
            IEnumerable<ulong> integral,
            IEnumerable<ulong> fractional,
            bool positive,
            int baseValue)
        {
            ValidateBase(baseValue);
            if (integral is null) throw new ArgumentNullException(nameof(integral));
            if (fractional is null) throw new ArgumentNullException(nameof(fractional));

            double integralValue = 0;
            foreach (var digit in integral)
            {
                ValidateDigit(digit, baseValue, nameof(integral));
                integralValue = integralValue * baseValue + digit;
            }

            double fractionalValue = 0;
            foreach (var digit in fractional.Reverse())
            {
                ValidateDigit(digit, baseValue, nameof(fractional));
                fractionalValue = (fractionalValue + digit) / baseValue;
            }

            var result = integralValue + fractionalValue;
            return positive ? result : -result;
        }

        internal static (
            List<int> Integral,
            List<int> Fractional,
            bool Positive,
            bool Exact) FromDecimal(
            decimal value,
            int destinationBase,
            int maxFractionalDigits = DefaultMaxFractionalDigits)
        {
            ValidateBase(destinationBase);
            ValidateFractionalDigitLimit(maxFractionalDigits);

            var bits = decimal.GetBits(value);
            var scale = (bits[3] >> 16) & 0xFF;
            var magnitude = (BigInteger)(uint)bits[0]
                            | (BigInteger)(uint)bits[1] << 32
                            | (BigInteger)(uint)bits[2] << 64;
            var denominator = BigInteger.Pow(10, scale);
            var integralValue = BigInteger.DivRem(magnitude, denominator, out var fractionalNumerator);
            var fractional = FractionFromRatio(
                fractionalNumerator,
                denominator,
                destinationBase,
                maxFractionalDigits,
                out var exact);

            return (
                ToDigits(integralValue, destinationBase),
                fractional,
                value >= 0,
                exact);
        }

        internal static void ValidateBase(int baseValue)
        {
            if (baseValue < 2)
                throw new ArgumentOutOfRangeException(nameof(baseValue), "Base must be at least 2.");
        }

        internal static void ValidateFractionalDigitLimit(int maxFractionalDigits)
        {
            if (maxFractionalDigits < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(maxFractionalDigits),
                    "The maximum number of fractional digits cannot be negative.");
        }

        private static (BigInteger Numerator, BigInteger Denominator) Reduce(
            BigInteger numerator,
            BigInteger denominator)
        {
            if (numerator.IsZero) return (BigInteger.Zero, BigInteger.One);

            var divisor = BigInteger.GreatestCommonDivisor(numerator, denominator);
            return (numerator / divisor, denominator / divisor);
        }

        private static void ValidateDigit(long digit, int baseValue, string parameterName)
        {
            if (digit < 0 || digit >= baseValue)
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    $"Every digit must be within the range [0,{baseValue - 1}].");
        }

        private static void ValidateDigit(ulong digit, int baseValue, string parameterName)
        {
            if (digit >= (ulong)baseValue)
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    $"Every digit must be within the range [0,{baseValue - 1}].");
        }
    }
}
