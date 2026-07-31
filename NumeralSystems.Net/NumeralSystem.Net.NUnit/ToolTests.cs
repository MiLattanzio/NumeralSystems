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

        private static (int ExitCode, string Output, string Error) Run(params string[] arguments)
        {
            using var output = new StringWriter();
            using var error = new StringWriter();
            var exitCode = NumsysApplication.Run(arguments, output, error);
            return (exitCode, output.ToString(), error.ToString());
        }
    }
}
