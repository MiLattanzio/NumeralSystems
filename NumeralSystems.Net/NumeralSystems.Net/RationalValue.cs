#nullable enable
using System;
using System.Numerics;

namespace NumeralSystems.Net
{
    /// <summary>
    /// Represents an exact, normalized rational number.
    /// </summary>
    /// <remarks>
    /// The denominator is always positive and the numerator and denominator are
    /// reduced by their greatest common divisor. Zero is always represented as 0/1.
    /// </remarks>
    public sealed class RationalValue : IComparable<RationalValue>, IEquatable<RationalValue>
    {
        /// <summary>Gets the signed numerator.</summary>
        public BigInteger Numerator { get; }

        /// <summary>Gets the positive denominator.</summary>
        public BigInteger Denominator { get; }

        /// <summary>Gets the exact value zero.</summary>
        public static RationalValue Zero { get; } = new RationalValue(BigInteger.Zero, BigInteger.One, true);

        /// <summary>Gets the exact value one.</summary>
        public static RationalValue One { get; } = new RationalValue(BigInteger.One, BigInteger.One, true);

        /// <summary>Gets whether this value is zero.</summary>
        public bool IsZero => Numerator.IsZero;

        /// <summary>Gets whether this value is an integer.</summary>
        public bool IsInteger => Denominator.IsOne;

        /// <summary>Gets the sign of the numerator.</summary>
        public int Sign => Numerator.Sign;

        /// <summary>Creates and normalizes an exact rational value.</summary>
        /// <exception cref="DivideByZeroException">The denominator is zero.</exception>
        public RationalValue(BigInteger numerator, BigInteger denominator)
        {
            if (denominator.IsZero)
                throw new DivideByZeroException("The rational denominator cannot be zero.");

            if (denominator.Sign < 0)
            {
                numerator = -numerator;
                denominator = -denominator;
            }

            if (numerator.IsZero)
            {
                Numerator = BigInteger.Zero;
                Denominator = BigInteger.One;
                return;
            }

            var divisor = BigInteger.GreatestCommonDivisor(BigInteger.Abs(numerator), denominator);
            Numerator = numerator / divisor;
            Denominator = denominator / divisor;
        }

        private RationalValue(BigInteger numerator, BigInteger denominator, bool normalized)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        /// <summary>Creates an exact integer rational value.</summary>
        public static RationalValue FromInteger(BigInteger value) =>
            value.IsZero ? Zero : new RationalValue(value, BigInteger.One, true);

        /// <summary>Creates the exact rational value represented by a <see cref="decimal"/>.</summary>
        public static RationalValue FromDecimal(decimal value)
        {
            var bits = decimal.GetBits(value);
            var scale = (bits[3] >> 16) & 0xFF;
            var numerator = (BigInteger)(uint)bits[0]
                            | (BigInteger)(uint)bits[1] << 32
                            | (BigInteger)(uint)bits[2] << 64;
            if ((bits[3] & int.MinValue) != 0) numerator = -numerator;
            return new RationalValue(numerator, BigInteger.Pow(10, scale));
        }

        /// <summary>Creates the exact rational value encoded by an IEEE 754 double.</summary>
        public static RationalValue FromDouble(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentOutOfRangeException(nameof(value), "The value must be finite.");

            var bits = BitConverter.DoubleToInt64Bits(value);
            var negative = bits < 0;
            var exponentBits = (int)((bits >> 52) & 0x7FF);
            var fractionBits = (ulong)bits & 0x000F_FFFF_FFFF_FFFFUL;
            if (exponentBits == 0 && fractionBits == 0) return Zero;

            BigInteger significand;
            int binaryExponent;
            if (exponentBits == 0)
            {
                significand = fractionBits;
                binaryExponent = -1074;
            }
            else
            {
                significand = fractionBits | 0x0010_0000_0000_0000UL;
                binaryExponent = exponentBits - 1023 - 52;
            }

            var numerator = binaryExponent >= 0 ? significand << binaryExponent : significand;
            var denominator = binaryExponent >= 0 ? BigInteger.One : BigInteger.One << -binaryExponent;
            return new RationalValue(negative ? -numerator : numerator, denominator);
        }

        /// <summary>Creates the exact rational value encoded by an IEEE 754 single.</summary>
        public static RationalValue FromSingle(float value)
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
                throw new ArgumentOutOfRangeException(nameof(value), "The value must be finite.");

            var bits = BitConverter.SingleToInt32Bits(value);
            var negative = bits < 0;
            var exponentBits = (bits >> 23) & 0xFF;
            var fractionBits = (uint)bits & 0x007F_FFFFU;
            if (exponentBits == 0 && fractionBits == 0) return Zero;

            BigInteger significand;
            int binaryExponent;
            if (exponentBits == 0)
            {
                significand = fractionBits;
                binaryExponent = -149;
            }
            else
            {
                significand = fractionBits | 0x0080_0000U;
                binaryExponent = exponentBits - 127 - 23;
            }

            var numerator = binaryExponent >= 0 ? significand << binaryExponent : significand;
            var denominator = binaryExponent >= 0 ? BigInteger.One : BigInteger.One << -binaryExponent;
            return new RationalValue(negative ? -numerator : numerator, denominator);
        }

        /// <summary>
        /// Creates an exact value from positional digits. No floating-point or
        /// <see cref="decimal"/> intermediary is used.
        /// </summary>
        public static RationalValue FromDigits(
            System.Collections.Generic.IEnumerable<int> integral,
            System.Collections.Generic.IEnumerable<int> fractional,
            bool negative,
            int baseValue)
        {
            var ratio = Utils.PositionalNotation.ToRatio(integral, fractional, negative, baseValue);
            return new RationalValue(ratio.Numerator, ratio.Denominator);
        }

        /// <summary>Expands this exact value in a positional base.</summary>
        public NumeralExpansion Expand(int baseValue, NumeralConversionOptions? options = null) =>
            NumeralExpansion.Create(this, baseValue, options ?? NumeralConversionOptions.Default);

        /// <summary>Returns the integer part, truncated toward zero.</summary>
        public BigInteger Truncate() => Numerator / Denominator;

        /// <summary>
        /// Converts the ratio to <see cref="decimal"/> without first converting
        /// its potentially much larger numerator and denominator separately.
        /// </summary>
        public decimal ToDecimal(NumeralRoundingMode roundingMode = NumeralRoundingMode.ToNearestEven)
        {
            if (!Enum.IsDefined(typeof(NumeralRoundingMode), roundingMode))
                throw new ArgumentOutOfRangeException(nameof(roundingMode));
            if (IsZero) return decimal.Zero;

            var magnitude = BigInteger.Abs(Numerator);
            var maximumCoefficient = (BigInteger.One << 96) - BigInteger.One;
            for (var scale = 28; scale >= 0; scale--)
            {
                var scaled = magnitude * BigInteger.Pow(10, scale);
                var coefficient = BigInteger.DivRem(scaled, Denominator, out var remainder);
                if (ShouldRoundDecimal(
                        coefficient,
                        remainder,
                        Denominator,
                        Sign < 0,
                        roundingMode))
                    coefficient++;
                if (coefficient > maximumCoefficient) continue;

                var low = (uint)(coefficient & uint.MaxValue);
                var middle = (uint)((coefficient >> 32) & uint.MaxValue);
                var high = (uint)((coefficient >> 64) & uint.MaxValue);
                return new decimal(
                    unchecked((int)low),
                    unchecked((int)middle),
                    unchecked((int)high),
                    Sign < 0,
                    (byte)scale);
            }

            throw new OverflowException("The exact rational value is outside the range of System.Decimal.");
        }

        /// <summary>Returns the absolute value.</summary>
        public RationalValue Abs() => Sign < 0 ? new RationalValue(-Numerator, Denominator, true) : this;

        /// <summary>Returns this value with the opposite sign.</summary>
        public RationalValue Negate() => IsZero ? this : new RationalValue(-Numerator, Denominator, true);

        /// <summary>Adds two exact rational values.</summary>
        public RationalValue Add(RationalValue other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));
            return new RationalValue(
                Numerator * other.Denominator + other.Numerator * Denominator,
                Denominator * other.Denominator);
        }

        /// <summary>Subtracts two exact rational values.</summary>
        public RationalValue Subtract(RationalValue other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));
            return new RationalValue(
                Numerator * other.Denominator - other.Numerator * Denominator,
                Denominator * other.Denominator);
        }

        /// <summary>Multiplies two exact rational values.</summary>
        public RationalValue Multiply(RationalValue other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));
            return new RationalValue(Numerator * other.Numerator, Denominator * other.Denominator);
        }

        /// <summary>Divides by another exact rational value.</summary>
        public RationalValue Divide(RationalValue other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));
            if (other.IsZero) throw new DivideByZeroException("Cannot divide by a zero rational value.");
            return new RationalValue(Numerator * other.Denominator, Denominator * other.Numerator);
        }

        /// <inheritdoc />
        public int CompareTo(RationalValue? other)
        {
            if (other is null) return 1;
            return (Numerator * other.Denominator).CompareTo(other.Numerator * Denominator);
        }

        /// <inheritdoc />
        public bool Equals(RationalValue? other) =>
            other is not null && Numerator == other.Numerator && Denominator == other.Denominator;

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as RationalValue);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (Numerator.GetHashCode() * 397) ^ Denominator.GetHashCode();
            }
        }

        /// <inheritdoc />
        public override string ToString() => Denominator.IsOne
            ? Numerator.ToString(System.Globalization.CultureInfo.InvariantCulture)
            : Numerator.ToString(System.Globalization.CultureInfo.InvariantCulture) + "/" +
              Denominator.ToString(System.Globalization.CultureInfo.InvariantCulture);

        private static bool ShouldRoundDecimal(
            BigInteger coefficient,
            BigInteger remainder,
            BigInteger denominator,
            bool negative,
            NumeralRoundingMode mode)
        {
            if (remainder.IsZero) return false;
            switch (mode)
            {
                case NumeralRoundingMode.ToZero:
                    return false;
                case NumeralRoundingMode.AwayFromZero:
                    return true;
                case NumeralRoundingMode.ToNegativeInfinity:
                    return negative;
                case NumeralRoundingMode.ToPositiveInfinity:
                    return !negative;
                case NumeralRoundingMode.ToNearestAwayFromZero:
                    return remainder * 2 >= denominator;
                case NumeralRoundingMode.ToNearestEven:
                    var comparison = (remainder * 2).CompareTo(denominator);
                    return comparison > 0 || comparison == 0 && !coefficient.IsEven;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        /// <summary>Adds two exact rational values.</summary>
        public static RationalValue operator +(RationalValue left, RationalValue right) => left.Add(right);

        /// <summary>Subtracts two exact rational values.</summary>
        public static RationalValue operator -(RationalValue left, RationalValue right) => left.Subtract(right);

        /// <summary>Multiplies two exact rational values.</summary>
        public static RationalValue operator *(RationalValue left, RationalValue right) => left.Multiply(right);

        /// <summary>Divides two exact rational values.</summary>
        public static RationalValue operator /(RationalValue left, RationalValue right) => left.Divide(right);

        /// <summary>Negates an exact rational value.</summary>
        public static RationalValue operator -(RationalValue value) => value.Negate();

        /// <summary>Tests exact equality.</summary>
        public static bool operator ==(RationalValue? left, RationalValue? right) => Equals(left, right);

        /// <summary>Tests exact inequality.</summary>
        public static bool operator !=(RationalValue? left, RationalValue? right) => !Equals(left, right);
    }
}
