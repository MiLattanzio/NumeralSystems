#nullable enable
using System;

namespace NumeralSystems.Net.Type.Incomplete
{
    /// <summary>Describes why a bit-constraint expression could not be parsed.</summary>
    public enum BitConstraintParseErrorReason
    {
        /// <summary>The expression is valid.</summary>
        None,
        /// <summary>The expression is empty.</summary>
        EmptyInput,
        /// <summary>The variable name is missing or invalid.</summary>
        InvalidVariable,
        /// <summary>The bitwise operator is missing or invalid.</summary>
        InvalidOperator,
        /// <summary>The right operand is missing.</summary>
        MissingOperand,
        /// <summary>The right operand is not a valid bit pattern.</summary>
        InvalidOperand,
        /// <summary>The equals sign is missing.</summary>
        MissingEquals,
        /// <summary>The expected result is missing.</summary>
        MissingExpectedResult,
        /// <summary>The expected result is not a valid bit pattern.</summary>
        InvalidExpectedResult,
        /// <summary>The operand and expected result use different widths.</summary>
        WidthMismatch
    }

    /// <summary>
    /// Contains either a parsed <see cref="BitConstraint"/> or a structured error.
    /// </summary>
    public sealed class BitConstraintParseResult
    {
        private BitConstraintParseResult(
            BitConstraint? constraint,
            BitConstraintParseErrorReason errorReason,
            int errorPosition,
            string message)
        {
            Constraint = constraint;
            ErrorReason = errorReason;
            ErrorPosition = errorPosition;
            Message = message;
        }

        /// <summary>Gets whether parsing succeeded.</summary>
        public bool IsSuccess => Constraint is not null;

        /// <summary>Gets the parsed constraint, or <see langword="null"/> on failure.</summary>
        public BitConstraint? Constraint { get; }

        /// <summary>Gets the error category, or <see cref="BitConstraintParseErrorReason.None"/>.</summary>
        public BitConstraintParseErrorReason ErrorReason { get; }

        /// <summary>Gets the zero-based UTF-16 error position, or -1 on success.</summary>
        public int ErrorPosition { get; }

        /// <summary>Gets a human-readable diagnostic.</summary>
        public string Message { get; }

        internal static BitConstraintParseResult Success(BitConstraint constraint) =>
            new BitConstraintParseResult(
                constraint ?? throw new ArgumentNullException(nameof(constraint)),
                BitConstraintParseErrorReason.None,
                -1,
                string.Empty);

        internal static BitConstraintParseResult Failure(
            BitConstraintParseErrorReason reason,
            int position,
            string message) =>
            new BitConstraintParseResult(null, reason, position, message);
    }
}
