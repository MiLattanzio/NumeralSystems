using System;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Type.Base
{
    /// <summary>
    /// Connects complete primitive wrappers to the shared reverse-operation engine.
    /// </summary>
    public abstract class CompleteBitPattern<TSelf, TIncomplete>
        where TSelf : CompleteBitPattern<TSelf, TIncomplete>
        where TIncomplete : IncompleteBitPattern<TIncomplete>
    {
        /// <summary>
        /// Gets the complete least-significant-bit-first representation.
        /// </summary>
        protected abstract bool[] CompletePatternBits { get; }

        /// <summary>
        /// Creates the matching incomplete wrapper.
        /// </summary>
        protected abstract TIncomplete FromPattern(BitPattern pattern);

        /// <summary>
        /// Gets an immutable snapshot of the complete wrapper.
        /// </summary>
        public BitPattern Pattern => new BitPattern(CompletePatternBits);

        /// <summary>
        /// Solves <c>x XOR right == this</c> with a complete right operand.
        /// </summary>
        public bool ReverseXor(TSelf right, out TIncomplete result)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            return TryReverseXor(right.Pattern, out result);
        }

        /// <summary>
        /// Solves <c>x XOR right == this</c> with an incomplete right operand.
        /// </summary>
        public bool ReverseXor(TIncomplete right, out TIncomplete result)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            return TryReverseXor(right.Pattern, out result);
        }

        /// <summary>
        /// Solves <c>x NAND right == this</c> with a complete right operand.
        /// </summary>
        public bool ReverseNand(TSelf right, out TIncomplete result)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            return TryReverseNand(right.Pattern, out result);
        }

        /// <summary>
        /// Solves <c>x NAND right == this</c> with an incomplete right operand.
        /// </summary>
        public bool ReverseNand(TIncomplete right, out TIncomplete result)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            return TryReverseNand(right.Pattern, out result);
        }

        private bool TryReverseXor(BitPattern right, out TIncomplete result)
        {
            if (!Pattern.TryReverseXor(right, out var solution))
            {
                result = null;
                return false;
            }

            result = FromPattern(solution);
            return true;
        }

        private bool TryReverseNand(BitPattern right, out TIncomplete result)
        {
            if (!Pattern.TryReverseNand(right, out var solution))
            {
                result = null;
                return false;
            }

            result = FromPattern(solution);
            return true;
        }
    }
}
