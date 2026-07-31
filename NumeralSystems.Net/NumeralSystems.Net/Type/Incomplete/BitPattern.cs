using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NumeralSystems.Net.Type.Incomplete
{
    /// <summary>
    /// Represents an immutable, fixed-width pattern of known and unknown bits.
    /// </summary>
    /// <remarks>
    /// Bits are stored least-significant bit first, consistently with the primitive
    /// wrappers in NumeralSystems.Net. A <see langword="null"/> bit is unknown.
    /// </remarks>
    public sealed class BitPattern : IReadOnlyList<bool?>, IEquatable<BitPattern>
    {
        private readonly bool?[] _bits;

        /// <summary>
        /// Initializes a pattern from a least-significant-bit-first sequence.
        /// </summary>
        /// <param name="bits">Known and unknown bits.</param>
        public BitPattern(IEnumerable<bool?> bits)
        {
            if (bits == null) throw new ArgumentNullException(nameof(bits));
            _bits = bits.ToArray();
        }

        /// <summary>
        /// Creates a complete pattern from a least-significant-bit-first sequence.
        /// </summary>
        public BitPattern(IEnumerable<bool> bits)
            : this(bits?.Select(bit => (bool?)bit) ?? throw new ArgumentNullException(nameof(bits)))
        {
        }

        /// <summary>
        /// Creates a complete fixed-width pattern from an unsigned encoded value.
        /// </summary>
        public static BitPattern FromUnsigned(BigInteger value, int width)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width), "The width cannot be negative.");
            if (value < BigInteger.Zero)
                throw new ArgumentOutOfRangeException(nameof(value), "The encoded value cannot be negative.");
            if (width == 0 && value != BigInteger.Zero ||
                width > 0 && value >= (BigInteger.One << width))
                throw new ArgumentOutOfRangeException(nameof(value), "The encoded value does not fit the width.");

            return new BitPattern(ToBits(value, width));
        }

        /// <summary>
        /// Creates a fixed-width pattern in which every bit is unknown.
        /// </summary>
        public static BitPattern Unknown(int width)
        {
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width), "The width cannot be negative.");
            return new BitPattern(Enumerable.Repeat((bool?)null, width));
        }

        /// <summary>
        /// Parses a most-significant-bit-first pattern containing <c>0</c>,
        /// <c>1</c>, and <c>?</c>. Underscores and whitespace are ignored.
        /// </summary>
        public static BitPattern Parse(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (TryParse(value, out var pattern)) return pattern;
            throw new FormatException("A bit pattern may contain only 0, 1, ?, underscores, and whitespace.");
        }

        /// <summary>
        /// Attempts to parse a most-significant-bit-first pattern containing
        /// <c>0</c>, <c>1</c>, and <c>?</c>.
        /// </summary>
        public static bool TryParse(string value, out BitPattern pattern)
        {
            pattern = null;
            if (value == null) return false;

            var symbols = value
                .Where(symbol => symbol != '_' && !char.IsWhiteSpace(symbol))
                .ToArray();
            if (symbols.Length == 0 || symbols.Any(symbol => symbol != '0' && symbol != '1' && symbol != '?'))
                return false;

            pattern = new BitPattern(symbols
                .Reverse()
                .Select(symbol => symbol == '?' ? (bool?)null : symbol == '1'));
            return true;
        }

        /// <summary>
        /// Gets the fixed width of the pattern.
        /// </summary>
        public int Count => _bits.Length;

        /// <summary>
        /// Gets a bit by its zero-based, least-significant-bit-first index.
        /// </summary>
        public bool? this[int index] => _bits[index];

        /// <summary>
        /// Gets the number of unknown bits.
        /// </summary>
        public int UnknownBitCount => _bits.Count(bit => bit is null);

        /// <summary>
        /// Gets whether every bit is known.
        /// </summary>
        public bool IsComplete => UnknownBitCount == 0;

        /// <summary>
        /// Gets the exact number of complete values represented by this pattern.
        /// </summary>
        public BigInteger CandidateCount => BigInteger.One << UnknownBitCount;

        /// <summary>
        /// Gets the smallest unsigned encoded value represented by this pattern.
        /// </summary>
        public BigInteger MinValue => ToUnsignedValue(unknownValue: false);

        /// <summary>
        /// Gets the largest unsigned encoded value represented by this pattern.
        /// </summary>
        public BigInteger MaxValue => ToUnsignedValue(unknownValue: true);

        /// <summary>
        /// Gets the smallest two's-complement value represented by this pattern.
        /// </summary>
        public BigInteger SignedMinValue => ToSignedBoundary(findMinimum: true);

        /// <summary>
        /// Gets the largest two's-complement value represented by this pattern.
        /// </summary>
        public BigInteger SignedMaxValue => ToSignedBoundary(findMinimum: false);

        /// <summary>
        /// Returns a defensive copy of the least-significant-bit-first representation.
        /// </summary>
        public bool?[] ToArray() => (bool?[])_bits.Clone();

        /// <summary>
        /// Tests whether a complete bit sequence is represented by this pattern.
        /// </summary>
        public bool IsMatch(IEnumerable<bool> value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var candidate = value as bool[] ?? value.ToArray();
            if (candidate.Length != Count) return false;

            for (var index = 0; index < Count; index++)
            {
                if (_bits[index].HasValue && _bits[index].Value != candidate[index])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Tests whether an unsigned encoded value is represented by this pattern.
        /// </summary>
        public bool IsMatch(BigInteger value)
        {
            if (value < BigInteger.Zero || value > WidthMask) return false;

            for (var index = 0; index < Count; index++)
            {
                if (!_bits[index].HasValue) continue;
                var candidateBit = (value & (BigInteger.One << index)) != BigInteger.Zero;
                if (_bits[index].Value != candidateBit) return false;
            }

            return true;
        }

        /// <summary>
        /// Tests whether a signed two's-complement value is represented by this pattern.
        /// </summary>
        public bool IsSignedMatch(BigInteger value)
        {
            if (Count == 0) return value == BigInteger.Zero;
            var minimum = -(BigInteger.One << (Count - 1));
            var maximum = (BigInteger.One << (Count - 1)) - BigInteger.One;
            if (value < minimum || value > maximum) return false;

            var encoded = value < BigInteger.Zero ? value + (BigInteger.One << Count) : value;
            return IsMatch(encoded);
        }

        /// <summary>
        /// Tests whether two patterns have at least one common complete value.
        /// </summary>
        public bool IsCompatibleWith(BitPattern other)
        {
            EnsureSameWidth(other);
            for (var index = 0; index < Count; index++)
            {
                if (_bits[index].HasValue &&
                    other._bits[index].HasValue &&
                    _bits[index].Value != other._bits[index].Value)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to intersect two patterns.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> and the most specific shared pattern when compatible;
        /// otherwise <see langword="false"/>.
        /// </returns>
        public bool TryIntersect(BitPattern other, out BitPattern intersection)
        {
            EnsureSameWidth(other);
            if (!IsCompatibleWith(other))
            {
                intersection = null;
                return false;
            }

            var bits = new bool?[Count];
            for (var index = 0; index < Count; index++)
                bits[index] = _bits[index] ?? other._bits[index];

            intersection = new BitPattern(bits);
            return true;
        }

        /// <summary>
        /// Intersects two compatible patterns.
        /// </summary>
        /// <exception cref="InvalidOperationException">The patterns contradict each other.</exception>
        public BitPattern Intersect(BitPattern other)
        {
            if (TryIntersect(other, out var intersection)) return intersection;
            throw new InvalidOperationException("The bit patterns do not have a common candidate.");
        }

        /// <summary>
        /// Enumerates at most <paramref name="limit"/> unsigned encoded candidates.
        /// </summary>
        /// <remarks>
        /// Candidate order is deterministic: unknown bits are treated as a binary
        /// counter from the lowest unknown bit to the highest.
        /// </remarks>
        public IEnumerable<BigInteger> EnumerateCandidates(BigInteger limit)
        {
            if (limit < BigInteger.Zero)
                throw new ArgumentOutOfRangeException(nameof(limit), "The candidate limit cannot be negative.");

            var numberToReturn = BigInteger.Min(limit, CandidateCount);
            var knownValue = MinValue;
            var unknownIndexes = UnknownIndexes();

            for (var ordinal = BigInteger.Zero; ordinal < numberToReturn; ordinal++)
            {
                var value = knownValue;
                for (var unknownIndex = 0; unknownIndex < unknownIndexes.Length; unknownIndex++)
                {
                    if ((ordinal & (BigInteger.One << unknownIndex)) != BigInteger.Zero)
                        value |= BigInteger.One << unknownIndexes[unknownIndex];
                }

                yield return value;
            }
        }

        /// <summary>
        /// Enumerates at most <paramref name="limit"/> complete bit arrays.
        /// </summary>
        public IEnumerable<bool[]> EnumerateBitArrays(BigInteger limit)
        {
            foreach (var value in EnumerateCandidates(limit))
                yield return ToBits(value, Count);
        }

        /// <summary>
        /// Computes the three-valued bitwise NOT.
        /// </summary>
        public BitPattern Not() => new BitPattern(_bits.Select(bit => bit.HasValue ? !bit.Value : (bool?)null));

        /// <summary>
        /// Computes the three-valued bitwise AND.
        /// </summary>
        public BitPattern And(BitPattern other) => Combine(other, BitOperation.And);

        /// <summary>
        /// Applies a mask using three-valued bitwise AND.
        /// </summary>
        public BitPattern ApplyMask(BitPattern mask) => And(mask);

        /// <summary>
        /// Applies a complete unsigned mask using this pattern's width.
        /// </summary>
        public BitPattern ApplyMask(BigInteger mask) => And(FromUnsigned(mask, Count));

        /// <summary>
        /// Computes the three-valued bitwise OR.
        /// </summary>
        public BitPattern Or(BitPattern other) => Combine(other, BitOperation.Or);

        /// <summary>
        /// Computes the three-valued bitwise XOR.
        /// </summary>
        public BitPattern Xor(BitPattern other) => Combine(other, BitOperation.Xor);

        /// <summary>
        /// Computes the three-valued bitwise NAND.
        /// </summary>
        public BitPattern Nand(BitPattern other) => Combine(other, BitOperation.Nand);

        /// <summary>
        /// Solves <c>x AND right == this</c>.
        /// </summary>
        public bool TryReverseAnd(BitPattern right, out BitPattern left) =>
            TryReverse(right, BitOperation.And, out left);

        /// <summary>
        /// Solves <c>x OR right == this</c>.
        /// </summary>
        public bool TryReverseOr(BitPattern right, out BitPattern left) =>
            TryReverse(right, BitOperation.Or, out left);

        /// <summary>
        /// Solves <c>x XOR right == this</c>.
        /// </summary>
        public bool TryReverseXor(BitPattern right, out BitPattern left) =>
            TryReverse(right, BitOperation.Xor, out left);

        /// <summary>
        /// Solves <c>x NAND right == this</c>.
        /// </summary>
        public bool TryReverseNand(BitPattern right, out BitPattern left) =>
            TryReverse(right, BitOperation.Nand, out left);

        /// <summary>
        /// Solves <c>x XOR right == this</c>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The constraint has no solution.</exception>
        public BitPattern ReverseXor(BitPattern right)
        {
            if (TryReverseXor(right, out var left)) return left;
            throw new InvalidOperationException("The reverse XOR constraint has no solution.");
        }

        /// <summary>
        /// Solves <c>x NAND right == this</c>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The constraint has no solution.</exception>
        public BitPattern ReverseNand(BitPattern right)
        {
            if (TryReverseNand(right, out var left)) return left;
            throw new InvalidOperationException("The reverse NAND constraint has no solution.");
        }

        /// <summary>
        /// Solves the constraint <c>x AND mask == result</c>.
        /// </summary>
        public static bool TrySolveAnd(BitPattern mask, BitPattern result, out BitPattern solution)
        {
            if (mask == null) throw new ArgumentNullException(nameof(mask));
            if (result == null) throw new ArgumentNullException(nameof(result));
            return result.TryReverseAnd(mask, out solution);
        }

        /// <summary>
        /// Solves the constraint <c>x AND mask == result</c>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The constraint has no solution.</exception>
        public static BitPattern SolveAnd(BitPattern mask, BitPattern result)
        {
            if (TrySolveAnd(mask, result, out var solution)) return solution;
            throw new InvalidOperationException("The AND constraint has no solution.");
        }

        /// <summary>
        /// Solves <c>x AND mask == result</c> for complete encoded operands.
        /// </summary>
        public static bool TrySolveAnd(
            BigInteger mask,
            BigInteger result,
            int width,
            out BitPattern solution) =>
            TrySolveAnd(FromUnsigned(mask, width), FromUnsigned(result, width), out solution);

        /// <summary>
        /// Shifts left, discarding overflowing high bits and introducing known zeroes.
        /// </summary>
        public BitPattern ShiftLeft(int count)
        {
            ValidateShiftCount(count);
            if (count >= Count) return Zeros(Count);

            var result = new bool?[Count];
            for (var index = 0; index < Count; index++)
                result[index] = index < count ? false : _bits[index - count];
            return new BitPattern(result);
        }

        /// <summary>
        /// Shifts left logically. This is an alias of <see cref="ShiftLeft"/>.
        /// </summary>
        public BitPattern LogicalShiftLeft(int count) => ShiftLeft(count);

        /// <summary>
        /// Shifts left arithmetically. Fixed-width left shifts have the same bit
        /// behavior as logical left shifts.
        /// </summary>
        public BitPattern ArithmeticShiftLeft(int count) => ShiftLeft(count);

        /// <summary>
        /// Shifts right logically, introducing known zeroes.
        /// </summary>
        public BitPattern LogicalShiftRight(int count)
        {
            ValidateShiftCount(count);
            if (count >= Count) return Zeros(Count);

            var result = new bool?[Count];
            for (var index = 0; index < Count; index++)
                result[index] = index + count >= Count ? false : _bits[index + count];
            return new BitPattern(result);
        }

        /// <summary>
        /// Shifts right arithmetically, extending the sign bit.
        /// </summary>
        public BitPattern ArithmeticShiftRight(int count)
        {
            ValidateShiftCount(count);
            if (Count == 0) return this;
            var sign = _bits[Count - 1];
            if (count >= Count) return new BitPattern(Enumerable.Repeat(sign, Count));

            var result = new bool?[Count];
            for (var index = 0; index < Count; index++)
                result[index] = index + count >= Count ? sign : _bits[index + count];
            return new BitPattern(result);
        }

        /// <summary>
        /// Rotates bits left inside the fixed pattern width.
        /// </summary>
        public BitPattern RotateLeft(int count)
        {
            ValidateShiftCount(count);
            if (Count == 0) return this;
            count %= Count;

            var result = new bool?[Count];
            for (var index = 0; index < Count; index++)
                result[index] = _bits[(index - count + Count) % Count];
            return new BitPattern(result);
        }

        /// <summary>
        /// Rotates bits right inside the fixed pattern width.
        /// </summary>
        public BitPattern RotateRight(int count)
        {
            ValidateShiftCount(count);
            if (Count == 0) return this;
            count %= Count;

            var result = new bool?[Count];
            for (var index = 0; index < Count; index++)
                result[index] = _bits[(index + count) % Count];
            return new BitPattern(result);
        }

        /// <inheritdoc />
        public IEnumerator<bool?> GetEnumerator() => ((IEnumerable<bool?>)_bits).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public bool Equals(BitPattern other) =>
            other != null && _bits.SequenceEqual(other._bits);

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as BitPattern);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                foreach (var bit in _bits)
                    hash = hash * 31 + (bit.HasValue ? (bit.Value ? 1 : 0) : 2);
                return hash;
            }
        }

        /// <summary>
        /// Formats bits most-significant bit first, using <c>?</c> for unknown bits.
        /// </summary>
        public override string ToString() =>
            string.Concat(_bits.Reverse().Select(bit =>
                bit.HasValue ? (bit.Value ? "1" : "0") : "?"));

        /// <summary>
        /// Compares two patterns by value.
        /// </summary>
        public static bool operator ==(BitPattern left, BitPattern right) =>
            ReferenceEquals(left, right) || left?.Equals(right) == true;

        /// <summary>
        /// Compares two patterns by value.
        /// </summary>
        public static bool operator !=(BitPattern left, BitPattern right) => !(left == right);

        private BigInteger WidthMask =>
            Count == 0 ? BigInteger.Zero : (BigInteger.One << Count) - BigInteger.One;

        private BigInteger ToUnsignedValue(bool unknownValue)
        {
            var value = BigInteger.Zero;
            for (var index = 0; index < Count; index++)
            {
                if (_bits[index] == true || (_bits[index] is null && unknownValue))
                    value |= BigInteger.One << index;
            }
            return value;
        }

        private BigInteger ToSignedBoundary(bool findMinimum)
        {
            if (Count == 0) return BigInteger.Zero;

            var signIndex = Count - 1;
            var sign = _bits[signIndex];
            var useNegative = findMinimum ? sign != false : sign == true;
            var lower = BigInteger.Zero;

            for (var index = 0; index < signIndex; index++)
            {
                var bit = _bits[index];
                var useOne = bit == true || (bit is null && !findMinimum);
                if (useOne) lower |= BigInteger.One << index;
            }

            return useNegative ? lower - (BigInteger.One << signIndex) : lower;
        }

        private int[] UnknownIndexes() =>
            _bits.Select((bit, index) => new { bit, index })
                .Where(item => item.bit is null)
                .Select(item => item.index)
                .ToArray();

        private BitPattern Combine(BitPattern other, BitOperation operation)
        {
            EnsureSameWidth(other);
            var result = new bool?[Count];
            for (var index = 0; index < Count; index++)
                result[index] = EvaluatePatternBit(_bits[index], other._bits[index], operation);
            return new BitPattern(result);
        }

        private bool TryReverse(BitPattern right, BitOperation operation, out BitPattern left)
        {
            EnsureSameWidth(right);
            var result = new bool?[Count];

            for (var index = 0; index < Count; index++)
            {
                var canBeFalse = CanProduce(false, right._bits[index], _bits[index], operation);
                var canBeTrue = CanProduce(true, right._bits[index], _bits[index], operation);

                if (!canBeFalse && !canBeTrue)
                {
                    left = null;
                    return false;
                }

                result[index] = canBeFalse && canBeTrue ? (bool?)null : canBeTrue;
            }

            left = new BitPattern(result);
            return true;
        }

        private static bool CanProduce(bool left, bool? right, bool? desired, BitOperation operation)
        {
            foreach (var rightValue in ValuesOf(right))
            {
                var actual = Evaluate(left, rightValue, operation);
                if (!desired.HasValue || desired.Value == actual) return true;
            }
            return false;
        }

        private static IEnumerable<bool> ValuesOf(bool? bit)
        {
            if (bit.HasValue)
            {
                yield return bit.Value;
                yield break;
            }

            yield return false;
            yield return true;
        }

        private static bool? EvaluatePatternBit(bool? left, bool? right, BitOperation operation)
        {
            bool? common = null;
            var hasValue = false;
            foreach (var leftValue in ValuesOf(left))
            {
                foreach (var rightValue in ValuesOf(right))
                {
                    var value = Evaluate(leftValue, rightValue, operation);
                    if (!hasValue)
                    {
                        common = value;
                        hasValue = true;
                    }
                    else if (common != value)
                    {
                        return null;
                    }
                }
            }
            return common;
        }

        private static bool Evaluate(bool left, bool right, BitOperation operation) => operation switch
        {
            BitOperation.And => left && right,
            BitOperation.Or => left || right,
            BitOperation.Xor => left ^ right,
            BitOperation.Nand => !(left && right),
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };

        private static bool[] ToBits(BigInteger value, int width)
        {
            var bits = new bool[width];
            for (var index = 0; index < width; index++)
                bits[index] = (value & (BigInteger.One << index)) != BigInteger.Zero;
            return bits;
        }

        private static BitPattern Zeros(int width) =>
            new BitPattern(Enumerable.Repeat((bool?)false, width));

        private void EnsureSameWidth(BitPattern other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            if (Count != other.Count)
                throw new ArgumentException("Bit patterns must have the same width.", nameof(other));
        }

        private static void ValidateShiftCount(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "The shift count cannot be negative.");
        }

        private enum BitOperation
        {
            And,
            Or,
            Xor,
            Nand
        }
    }
}
