#nullable enable
using System;

namespace NumeralSystems.Net.Type.Incomplete
{
    /// <summary>Parses expressions such as <c>x &amp; 1010 = 1000</c>.</summary>
    public static class BitConstraintParser
    {
        /// <summary>
        /// Parses a single constraint without throwing for malformed input.
        /// </summary>
        public static BitConstraintParseResult Parse(string? expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return Failure(BitConstraintParseErrorReason.EmptyInput, 0, "A constraint expression is required.");

            var index = 0;
            SkipWhitespace(expression, ref index);
            var variableStart = index;
            if (index >= expression.Length || !IsIdentifierStart(expression[index]))
                return Failure(
                    BitConstraintParseErrorReason.InvalidVariable,
                    index,
                    "A variable name must start with a letter or underscore.");

            index++;
            while (index < expression.Length && IsIdentifierPart(expression[index])) index++;
            var variableName = expression.Substring(variableStart, index - variableStart);

            SkipWhitespace(expression, ref index);
            if (!TryReadOperator(expression, ref index, out var operation))
                return Failure(
                    BitConstraintParseErrorReason.InvalidOperator,
                    index,
                    "Expected one of &, |, ^, or nand after the variable name.");

            SkipWhitespace(expression, ref index);
            var operandStart = index;
            var equalsIndex = expression.IndexOf('=', index);
            if (equalsIndex < 0)
                return Failure(
                    BitConstraintParseErrorReason.MissingEquals,
                    expression.Length,
                    "A constraint must contain an equals sign.");

            var operandText = expression.Substring(operandStart, equalsIndex - operandStart).Trim();
            if (operandText.Length == 0)
                return Failure(
                    BitConstraintParseErrorReason.MissingOperand,
                    operandStart,
                    "A bit-pattern operand is required before the equals sign.");
            if (!BitPattern.TryParse(operandText, out var operand))
                return Failure(
                    BitConstraintParseErrorReason.InvalidOperand,
                    FirstInvalidPatternCharacter(expression, operandStart, equalsIndex),
                    "The operand may contain only 0, 1, ?, underscores, and whitespace.");

            var expectedStart = equalsIndex + 1;
            var expectedText = expression.Substring(expectedStart).Trim();
            if (expectedText.Length == 0)
                return Failure(
                    BitConstraintParseErrorReason.MissingExpectedResult,
                    expectedStart,
                    "A bit-pattern result is required after the equals sign.");
            if (!BitPattern.TryParse(expectedText, out var expectedResult))
                return Failure(
                    BitConstraintParseErrorReason.InvalidExpectedResult,
                    FirstInvalidPatternCharacter(expression, expectedStart, expression.Length),
                    "The expected result may contain only 0, 1, ?, underscores, and whitespace.");

            if (operand.Count != expectedResult.Count)
                return Failure(
                    BitConstraintParseErrorReason.WidthMismatch,
                    expectedStart,
                    $"The operand has width {operand.Count}, but the expected result has width {expectedResult.Count}.");

            return BitConstraintParseResult.Success(
                new BitConstraint(variableName, operation, operand, expectedResult));
        }

        private static BitConstraintParseResult Failure(
            BitConstraintParseErrorReason reason,
            int position,
            string message) =>
            BitConstraintParseResult.Failure(reason, position, message);

        private static bool TryReadOperator(
            string expression,
            ref int index,
            out BitConstraintOperator operation)
        {
            operation = default;
            if (index >= expression.Length) return false;

            switch (expression[index])
            {
                case '&':
                    operation = BitConstraintOperator.And;
                    index++;
                    return true;
                case '|':
                    operation = BitConstraintOperator.Or;
                    index++;
                    return true;
                case '^':
                    operation = BitConstraintOperator.Xor;
                    index++;
                    return true;
            }

            const string nand = "nand";
            if (expression.Length - index < nand.Length ||
                !string.Equals(
                    expression.Substring(index, nand.Length),
                    nand,
                    StringComparison.OrdinalIgnoreCase))
                return false;

            var end = index + nand.Length;
            if (end < expression.Length && IsIdentifierPart(expression[end])) return false;
            index = end;
            operation = BitConstraintOperator.Nand;
            return true;
        }

        private static int FirstInvalidPatternCharacter(string expression, int start, int end)
        {
            for (var index = start; index < end; index++)
            {
                var symbol = expression[index];
                if (symbol != '0' && symbol != '1' && symbol != '?' &&
                    symbol != '_' && !char.IsWhiteSpace(symbol))
                    return index;
            }

            return start;
        }

        private static bool IsIdentifierStart(char value) => value == '_' || char.IsLetter(value);

        private static bool IsIdentifierPart(char value) =>
            value == '_' || char.IsLetterOrDigit(value);

        private static void SkipWhitespace(string expression, ref int index)
        {
            while (index < expression.Length && char.IsWhiteSpace(expression[index])) index++;
        }
    }
}
