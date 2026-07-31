using System;
using System.Collections.Generic;
using System.Numerics;

namespace NumeralSystems.Net.Type.Incomplete
{
    /// <summary>
    /// Shared bit-pattern behavior for the legacy <c>Incomplete*</c> wrappers.
    /// </summary>
    /// <typeparam name="TSelf">The concrete incomplete wrapper type.</typeparam>
    public abstract class IncompleteBitPattern<TSelf>
        where TSelf : IncompleteBitPattern<TSelf>
    {
        /// <summary>
        /// Gets the wrapper's least-significant-bit-first representation.
        /// </summary>
        protected abstract bool?[] PatternBits { get; }

        /// <summary>
        /// Creates the concrete wrapper from a shared pattern.
        /// </summary>
        protected abstract TSelf FromPattern(BitPattern pattern);

        /// <summary>
        /// Gets an immutable snapshot of this value as a shared <see cref="BitPattern"/>.
        /// </summary>
        public BitPattern Pattern => new BitPattern(PatternBits);

        /// <summary>
        /// Gets the number of unknown bits.
        /// </summary>
        public int UnknownBitCount => Pattern.UnknownBitCount;

        /// <summary>
        /// Gets the exact candidate count without fixed-width integer overflow.
        /// </summary>
        public BigInteger CandidateCount => Pattern.CandidateCount;

        /// <summary>
        /// Gets the smallest unsigned encoded candidate.
        /// </summary>
        public BigInteger MinValue => Pattern.MinValue;

        /// <summary>
        /// Gets the largest unsigned encoded candidate.
        /// </summary>
        public BigInteger MaxValue => Pattern.MaxValue;

        /// <summary>
        /// Gets the smallest candidate when the bits are interpreted as two's complement.
        /// </summary>
        public BigInteger SignedMinValue => Pattern.SignedMinValue;

        /// <summary>
        /// Gets the largest candidate when the bits are interpreted as two's complement.
        /// </summary>
        public BigInteger SignedMaxValue => Pattern.SignedMaxValue;

        /// <summary>
        /// Tests whether an unsigned encoded value is represented by this pattern.
        /// </summary>
        public bool IsMatch(BigInteger value) => Pattern.IsMatch(value);

        /// <summary>
        /// Tests whether a signed two's-complement value is represented by this pattern.
        /// </summary>
        public bool IsSignedMatch(BigInteger value) => Pattern.IsSignedMatch(value);

        /// <summary>
        /// Tests whether a complete bit sequence is represented by this pattern.
        /// </summary>
        public bool IsMatch(IEnumerable<bool> bits) => Pattern.IsMatch(bits);

        /// <summary>
        /// Enumerates at most <paramref name="limit"/> unsigned encoded candidates.
        /// </summary>
        public IEnumerable<BigInteger> EnumerateCandidates(BigInteger limit) =>
            Pattern.EnumerateCandidates(limit);

        /// <summary>
        /// Tests whether this wrapper and another wrapper have a common candidate.
        /// </summary>
        public bool IsCompatibleWith(TSelf other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            return Pattern.IsCompatibleWith(other.Pattern);
        }

        /// <summary>
        /// Tries to intersect two incomplete wrappers.
        /// </summary>
        public bool TryIntersect(TSelf other, out TSelf intersection)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (!Pattern.TryIntersect(other.Pattern, out var result))
            {
                intersection = null;
                return false;
            }

            intersection = FromPattern(result);
            return true;
        }

        /// <summary>
        /// Intersects two compatible incomplete wrappers.
        /// </summary>
        public TSelf Intersect(TSelf other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            return FromPattern(Pattern.Intersect(other.Pattern));
        }

        /// <summary>
        /// Applies a three-valued AND mask.
        /// </summary>
        public TSelf ApplyMask(TSelf mask)
        {
            if (mask == null) throw new ArgumentNullException(nameof(mask));
            return FromPattern(Pattern.ApplyMask(mask.Pattern));
        }

        /// <summary>
        /// Applies a complete unsigned mask using the wrapper's width.
        /// </summary>
        public TSelf ApplyMask(BigInteger mask) => FromPattern(Pattern.ApplyMask(mask));

        /// <summary>
        /// Computes a three-valued NAND operation.
        /// </summary>
        public TSelf Nand(TSelf other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            return FromPattern(Pattern.Nand(other.Pattern));
        }

        /// <summary>
        /// Solves <c>x XOR right == this</c>.
        /// </summary>
        public bool ReverseXor(TSelf right, out TSelf result)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            if (!Pattern.TryReverseXor(right.Pattern, out var solution))
            {
                result = null;
                return false;
            }

            result = FromPattern(solution);
            return true;
        }

        /// <summary>
        /// Solves <c>x NAND right == this</c>.
        /// </summary>
        public bool ReverseNand(TSelf right, out TSelf result)
        {
            if (right == null) throw new ArgumentNullException(nameof(right));
            if (!Pattern.TryReverseNand(right.Pattern, out var solution))
            {
                result = null;
                return false;
            }

            result = FromPattern(solution);
            return true;
        }

        /// <summary>
        /// Solves <c>x AND mask == this</c>.
        /// </summary>
        public bool TrySolveAnd(TSelf mask, out TSelf result)
        {
            if (mask == null) throw new ArgumentNullException(nameof(mask));
            if (!BitPattern.TrySolveAnd(mask.Pattern, Pattern, out var solution))
            {
                result = null;
                return false;
            }

            result = FromPattern(solution);
            return true;
        }

        /// <summary>
        /// Shifts left inside the fixed wrapper width.
        /// </summary>
        public TSelf ShiftLeft(int count) => FromPattern(Pattern.ShiftLeft(count));

        /// <summary>
        /// Shifts left logically inside the fixed wrapper width.
        /// </summary>
        public TSelf LogicalShiftLeft(int count) => FromPattern(Pattern.LogicalShiftLeft(count));

        /// <summary>
        /// Shifts left arithmetically inside the fixed wrapper width.
        /// </summary>
        public TSelf ArithmeticShiftLeft(int count) => FromPattern(Pattern.ArithmeticShiftLeft(count));

        /// <summary>
        /// Shifts right logically inside the fixed wrapper width.
        /// </summary>
        public TSelf LogicalShiftRight(int count) =>
            FromPattern(Pattern.LogicalShiftRight(count));

        /// <summary>
        /// Shifts right arithmetically, extending the highest bit.
        /// </summary>
        public TSelf ArithmeticShiftRight(int count) =>
            FromPattern(Pattern.ArithmeticShiftRight(count));

        /// <summary>
        /// Rotates left inside the fixed wrapper width.
        /// </summary>
        public TSelf RotateLeft(int count) => FromPattern(Pattern.RotateLeft(count));

        /// <summary>
        /// Rotates right inside the fixed wrapper width.
        /// </summary>
        public TSelf RotateRight(int count) => FromPattern(Pattern.RotateRight(count));
    }
}
