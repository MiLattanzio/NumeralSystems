using System.IO;
using NumeralSystems.Net.Tool;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class ToolTests
    {
        [Test]
        public void ConvertMatchesDocumentedExample()
        {
            var result = Run("convert", "FF", "--from", "16", "--to", "2");

            Assert.That(result.ExitCode, Is.Zero);
            Assert.That(result.Output.Trim(), Is.EqualTo("11111111"));
            Assert.That(result.Error, Is.Empty);
        }

        [Test]
        public void InspectReportsBoundedUnknownByteInformation()
        {
            var result = Run("inspect", "1100????", "--type", "byte");

            Assert.That(result.ExitCode, Is.Zero);
            Assert.That(result.Output, Does.Contain("Unknown bits: 4"));
            Assert.That(result.Output, Does.Contain("Candidates: 16"));
            Assert.That(result.Output, Does.Contain("Unsigned range: 192..207"));
            Assert.That(result.Output, Does.Contain("192  0xC0"));
        }

        [Test]
        public void SolveMatchesDocumentedAndConstraint()
        {
            var result = Run("solve", "x & 10101010 = 10001000");

            Assert.That(result.ExitCode, Is.Zero);
            Assert.That(result.Output, Does.Contain("x = 1?0?1?0?"));
            Assert.That(result.Output, Does.Contain("Candidates: 16"));
        }

        [Test]
        public void SolveReturnsDedicatedCodeWhenConstraintIsImpossible()
        {
            var result = Run("solve", "x & 0000 = 0001");

            Assert.That(result.ExitCode, Is.EqualTo(3));
            Assert.That(result.Output.Trim(), Is.EqualTo("No solution."));
        }

        [TestCase("x ^ 00110011 = 1100????", "x = 1111????")]
        [TestCase("x | 00001111 = 10101111", "x = 1010????")]
        [TestCase("x nand 1111 = 0011", "x = 1100")]
        public void SolveSupportsEveryConstraintOperator(string expression, string expected)
        {
            var result = Run("solve", expression);

            Assert.That(result.ExitCode, Is.Zero);
            Assert.That(result.Output, Does.Contain(expected));
            Assert.That(result.Error, Is.Empty);
        }

        [Test]
        public void SolveComposesConstraintsAndExplainsEveryBit()
        {
            var result = Run(
                "solve",
                "x & 10101010 = 10001000; x | 00001111 = 10001111",
                "--explain");

            Assert.Multiple(() =>
            {
                Assert.That(result.ExitCode, Is.Zero);
                Assert.That(result.Output, Does.Contain("x = 10001?0?"));
                Assert.That(result.Output, Does.Contain("Candidates: 4"));
                Assert.That(result.Output, Does.Contain("Explanation (MSB to LSB):"));
                Assert.That(result.Output, Does.Contain("bit    7: 1"));
            });
        }

        [Test]
        public void SolveExplainsContradictionsOnlyWhenRequested()
        {
            var result = Run(
                "solve",
                "x ^ 0000 = 0000; x | 0000 = 0001",
                "--explain");

            Assert.Multiple(() =>
            {
                Assert.That(result.ExitCode, Is.EqualTo(3));
                Assert.That(result.Output, Does.StartWith("No solution."));
                Assert.That(result.Output, Does.Contain("bit    0: !"));
                Assert.That(result.Output, Does.Contain("requires 0"));
            });
        }

        [Test]
        public void SolveEnumeratesOnlyAnExplicitBoundedCandidateCount()
        {
            var result = Run("solve", "x ^ 0000 = ????", "--limit", "2");
            var excessive = Run("solve", "x ^ 0000 = ????", "--limit", "10001");

            Assert.Multiple(() =>
            {
                Assert.That(result.ExitCode, Is.Zero);
                Assert.That(result.Output, Does.Contain("First candidates (limit 2):"));
                Assert.That(result.Output, Does.Contain("  0"));
                Assert.That(result.Output, Does.Contain("  1"));
                Assert.That(excessive.ExitCode, Is.EqualTo(2));
                Assert.That(excessive.Error, Does.Contain("cannot exceed 10000"));
            });
        }

        [Test]
        public void SolveReturnsDedicatedCodeWhenTimeoutExpires()
        {
            var result = Run("solve", "x ^ 0000 = ????", "--timeout", "0");

            Assert.That(result.ExitCode, Is.EqualTo(4));
            Assert.That(result.Error, Does.Contain("timeout"));
        }

        private static (int ExitCode, string Output, string Error) Run(params string[] arguments)
        {
            using var output = new StringWriter();
            using var error = new StringWriter();
            var exitCode = NumsysApplication.Run(arguments, output, error);
            return (exitCode, output.ToString(), error.ToString());
        }
    }
}
