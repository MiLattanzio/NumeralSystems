#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NumeralSystems.Net.Type.Incomplete
{
    /// <summary>Explains the values permitted for one bit of a constraint solution.</summary>
    public sealed class BitConstraintBitExplanation
    {
        internal BitConstraintBitExplanation(
            int bitIndex,
            bool canBeZero,
            bool canBeOne,
            string message,
            IEnumerable<BitConstraint> sources)
        {
            if (bitIndex < 0) throw new ArgumentOutOfRangeException(nameof(bitIndex));
            BitIndex = bitIndex;
            CanBeZero = canBeZero;
            CanBeOne = canBeOne;
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Sources = new ReadOnlyCollection<BitConstraint>(
                (sources ?? throw new ArgumentNullException(nameof(sources))).ToArray());
        }

        /// <summary>Gets the zero-based, least-significant-bit-first index.</summary>
        public int BitIndex { get; }

        /// <summary>Gets whether zero satisfies every constraint at this position.</summary>
        public bool CanBeZero { get; }

        /// <summary>Gets whether one satisfies every constraint at this position.</summary>
        public bool CanBeOne { get; }

        /// <summary>Gets whether no bit value satisfies all constraints at this position.</summary>
        public bool IsContradiction => !CanBeZero && !CanBeOne;

        /// <summary>
        /// Gets the forced value, or <see langword="null"/> when both values are possible
        /// or the position is contradictory.
        /// </summary>
        public bool? RequiredValue => CanBeZero == CanBeOne ? (bool?)null : CanBeOne;

        /// <summary>Gets a human-readable explanation.</summary>
        public string Message { get; }

        /// <summary>Gets the constraints responsible for the conclusion.</summary>
        public IReadOnlyList<BitConstraint> Sources { get; }
    }
}
