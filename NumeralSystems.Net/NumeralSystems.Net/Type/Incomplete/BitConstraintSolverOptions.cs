#nullable enable
using System;
using System.Numerics;

namespace NumeralSystems.Net.Type.Incomplete
{
    /// <summary>Defines explicit resource limits for constraint solving and enumeration.</summary>
    public sealed class BitConstraintSolverOptions
    {
        /// <summary>Gets conservative defaults suitable for library callers.</summary>
        public static BitConstraintSolverOptions Default { get; } = new BitConstraintSolverOptions();

        /// <summary>Creates immutable solver limits.</summary>
        public BitConstraintSolverOptions(
            int maximumConstraints = 1024,
            int maximumBitWidth = 65_536,
            BigInteger? maximumEnumeratedCandidates = null,
            TimeSpan? timeout = null)
        {
            if (maximumConstraints <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(maximumConstraints),
                    "The maximum constraint count must be positive.");
            if (maximumBitWidth <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(maximumBitWidth),
                    "The maximum bit width must be positive.");

            var enumerationLimit = maximumEnumeratedCandidates ?? new BigInteger(256);
            if (enumerationLimit < BigInteger.Zero)
                throw new ArgumentOutOfRangeException(
                    nameof(maximumEnumeratedCandidates),
                    "The candidate enumeration limit cannot be negative.");

            var effectiveTimeout = timeout ?? TimeSpan.FromSeconds(5);
            if (effectiveTimeout < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(
                    nameof(timeout),
                    "The timeout cannot be negative.");

            MaximumConstraints = maximumConstraints;
            MaximumBitWidth = maximumBitWidth;
            MaximumEnumeratedCandidates = enumerationLimit;
            Timeout = effectiveTimeout;
        }

        /// <summary>Gets the greatest number of constraints accepted by one solve.</summary>
        public int MaximumConstraints { get; }

        /// <summary>Gets the greatest fixed bit width accepted by one solve.</summary>
        public int MaximumBitWidth { get; }

        /// <summary>Gets the greatest candidate limit accepted by a solution enumeration.</summary>
        public BigInteger MaximumEnumeratedCandidates { get; }

        /// <summary>Gets the timeout applied independently to solving and enumeration.</summary>
        public TimeSpan Timeout { get; }
    }
}
