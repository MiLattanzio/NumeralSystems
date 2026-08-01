#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace NumeralSystems.Net.Type.Incomplete
{
    /// <summary>Contains an exact composed bit-constraint result and its explanations.</summary>
    public sealed class BitConstraintSolution
    {
        private readonly BitConstraintSolverOptions _options;

        internal BitConstraintSolution(
            BitPattern? pattern,
            IEnumerable<BitConstraintBitExplanation> explanations,
            BitConstraintSolverOptions options)
        {
            Pattern = pattern;
            Explanations = new ReadOnlyCollection<BitConstraintBitExplanation>(
                (explanations ?? throw new ArgumentNullException(nameof(explanations))).ToArray());
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>Gets whether all constraints have at least one common candidate.</summary>
        public bool IsSatisfiable => Pattern is not null;

        /// <summary>
        /// Gets the exact solution pattern, or <see langword="null"/> when contradictory.
        /// </summary>
        public BitPattern? Pattern { get; }

        /// <summary>Gets the exact number of candidates, or zero when contradictory.</summary>
        public BigInteger CandidateCount => Pattern?.CandidateCount ?? BigInteger.Zero;

        /// <summary>Gets one explanation per bit, ordered from least to most significant.</summary>
        public IReadOnlyList<BitConstraintBitExplanation> Explanations { get; }

        /// <summary>Gets the configured upper bound for candidate enumeration.</summary>
        public BigInteger CandidateEnumerationLimit => _options.MaximumEnumeratedCandidates;

        /// <summary>Returns the solution pattern or throws when the constraints contradict.</summary>
        public BitPattern GetPatternOrThrow()
        {
            if (Pattern is not null) return Pattern;
            throw new InvalidOperationException("The composed bit constraints have no solution.");
        }

        /// <summary>
        /// Enumerates at most <paramref name="limit"/> candidates while enforcing both
        /// the configured enumeration limit and timeout.
        /// </summary>
        public IEnumerable<BigInteger> EnumerateCandidates(
            BigInteger limit,
            CancellationToken cancellationToken = default)
        {
            if (limit < BigInteger.Zero)
                throw new ArgumentOutOfRangeException(nameof(limit), "The candidate limit cannot be negative.");
            if (limit > _options.MaximumEnumeratedCandidates)
                throw new BitConstraintLimitException(
                    nameof(BitConstraintSolverOptions.MaximumEnumeratedCandidates),
                    $"The requested candidate limit {limit} exceeds the configured maximum " +
                    $"of {_options.MaximumEnumeratedCandidates}.");

            return EnumerateCandidatesCore(limit, cancellationToken);
        }

        private IEnumerable<BigInteger> EnumerateCandidatesCore(
            BigInteger limit,
            CancellationToken cancellationToken)
        {
            if (Pattern is null) yield break;

            var stopwatch = Stopwatch.StartNew();
            foreach (var candidate in Pattern.EnumerateCandidates(limit))
            {
                cancellationToken.ThrowIfCancellationRequested();
                ThrowIfTimedOut(stopwatch, _options.Timeout);
                yield return candidate;
            }
        }

        internal static void ThrowIfTimedOut(Stopwatch stopwatch, TimeSpan timeout)
        {
            if (stopwatch.Elapsed >= timeout)
                throw new BitConstraintTimeoutException(timeout);
        }
    }
}
