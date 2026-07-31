using System;
using System.Linq;
using System.Numerics;
using NumeralSystems.Net.Type.Incomplete;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class BitPatternTests
    {
        [Test]
        public void MetadataAndBoundedEnumerationAreExact()
        {
            var pattern = new BitPattern(new bool?[] { null, true, false, null });

            Assert.Multiple(() =>
            {
                Assert.That(pattern.Count, Is.EqualTo(4));
                Assert.That(pattern.UnknownBitCount, Is.EqualTo(2));
                Assert.That(pattern.CandidateCount, Is.EqualTo(new BigInteger(4)));
                Assert.That(pattern.MinValue, Is.EqualTo(new BigInteger(2)));
                Assert.That(pattern.MaxValue, Is.EqualTo(new BigInteger(11)));
                Assert.That(pattern.SignedMinValue, Is.EqualTo(new BigInteger(-6)));
                Assert.That(pattern.SignedMaxValue, Is.EqualTo(new BigInteger(3)));
                Assert.That(pattern.IsMatch(2), Is.True);
                Assert.That(pattern.IsMatch(3), Is.True);
                Assert.That(pattern.IsMatch(10), Is.True);
                Assert.That(pattern.IsMatch(11), Is.True);
                Assert.That(pattern.IsMatch(7), Is.False);
                Assert.That(pattern.EnumerateCandidates(2).ToArray(),
                    Is.EqualTo(new[] { new BigInteger(2), new BigInteger(3) }));
                Assert.That(pattern.EnumerateCandidates(0), Is.Empty);
                Assert.That(pattern.EnumerateCandidates(100).Count(), Is.EqualTo(4));
                Assert.That(pattern.ApplyMask(0b0111).MaxValue, Is.EqualTo(new BigInteger(3)));
            });
        }

        [Test]
        public void CandidateCountUsesBigIntegerBeyondPrimitiveWidths()
        {
            var pattern = new BitPattern(Enumerable.Repeat((bool?)null, 128));

            Assert.That(pattern.UnknownBitCount, Is.EqualTo(128));
            Assert.That(pattern.CandidateCount, Is.EqualTo(BigInteger.One << 128));
            Assert.That(pattern.EnumerateCandidates(3).Count(), Is.EqualTo(3));
        }

        [Test]
        public void NegativeEnumerationLimitIsRejected()
        {
            var pattern = new BitPattern(new bool?[] { null });
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                pattern.EnumerateCandidates(-1).ToArray());
        }

        [Test]
        public void CompatibilityAndIntersectionPreserveBothConstraints()
        {
            var left = new BitPattern(new bool?[] { true, null, false, null });
            var right = new BitPattern(new bool?[] { null, true, false, false });
            var contradiction = new BitPattern(new bool?[] { false, null, null, null });

            Assert.Multiple(() =>
            {
                Assert.That(left.IsCompatibleWith(right), Is.True);
                Assert.That(left.TryIntersect(right, out var intersection), Is.True);
                Assert.That(intersection,
                    Is.EqualTo(new BitPattern(new bool?[] { true, true, false, false })));
                Assert.That(left.IsCompatibleWith(contradiction), Is.False);
                Assert.That(left.TryIntersect(contradiction, out _), Is.False);
                Assert.Throws<InvalidOperationException>(() => left.Intersect(contradiction));
            });
        }

        [Test]
        public void AndConstraintSolverDetectsSolutionsAndContradictions()
        {
            var mask = FromByte(0b1111_0000);
            var result = FromByte(0b1010_0000);

            Assert.That(BitPattern.TrySolveAnd(mask, result, out var solution), Is.True);
            Assert.Multiple(() =>
            {
                Assert.That(solution.UnknownBitCount, Is.EqualTo(4));
                Assert.That(solution.IsMatch(0b1010_0000), Is.True);
                Assert.That(solution.IsMatch(0b1010_1111), Is.True);
                Assert.That(solution.IsMatch(0b1110_0000), Is.False);
            });

            var impossible = FromByte(0b0000_0001);
            Assert.That(BitPattern.TrySolveAnd(mask, impossible, out _), Is.False);
            Assert.Throws<InvalidOperationException>(() => BitPattern.SolveAnd(mask, impossible));
        }

        [Test]
        public void IncompleteWrappersDelegateToSharedEngine()
        {
            var left = new IncompleteInt
            {
                Binary = Enumerable.Repeat((bool?)null, 32).ToArray()
            };
            var rightBits = Enumerable.Repeat((bool?)null, 32).ToArray();
            rightBits[0] = true;
            var right = new IncompleteInt { Binary = rightBits };

            Assert.Multiple(() =>
            {
                Assert.That(left.UnknownBitCount, Is.EqualTo(32));
                Assert.That(left.CandidateCount, Is.EqualTo(BigInteger.One << 32));
                Assert.That(left.MinValue, Is.EqualTo(BigInteger.Zero));
                Assert.That(left.MaxValue, Is.EqualTo((BigInteger.One << 32) - 1));
                Assert.That(left.IsCompatibleWith(right), Is.True);
                Assert.That(left.Intersect(right).UnknownBitCount, Is.EqualTo(31));
                Assert.That(left.EnumerateCandidates(5).Count(), Is.EqualTo(5));
                Assert.That(right.RotateLeft(1).Pattern[1], Is.True);
            });
        }

        [Test]
        public void LegacyWrapperOperationsUseSetBasedTernaryLogic()
        {
            var unknownBits = Enumerable.Repeat((bool?)false, 32).ToArray();
            unknownBits[0] = null;
            var unknown = new IncompleteInt { Binary = unknownBits };
            var zero = new NumeralSystems.Net.Type.Base.Int { Value = 0 }.Incomplete();
            var one = new NumeralSystems.Net.Type.Base.Int { Value = 1 }.Incomplete();

            Assert.Multiple(() =>
            {
                Assert.That(zero.And(unknown).Pattern[0], Is.False);
                Assert.That(one.Or(unknown).Pattern[0], Is.True);
                Assert.That(zero.Nand(unknown).Pattern[0], Is.True);
            });

            var left = new NumeralSystems.Net.Type.Base.Int { Value = 0b1100 }.Incomplete();
            var right = new NumeralSystems.Net.Type.Base.Int { Value = 0b1010 }.Incomplete();
            var xorResult = left.Xor(right);
            var nandResult = left.Nand(right);

            Assert.Multiple(() =>
            {
                Assert.That(xorResult.ReverseXor(right, out var reverseXor), Is.True);
                Assert.That(reverseXor.IsMatch(0b1100), Is.True);
                Assert.That(nandResult.ReverseNand(right, out var reverseNand), Is.True);
                Assert.That(reverseNand.IsMatch(0b1100), Is.True);
            });
        }

        [Test]
        public void ReverseNandRejectsAnImpossibleBit()
        {
            var desiredZero = BitPattern.FromUnsigned(0, 1);
            var rightZero = BitPattern.FromUnsigned(0, 1);

            Assert.That(desiredZero.TryReverseNand(rightZero, out _), Is.False);
            Assert.Throws<InvalidOperationException>(() => desiredZero.ReverseNand(rightZero));
        }

        [Test]
        public void CompleteWrappersExposeReverseXorAndReverseNand()
        {
            var left = new NumeralSystems.Net.Type.Base.Int { Value = 0b1100 };
            var right = new NumeralSystems.Net.Type.Base.Int { Value = 0b1010 };
            var xorResult = left.Xor(right);
            var nandResult = left.Nand(right);

            Assert.Multiple(() =>
            {
                Assert.That(xorResult.ReverseXor(right, out var reverseXor), Is.True);
                Assert.That(reverseXor.Contains(left), Is.True);
                Assert.That(nandResult.ReverseNand(right, out var reverseNand), Is.True);
                Assert.That(reverseNand.Contains(left), Is.True);
            });
        }

        [Test]
        public void EveryBytePairSatisfiesForwardReverseAndConstraintOperations()
        {
            for (var leftValue = 0; leftValue <= byte.MaxValue; leftValue++)
            {
                var left = FromByte((byte)leftValue);
                for (var rightValue = 0; rightValue <= byte.MaxValue; rightValue++)
                {
                    var right = FromByte((byte)rightValue);
                    var andValue = (byte)(leftValue & rightValue);
                    var orValue = (byte)(leftValue | rightValue);
                    var xorValue = (byte)(leftValue ^ rightValue);
                    var nandValue = (byte)~andValue;

                    Assert.That(left.And(right).MinValue, Is.EqualTo(new BigInteger(andValue)));
                    Assert.That(left.Or(right).MinValue, Is.EqualTo(new BigInteger(orValue)));
                    Assert.That(left.Xor(right).MinValue, Is.EqualTo(new BigInteger(xorValue)));
                    Assert.That(left.Nand(right).MinValue, Is.EqualTo(new BigInteger(nandValue)));

                    Assert.That(FromByte(andValue).TryReverseAnd(right, out var reverseAnd), Is.True);
                    Assert.That(reverseAnd.IsMatch(leftValue), Is.True);
                    Assert.That(FromByte(orValue).TryReverseOr(right, out var reverseOr), Is.True);
                    Assert.That(reverseOr.IsMatch(leftValue), Is.True);
                    Assert.That(FromByte(xorValue).TryReverseXor(right, out var reverseXor), Is.True);
                    Assert.That(reverseXor.IsMatch(leftValue), Is.True);
                    Assert.That(FromByte(nandValue).TryReverseNand(right, out var reverseNand), Is.True);
                    Assert.That(reverseNand.IsMatch(leftValue), Is.True);
                    Assert.That(BitPattern.TrySolveAnd(right, FromByte(andValue), out var solved), Is.True);
                    Assert.That(solved.IsMatch(leftValue), Is.True);
                }
            }
        }

        [Test]
        public void EveryByteValueSatisfiesShiftAndRotateOperations()
        {
            for (var value = 0; value <= byte.MaxValue; value++)
            {
                var pattern = FromByte((byte)value);
                for (var count = 0; count < 8; count++)
                {
                    var left = (byte)(value << count);
                    var logicalRight = (byte)(value >> count);
                    var signedRight = unchecked((byte)((sbyte)value >> count));
                    var rotateLeft = (byte)((value << count) | (value >> ((8 - count) % 8)));
                    var rotateRight = (byte)((value >> count) | (value << ((8 - count) % 8)));

                    Assert.That(pattern.ShiftLeft(count).MinValue, Is.EqualTo(new BigInteger(left)));
                    Assert.That(pattern.LogicalShiftRight(count).MinValue, Is.EqualTo(new BigInteger(logicalRight)));
                    Assert.That(pattern.ArithmeticShiftRight(count).MinValue, Is.EqualTo(new BigInteger(signedRight)));
                    Assert.That(pattern.RotateLeft(count).MinValue, Is.EqualTo(new BigInteger(rotateLeft)));
                    Assert.That(pattern.RotateRight(count).MinValue, Is.EqualTo(new BigInteger(rotateRight)));
                }
            }
        }

        [Test]
        public void LargerCompletePatternsSatisfyBitwiseAndShiftProperties()
        {
            var random = new Random(0x470);
            var bytes = new byte[16];

            for (var sample = 0; sample < 2_000; sample++)
            {
                random.NextBytes(bytes);
                var leftValue = BitConverter.ToUInt64(bytes, 0);
                var rightValue = BitConverter.ToUInt64(bytes, 8);
                var left = FromUInt64(leftValue);
                var right = FromUInt64(rightValue);
                var count = random.Next(0, 64);

                Assert.That(left.And(right).MinValue, Is.EqualTo(new BigInteger(leftValue & rightValue)));
                Assert.That(left.Or(right).MinValue, Is.EqualTo(new BigInteger(leftValue | rightValue)));
                Assert.That(left.Xor(right).MinValue, Is.EqualTo(new BigInteger(leftValue ^ rightValue)));
                Assert.That(left.Nand(right).MinValue, Is.EqualTo(new BigInteger(~(leftValue & rightValue))));
                Assert.That(left.ShiftLeft(count).MinValue,
                    Is.EqualTo(new BigInteger(unchecked(leftValue << count))));
                Assert.That(left.LogicalShiftRight(count).MinValue,
                    Is.EqualTo(new BigInteger(leftValue >> count)));
                Assert.That(left.ArithmeticShiftRight(count).MinValue,
                    Is.EqualTo(new BigInteger(unchecked((ulong)((long)leftValue >> count)))));
                Assert.That(left.RotateLeft(count).MinValue,
                    Is.EqualTo(new BigInteger(RotateLeft(leftValue, count))));
                Assert.That(left.RotateRight(count).MinValue,
                    Is.EqualTo(new BigInteger(RotateRight(leftValue, count))));
            }
        }

        [Test]
        public void LargerIncompletePatternsSatisfyIntersectionAndOperationProperties()
        {
            var random = new Random(0xB17);

            for (var sample = 0; sample < 1_000; sample++)
            {
                var left = RandomPattern(random, 32);
                var right = RandomPattern(random, 32);
                var expectedCompatibility = Enumerable.Range(0, 32).All(index =>
                    !left[index].HasValue ||
                    !right[index].HasValue ||
                    left[index].Value == right[index].Value);

                Assert.That(left.IsCompatibleWith(right), Is.EqualTo(expectedCompatibility));
                Assert.That(left.TryIntersect(right, out var intersection),
                    Is.EqualTo(expectedCompatibility));

                if (intersection != null)
                {
                    foreach (var candidate in intersection.EnumerateCandidates(16))
                    {
                        Assert.That(left.IsMatch(candidate), Is.True);
                        Assert.That(right.IsMatch(candidate), Is.True);
                    }
                }

                foreach (var leftCandidate in left.EnumerateCandidates(2))
                {
                    foreach (var rightCandidate in right.EnumerateCandidates(2))
                    {
                        Assert.That(left.And(right).IsMatch(leftCandidate & rightCandidate), Is.True);
                        Assert.That(left.Or(right).IsMatch(leftCandidate | rightCandidate), Is.True);
                        Assert.That(left.Xor(right).IsMatch(leftCandidate ^ rightCandidate), Is.True);
                    }
                }
            }
        }

        private static BitPattern FromByte(byte value) =>
            new BitPattern(Enumerable.Range(0, 8).Select(index => (value & (1 << index)) != 0));

        private static BitPattern FromUInt64(ulong value) =>
            new BitPattern(Enumerable.Range(0, 64).Select(index => (value & (1UL << index)) != 0));

        private static BitPattern RandomPattern(Random random, int width) =>
            new BitPattern(Enumerable.Range(0, width).Select(_ =>
            {
                var state = random.Next(0, 3);
                return state == 0 ? (bool?)false : state == 1 ? true : null;
            }));

        private static ulong RotateLeft(ulong value, int count) =>
            count == 0 ? value : (value << count) | (value >> (64 - count));

        private static ulong RotateRight(ulong value, int count) =>
            count == 0 ? value : (value >> count) | (value << (64 - count));
    }
}
