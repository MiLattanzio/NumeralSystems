using System.IO;
using System.Text.Json;
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
        public void ConvertReadsOneValuePerLineFromStandardInput()
        {
            var result = RunWithInput(
                "\uFEFFFF" + System.Environment.NewLine + "10" + System.Environment.NewLine + System.Environment.NewLine,
                "convert", "--from", "16", "--to", "2");

            Assert.Multiple(() =>
            {
                Assert.That(result.ExitCode, Is.Zero);
                Assert.That(
                    result.Output.Replace("\r", string.Empty).Trim(),
                    Is.EqualTo("11111111\n10000"));
                Assert.That(result.Error, Is.Empty);
            });
        }

        [Test]
        public void ConvertReadsValuesFromAnExplicitFile()
        {
            var path = Path.GetTempFileName();
            try
            {
                File.WriteAllLines(path, new[] { "7F", "80" });

                var result = Run(
                    "convert", "--input", path, "--from", "16", "--to", "10");

                Assert.Multiple(() =>
                {
                    Assert.That(result.ExitCode, Is.Zero);
                    Assert.That(result.Output, Does.Contain("127"));
                    Assert.That(result.Output, Does.Contain("128"));
                    Assert.That(result.Error, Is.Empty);
                });
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Test]
        public void JsonOutputHasAStableEnvelopeAndStringBigIntegers()
        {
            var result = Run(
                "--output", "json", "inspect", "1100????", "--type", "byte", "--explain");

            using var document = JsonDocument.Parse(result.Output);
            var root = document.RootElement;
            var payload = root.GetProperty("result");
            Assert.Multiple(() =>
            {
                Assert.That(result.ExitCode, Is.Zero);
                Assert.That(root.GetProperty("schemaVersion").GetString(), Is.EqualTo("1.0"));
                Assert.That(root.GetProperty("command").GetString(), Is.EqualTo("inspect"));
                Assert.That(root.GetProperty("success").GetBoolean(), Is.True);
                Assert.That(payload.GetProperty("candidateCount").ValueKind, Is.EqualTo(JsonValueKind.String));
                Assert.That(payload.GetProperty("candidateCount").GetString(), Is.EqualTo("16"));
                Assert.That(payload.GetProperty("explanation").GetArrayLength(), Is.EqualTo(8));
                Assert.That(result.Error, Is.Empty);
            });
        }

        [Test]
        public void JsonOutputWorksForPipelinedConversion()
        {
            var result = RunWithInput(
                "FF\n10\n",
                "convert", "--from", "16", "--to", "2", "--output", "json");

            using var document = JsonDocument.Parse(result.Output);
            var payload = document.RootElement.GetProperty("result");
            Assert.Multiple(() =>
            {
                Assert.That(result.ExitCode, Is.Zero);
                Assert.That(payload.GetProperty("count").GetInt32(), Is.EqualTo(2));
                Assert.That(
                    payload.GetProperty("results")[1].GetProperty("result").GetString(),
                    Is.EqualTo("10000"));
            });
        }

        [Test]
        public void JsonErrorsRemainMachineReadableOnStandardError()
        {
            var result = Run("--output", "json", "convert", "ZZ", "--from", "16", "--to", "2");

            using var document = JsonDocument.Parse(result.Error);
            var root = document.RootElement;
            Assert.Multiple(() =>
            {
                Assert.That(result.ExitCode, Is.EqualTo(2));
                Assert.That(result.Output, Is.Empty);
                Assert.That(root.GetProperty("success").GetBoolean(), Is.False);
                Assert.That(root.GetProperty("error").GetProperty("code").GetString(), Is.EqualTo("invalid_input"));
            });
        }

        [Test]
        public void ExplainAlsoDescribesConversionAndInspection()
        {
            var conversion = Run("convert", "FF", "--from", "16", "--to", "2", "--explain");
            var inspection = Run("inspect", "1100????", "--type", "byte", "--explain");

            Assert.Multiple(() =>
            {
                Assert.That(conversion.Output, Does.Contain("FF (base 16) = 255 (base 10)"));
                Assert.That(inspection.Output, Does.Contain("Explanation (MSB to LSB):"));
                Assert.That(inspection.Output, Does.Contain("bit    3: ?"));
            });
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

        private static (int ExitCode, string Output, string Error) RunWithInput(
            string inputText,
            params string[] arguments)
        {
            using var input = new StringReader(inputText);
            using var output = new StringWriter();
            using var error = new StringWriter();
            var exitCode = NumsysApplication.Run(arguments, input, output, error);
            return (exitCode, output.ToString(), error.ToString());
        }
    }
}
