using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using NumeralSystems.Net.Type.Incomplete;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class BitConstraintTests
    {
        [TestCase("x & 10101010 = 10001000", BitConstraintOperator.And, "1?0?1?0?")]
        [TestCase("x ^ 00110011 = 1100????", BitConstraintOperator.Xor, "1111????")]
        [TestCase("x | 00001111 = 10101111", BitConstraintOperator.Or, "1010????")]
        [TestCase("x nand 1111 = 0011", BitConstraintOperator.Nand, "1100")]
        public void DocumentedOperatorsProduceExactPatterns(
            string expression,
            BitConstraintOperator operation,
            string expectedPattern)
        {
            var constraint = BitConstraint.Parse(expression);

            Assert.Multiple(() =>
            {
                Assert.That(constraint.VariableName, Is.EqualTo("x"));
                Assert.That(constraint.Operation, Is.EqualTo(operation));
                Assert.That(constraint.Solve().ToString(), Is.EqualTo(expectedPattern));
                Assert.That(constraint.ToString(), Is.EqualTo(expression));
            });
        }

        [Test]
        public void StructuredParserReportsPositionAndReason()
        {
            var invalidOperand = BitConstraintParser.Parse("value & 10Z1 = 1001");
            var widthMismatch = BitConstraintParser.Parse("x ^ 101 = 11");

            Assert.Multiple(() =>
            {
                Assert.That(invalidOperand.IsSuccess, Is.False);
                Assert.That(invalidOperand.Constraint, Is.Null);
                Assert.That(invalidOperand.ErrorReason, Is.EqualTo(BitConstraintParseErrorReason.InvalidOperand));
                Assert.That(invalidOperand.ErrorPosition, Is.EqualTo(10));
                Assert.That(widthMismatch.ErrorReason, Is.EqualTo(BitConstraintParseErrorReason.WidthMismatch));
                Assert.That(BitConstraint.TryParse("x + 1 = 1", out _), Is.False);
                Assert.Throws<FormatException>(() => BitConstraint.Parse("x + 1 = 1"));
            });
        }

        [Test]
        public void MultipleConstraintsComposeWithoutCandidateEnumeration()
        {
            var constraints = BitConstraintSet.Parse(
                "x & 10101010 = 10001000;" + Environment.NewLine +
                "x | 00001111 = 10001111");

            var solution = constraints.Solve();

            Assert.Multiple(() =>
            {
                Assert.That(constraints.Count, Is.EqualTo(2));
                Assert.That(constraints.VariableName, Is.EqualTo("x"));
                Assert.That(constraints.Width, Is.EqualTo(8));
                Assert.That(solution.IsSatisfiable, Is.True);
                Assert.That(solution.Pattern?.ToString(), Is.EqualTo("10001?0?"));
                Assert.That(solution.CandidateCount, Is.EqualTo(new BigInteger(4)));
                Assert.That(solution.Explanations, Has.Count.EqualTo(8));
                Assert.That(solution.Explanations[6].RequiredValue, Is.False);
                Assert.That(solution.Explanations[2].RequiredValue, Is.Null);
            });
        }

        [Test]
        public void CompositionReportsTheExactContradictoryBit()
        {
            var constraints = BitConstraintSet.Parse(
                "x ^ 0000 = 0000; x | 0000 = 0001");

            var solution = constraints.Solve();
            var contradiction = solution.Explanations.Single(explanation => explanation.IsContradiction);

            Assert.Multiple(() =>
            {
                Assert.That(solution.IsSatisfiable, Is.False);
                Assert.That(solution.Pattern, Is.Null);
                Assert.That(solution.CandidateCount, Is.EqualTo(BigInteger.Zero));
                Assert.That(contradiction.BitIndex, Is.Zero);
                Assert.That(contradiction.Sources, Has.Count.EqualTo(2));
                Assert.That(contradiction.Message, Does.Contain("requires 0"));
                Assert.That(contradiction.Message, Does.Contain("requires 1"));
                Assert.Throws<InvalidOperationException>(() => solution.GetPatternOrThrow());
            });
        }

        [Test]
        public void IndividuallyImpossibleConstraintIsExplained()
        {
            var solution = BitConstraintSet.Parse("x & 0000 = 0001").Solve();

            Assert.Multiple(() =>
            {
                Assert.That(solution.IsSatisfiable, Is.False);
                Assert.That(solution.Explanations[0].IsContradiction, Is.True);
                Assert.That(solution.Explanations[0].Message, Does.Contain("No bit value satisfies"));
                Assert.That(solution.Explanations[0].Sources, Has.Count.EqualTo(1));
            });
        }

        [Test]
        public void CandidateCountRemainsExactBeyondPrimitiveWidths()
        {
            const int width = 256;
            var unknown = new string('?', width);
            var zero = new string('0', width);
            var solution = BitConstraintSet.Parse($"x ^ {zero} = {unknown}").Solve();

            Assert.Multiple(() =>
            {
                Assert.That(solution.Pattern?.UnknownBitCount, Is.EqualTo(width));
                Assert.That(solution.CandidateCount, Is.EqualTo(BigInteger.One << width));
            });
        }

        [Test]
        public void CandidateEnumerationRequiresAnAllowedExplicitLimit()
        {
            var options = new BitConstraintSolverOptions(maximumEnumeratedCandidates: 2);
            var solution = BitConstraintSet.Parse("x ^ 0000 = ????").Solve(options);

            Assert.Multiple(() =>
            {
                Assert.That(solution.EnumerateCandidates(2).ToArray(),
                    Is.EqualTo(new[] { BigInteger.Zero, BigInteger.One }));
                Assert.Throws<BitConstraintLimitException>(() =>
                    solution.EnumerateCandidates(3).ToArray());
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    solution.EnumerateCandidates(-1).ToArray());
            });
        }

        [Test]
        public void SolverEnforcesCountWidthTimeoutAndCancellationLimits()
        {
            var constraint = BitConstraint.Parse("x ^ 0000 = ????");
            var twoConstraints = new BitConstraintSet(new[] { constraint, constraint });
            var countOptions = new BitConstraintSolverOptions(maximumConstraints: 1);
            var widthOptions = new BitConstraintSolverOptions(maximumBitWidth: 3);
            var timeoutOptions = new BitConstraintSolverOptions(timeout: TimeSpan.Zero);
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            Assert.Multiple(() =>
            {
                Assert.Throws<BitConstraintLimitException>(() => twoConstraints.Solve(countOptions));
                Assert.Throws<BitConstraintLimitException>(() =>
                    new BitConstraintSet(new[] { constraint }).Solve(widthOptions));
                Assert.Throws<BitConstraintTimeoutException>(() =>
                    new BitConstraintSet(new[] { constraint }).Solve(timeoutOptions));
                Assert.Throws<OperationCanceledException>(() =>
                    new BitConstraintSet(new[] { constraint }).Solve(cancellationToken: cancellation.Token));
            });
        }

        [Test]
        public void ConstraintSetsAreImmutableAndRejectMixedVariablesOrWidths()
        {
            var source = new List<BitConstraint> { BitConstraint.Parse("x & 1 = 1") };
            var constraints = new BitConstraintSet(source);
            source.Add(BitConstraint.Parse("x | 1 = 1"));

            Assert.Multiple(() =>
            {
                Assert.That(constraints.Count, Is.EqualTo(1));
                Assert.That(constraints.Add(BitConstraint.Parse("X | 1 = 1")).Count, Is.EqualTo(2));
                Assert.Throws<ArgumentException>(() =>
                    constraints.Add(BitConstraint.Parse("y | 1 = 1")));
                Assert.Throws<ArgumentException>(() =>
                    constraints.Add(BitConstraint.Parse("x | 11 = 11")));
            });
        }

        [Test]
        public void EveryByteValueSatisfiesAllGeneratedOperatorConstraints()
        {
            var operands = new byte[] { 0x00, 0x0F, 0x33, 0x55, 0xAA, 0xF0, 0xFF };

            for (var value = 0; value <= byte.MaxValue; value++)
            {
                foreach (var operand in operands)
                {
                    AssertMatches(value, operand, BitConstraintOperator.And, value & operand);
                    AssertMatches(value, operand, BitConstraintOperator.Or, value | operand);
                    AssertMatches(value, operand, BitConstraintOperator.Xor, value ^ operand);
                    AssertMatches(value, operand, BitConstraintOperator.Nand, ~(value & operand) & 0xFF);
                }
            }
        }

        [Test]
        public void GeneratedLargerConstraintSetsAlwaysContainTheirSourceValue()
        {
            var random = new Random(0x520);
            for (var sample = 0; sample < 1_000; sample++)
            {
                var value = (uint)random.NextInt64(0, (long)uint.MaxValue + 1);
                var firstOperand = (uint)random.NextInt64(0, (long)uint.MaxValue + 1);
                var secondOperand = (uint)random.NextInt64(0, (long)uint.MaxValue + 1);
                var constraints = new BitConstraintSet(new[]
                {
                    CreateConstraint(value, firstOperand, BitConstraintOperator.And, value & firstOperand, 32),
                    CreateConstraint(value, secondOperand, BitConstraintOperator.Xor, value ^ secondOperand, 32)
                });

                var solution = constraints.Solve();
                Assert.That(solution.GetPatternOrThrow().IsMatch(value), Is.True);
            }
        }

        private static void AssertMatches(
            int value,
            int operand,
            BitConstraintOperator operation,
            int expected)
        {
            var constraint = CreateConstraint(value, operand, operation, expected, 8);
            Assert.That(constraint.Solve().IsMatch(value), Is.True);
        }

        private static BitConstraint CreateConstraint(
            BigInteger value,
            BigInteger operand,
            BitConstraintOperator operation,
            BigInteger expected,
            int width)
        {
            _ = value;
            return new BitConstraint(
                "x",
                operation,
                BitPattern.FromUnsigned(operand, width),
                BitPattern.FromUnsigned(expected, width));
        }
    }
}
