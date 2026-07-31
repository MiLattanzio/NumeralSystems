#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using NumeralSystems.Net.Utils;

namespace NumeralSystems.Net
{
    /// <summary>
    /// Immutable positional expansion of an exact rational value.
    /// </summary>
    public sealed class NumeralExpansion
    {
        private readonly ReadOnlyCollection<int> _integralDigits;
        private readonly ReadOnlyCollection<int> _fractionalDigits;

        private NumeralExpansion(
            RationalValue value,
            int baseValue,
            List<int> integralDigits,
            List<int> fractionalDigits,
            int? repeatingStartIndex,
            int repeatingLength,
            bool isTerminating,
            bool wasRounded)
        {
            Value = value;
            Base = baseValue;
            _integralDigits = integralDigits.AsReadOnly();
            _fractionalDigits = fractionalDigits.AsReadOnly();
            RepeatingStartIndex = repeatingStartIndex;
            RepeatingLength = repeatingLength;
            IsTerminating = isTerminating;
            WasRounded = wasRounded;
        }

        /// <summary>Gets the exact value from which this expansion was produced.</summary>
        public RationalValue Value { get; }

        /// <summary>Gets the positional base.</summary>
        public int Base { get; }

        /// <summary>Gets the magnitude digits before the radix point.</summary>
        public IReadOnlyList<int> IntegralDigits => _integralDigits;

        /// <summary>Gets the generated magnitude digits after the radix point.</summary>
        public IReadOnlyList<int> FractionalDigits => _fractionalDigits;

        /// <summary>Gets whether the exact value is negative.</summary>
        public bool Negative => Value.Sign < 0;

        /// <summary>Gets whether the expansion terminates in this base.</summary>
        public bool IsTerminating { get; }

        /// <summary>Gets whether a repeated remainder was detected.</summary>
        public bool HasRepeatingPeriod => RepeatingStartIndex.HasValue;

        /// <summary>Gets the zero-based fractional index at which the repeating period starts.</summary>
        public int? RepeatingStartIndex { get; }

        /// <summary>Gets the number of digits in the repeating period, or zero if no period was detected.</summary>
        public int RepeatingLength { get; }

        /// <summary>Gets whether discarded digits were rounded.</summary>
        public bool WasRounded { get; }

        /// <summary>
        /// Gets whether the materialized representation is exact: either terminating
        /// or carrying explicit repeating-period metadata.
        /// </summary>
        public bool IsExact => IsTerminating || HasRepeatingPeriod;

        internal static NumeralExpansion Create(
            RationalValue value,
            int baseValue,
            NumeralConversionOptions options)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            if (options is null) throw new ArgumentNullException(nameof(options));
            PositionalNotation.ValidateBase(baseValue);

            var magnitude = BigInteger.Abs(value.Numerator);
            var integralValue = BigInteger.DivRem(magnitude, value.Denominator, out var remainder);
            var integralDigits = PositionalNotation.ToDigits(integralValue, baseValue);
            var fractionalDigits = new List<int>();
            Dictionary<BigInteger, int>? seenRemainders = options.DetectRepeatingPeriod
                ? new Dictionary<BigInteger, int>()
                : null;
            int? repeatingStart = null;
            var repeatingLength = 0;

            while (!remainder.IsZero && fractionalDigits.Count < options.MaxFractionalDigits)
            {
                if (seenRemainders != null && seenRemainders.TryGetValue(remainder, out var firstIndex))
                {
                    if (!repeatingStart.HasValue)
                    {
                        repeatingStart = firstIndex;
                        repeatingLength = fractionalDigits.Count - firstIndex;
                    }

                    if (options.InfiniteBehavior == InfiniteExpansionBehavior.PreservePeriod)
                        break;
                    if (options.InfiniteBehavior == InfiniteExpansionBehavior.Throw)
                        throw CreateInfiniteExpansionException(value, baseValue);
                }
                else
                {
                    seenRemainders?.Add(remainder, fractionalDigits.Count);
                }

                remainder *= baseValue;
                var digit = BigInteger.DivRem(remainder, value.Denominator, out remainder);
                fractionalDigits.Add((int)digit);
            }

            // A period can close exactly on the configured boundary.
            if (!remainder.IsZero && seenRemainders != null &&
                seenRemainders.TryGetValue(remainder, out var boundaryStart))
            {
                repeatingStart ??= boundaryStart;
                repeatingLength = repeatingLength == 0
                    ? fractionalDigits.Count - boundaryStart
                    : repeatingLength;
            }

            if (remainder.IsZero)
            {
                return new NumeralExpansion(
                    value,
                    baseValue,
                    integralDigits,
                    fractionalDigits,
                    null,
                    0,
                    true,
                    false);
            }

            if (options.InfiniteBehavior == InfiniteExpansionBehavior.Throw)
            {
                if (TerminatesInBase(value.Denominator, baseValue))
                    throw new NumeralExpansionLimitException(
                        $"The terminating expansion of {value} in base {baseValue} requires more than " +
                        $"{options.MaxFractionalDigits} fractional digits.",
                        options.MaxFractionalDigits);
                throw CreateInfiniteExpansionException(value, baseValue);
            }

            if (options.InfiniteBehavior == InfiniteExpansionBehavior.PreservePeriod && !repeatingStart.HasValue)
                throw new NumeralExpansionLimitException(
                    $"The repeating period of {value} in base {baseValue} was not detected within " +
                    $"{options.MaxFractionalDigits} fractional digits.",
                    options.MaxFractionalDigits);

            var wasRounded = options.InfiniteBehavior == InfiniteExpansionBehavior.Round;
            if (wasRounded)
            {
                if (ShouldIncrement(
                        remainder,
                        value.Denominator,
                        value.Sign < 0,
                        options.RoundingMode,
                        fractionalDigits.Count > 0
                            ? fractionalDigits[fractionalDigits.Count - 1]
                            : integralDigits[integralDigits.Count - 1]))
                    IncrementMagnitude(integralDigits, fractionalDigits, baseValue);

                // A rounded finite projection cannot also represent a repeating tail.
                repeatingStart = null;
                repeatingLength = 0;
            }

            return new NumeralExpansion(
                value,
                baseValue,
                integralDigits,
                fractionalDigits,
                repeatingStart,
                repeatingLength,
                false,
                wasRounded);
        }

        /// <summary>Formats the expansion, placing parentheses around a detected period.</summary>
        public string ToString(
            NumeralAlphabet alphabet,
            string numberDecimalSeparator = ".",
            string negativeSign = "-",
            string repeatingStart = "(",
            string repeatingEnd = ")")
        {
            if (alphabet is null) throw new ArgumentNullException(nameof(alphabet));
            if (alphabet.Count < Base)
                throw new ArgumentException("The alphabet must contain at least as many symbols as the base.", nameof(alphabet));
            if (numberDecimalSeparator is null) throw new ArgumentNullException(nameof(numberDecimalSeparator));
            if (negativeSign is null) throw new ArgumentNullException(nameof(negativeSign));
            if (repeatingStart is null) throw new ArgumentNullException(nameof(repeatingStart));
            if (repeatingEnd is null) throw new ArgumentNullException(nameof(repeatingEnd));

            var integral = string.Concat(_integralDigits.Select(index => alphabet[index]));
            if (_fractionalDigits.Count == 0) return (Negative ? negativeSign : string.Empty) + integral;

            string fractional;
            if (RepeatingStartIndex is int periodStart)
            {
                var prefix = string.Concat(_fractionalDigits
                    .Take(periodStart)
                    .Select(index => alphabet[index]));
                var period = string.Concat(_fractionalDigits
                    .Skip(periodStart)
                    .Take(RepeatingLength)
                    .Select(index => alphabet[index]));
                fractional = prefix + repeatingStart + period + repeatingEnd;
            }
            else
            {
                fractional = string.Concat(_fractionalDigits.Select(index => alphabet[index]));
            }

            return (Negative ? negativeSign : string.Empty) + integral + numberDecimalSeparator + fractional;
        }

        /// <inheritdoc />
        public override string ToString() => ToString(NumeralAlphabet.CreateDefault(Base));

        private static InfiniteNumeralExpansionException CreateInfiniteExpansionException(
            RationalValue value,
            int baseValue) =>
            new InfiniteNumeralExpansionException(
                $"The exact value {value} has a non-terminating expansion in base {baseValue}.");

        private static bool TerminatesInBase(BigInteger denominator, int baseValue)
        {
            while (denominator > BigInteger.One)
            {
                var commonFactor = BigInteger.GreatestCommonDivisor(denominator, baseValue);
                if (commonFactor.IsOne) return false;
                denominator /= commonFactor;
            }

            return true;
        }

        private static bool ShouldIncrement(
            BigInteger remainder,
            BigInteger denominator,
            bool negative,
            NumeralRoundingMode mode,
            int lastRetainedDigit)
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
                    return comparison > 0 || comparison == 0 && (lastRetainedDigit & 1) != 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        private static void IncrementMagnitude(
            List<int> integralDigits,
            List<int> fractionalDigits,
            int baseValue)
        {
            for (var index = fractionalDigits.Count - 1; index >= 0; index--)
            {
                if (fractionalDigits[index] + 1 < baseValue)
                {
                    fractionalDigits[index]++;
                    return;
                }

                fractionalDigits[index] = 0;
            }

            for (var index = integralDigits.Count - 1; index >= 0; index--)
            {
                if (integralDigits[index] + 1 < baseValue)
                {
                    integralDigits[index]++;
                    return;
                }

                integralDigits[index] = 0;
            }

            integralDigits.Insert(0, 1);
        }
    }
}
