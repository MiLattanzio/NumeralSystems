#nullable enable
using System;

namespace NumeralSystems.Net
{
    /// <summary>Specifies how discarded fractional digits are rounded.</summary>
    public enum NumeralRoundingMode
    {
        /// <summary>Round toward zero.</summary>
        ToZero,
        /// <summary>Round away from zero whenever discarded digits are non-zero.</summary>
        AwayFromZero,
        /// <summary>Round toward negative infinity.</summary>
        ToNegativeInfinity,
        /// <summary>Round toward positive infinity.</summary>
        ToPositiveInfinity,
        /// <summary>Round to the nearest value, with midpoint ties going to the even digit.</summary>
        ToNearestEven,
        /// <summary>Round to the nearest value, with midpoint ties going away from zero.</summary>
        ToNearestAwayFromZero
    }

    /// <summary>Specifies how a non-terminating positional expansion is handled.</summary>
    public enum InfiniteExpansionBehavior
    {
        /// <summary>Throw instead of returning an inexact representation.</summary>
        Throw,
        /// <summary>Discard digits after the configured limit.</summary>
        Truncate,
        /// <summary>Round at the configured digit limit.</summary>
        Round,
        /// <summary>Preserve an exact repeating period when it is detected within the limit.</summary>
        PreservePeriod
    }

    /// <summary>
    /// Immutable options for converting an exact rational value to positional digits.
    /// </summary>
    public sealed class NumeralConversionOptions : IEquatable<NumeralConversionOptions>
    {
        /// <summary>Default maximum fractional digit count.</summary>
        public const int DefaultMaxFractionalDigits = 128;

        /// <summary>
        /// Gets the recommended exact-first defaults: detect and preserve a period,
        /// with a 128-digit safety limit.
        /// </summary>
        public static NumeralConversionOptions Default { get; } = new NumeralConversionOptions();

        /// <summary>
        /// Gets the 4.x behavior: generate at most 128 digits and truncate silently.
        /// </summary>
        public static NumeralConversionOptions Legacy { get; } = new NumeralConversionOptions(
            DefaultMaxFractionalDigits,
            NumeralRoundingMode.ToZero,
            false,
            InfiniteExpansionBehavior.Truncate);

        /// <summary>Gets the maximum number of fractional digits to materialize.</summary>
        public int MaxFractionalDigits { get; }

        /// <summary>Gets the rounding rule used when <see cref="InfiniteBehavior"/> is <see cref="InfiniteExpansionBehavior.Round"/>.</summary>
        public NumeralRoundingMode RoundingMode { get; }

        /// <summary>Gets whether repeated remainders are tracked.</summary>
        public bool DetectRepeatingPeriod { get; }

        /// <summary>Gets the policy for a non-terminating expansion.</summary>
        public InfiniteExpansionBehavior InfiniteBehavior { get; }

        /// <summary>Creates immutable conversion options.</summary>
        public NumeralConversionOptions(
            int maxFractionalDigits = DefaultMaxFractionalDigits,
            NumeralRoundingMode roundingMode = NumeralRoundingMode.ToNearestEven,
            bool detectRepeatingPeriod = true,
            InfiniteExpansionBehavior infiniteBehavior = InfiniteExpansionBehavior.PreservePeriod)
        {
            if (maxFractionalDigits < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(maxFractionalDigits),
                    "The maximum number of fractional digits cannot be negative.");
            if (!Enum.IsDefined(typeof(NumeralRoundingMode), roundingMode))
                throw new ArgumentOutOfRangeException(nameof(roundingMode));
            if (!Enum.IsDefined(typeof(InfiniteExpansionBehavior), infiniteBehavior))
                throw new ArgumentOutOfRangeException(nameof(infiniteBehavior));
            if (infiniteBehavior == InfiniteExpansionBehavior.PreservePeriod && !detectRepeatingPeriod)
                throw new ArgumentException(
                    "Period detection must be enabled when preserving repeating periods.",
                    nameof(detectRepeatingPeriod));

            MaxFractionalDigits = maxFractionalDigits;
            RoundingMode = roundingMode;
            DetectRepeatingPeriod = detectRepeatingPeriod;
            InfiniteBehavior = infiniteBehavior;
        }

        /// <summary>Returns a copy with a different fractional digit limit.</summary>
        public NumeralConversionOptions WithMaxFractionalDigits(int value) =>
            new NumeralConversionOptions(value, RoundingMode, DetectRepeatingPeriod, InfiniteBehavior);

        /// <summary>Returns a copy with a different rounding rule.</summary>
        public NumeralConversionOptions WithRoundingMode(NumeralRoundingMode value) =>
            new NumeralConversionOptions(MaxFractionalDigits, value, DetectRepeatingPeriod, InfiniteBehavior);

        /// <summary>Returns a copy with a different infinite-expansion policy.</summary>
        public NumeralConversionOptions WithInfiniteBehavior(
            InfiniteExpansionBehavior value,
            bool? detectRepeatingPeriod = null) =>
            new NumeralConversionOptions(
                MaxFractionalDigits,
                RoundingMode,
                detectRepeatingPeriod ?? DetectRepeatingPeriod,
                value);

        /// <inheritdoc />
        public bool Equals(NumeralConversionOptions? other) =>
            other is not null &&
            MaxFractionalDigits == other.MaxFractionalDigits &&
            RoundingMode == other.RoundingMode &&
            DetectRepeatingPeriod == other.DetectRepeatingPeriod &&
            InfiniteBehavior == other.InfiniteBehavior;

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as NumeralConversionOptions);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = MaxFractionalDigits;
                hashCode = (hashCode * 397) ^ (int)RoundingMode;
                hashCode = (hashCode * 397) ^ DetectRepeatingPeriod.GetHashCode();
                return (hashCode * 397) ^ (int)InfiniteBehavior;
            }
        }
    }
}
