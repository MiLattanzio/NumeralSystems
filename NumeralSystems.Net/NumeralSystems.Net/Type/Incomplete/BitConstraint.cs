#nullable enable
using System;

namespace NumeralSystems.Net.Type.Incomplete
{
    /// <summary>
    /// Represents an immutable constraint of the form
    /// <c>variable OP operand = expectedResult</c>.
    /// </summary>
    public sealed class BitConstraint : IEquatable<BitConstraint>
    {
        /// <summary>Creates a fixed-width bitwise constraint.</summary>
        public BitConstraint(
            string variableName,
            BitConstraintOperator operation,
            BitPattern operand,
            BitPattern expectedResult)
        {
            if (string.IsNullOrWhiteSpace(variableName))
                throw new ArgumentException("A variable name is required.", nameof(variableName));
            if (!IsValidIdentifier(variableName))
                throw new ArgumentException(
                    "A variable name must start with a letter or underscore and contain only letters, digits, or underscores.",
                    nameof(variableName));
            if (!Enum.IsDefined(typeof(BitConstraintOperator), operation))
                throw new ArgumentOutOfRangeException(nameof(operation));

            Operand = operand ?? throw new ArgumentNullException(nameof(operand));
            ExpectedResult = expectedResult ?? throw new ArgumentNullException(nameof(expectedResult));
            if (Operand.Count != ExpectedResult.Count)
                throw new ArgumentException("The operand and expected result must have the same width.");

            VariableName = variableName;
            Operation = operation;
        }

        /// <summary>Gets the constrained variable name.</summary>
        public string VariableName { get; }

        /// <summary>Gets the bitwise operation.</summary>
        public BitConstraintOperator Operation { get; }

        /// <summary>Gets the right-hand operand.</summary>
        public BitPattern Operand { get; }

        /// <summary>Gets the required operation result.</summary>
        public BitPattern ExpectedResult { get; }

        /// <summary>Gets the fixed bit width.</summary>
        public int Width => Operand.Count;

        /// <summary>Parses one constraint or throws <see cref="FormatException"/>.</summary>
        public static BitConstraint Parse(string expression)
        {
            if (expression is null) throw new ArgumentNullException(nameof(expression));
            var result = BitConstraintParser.Parse(expression);
            if (result.IsSuccess) return result.Constraint!;
            throw new FormatException($"{result.Message} Position: {result.ErrorPosition}.");
        }

        /// <summary>Attempts to parse one constraint.</summary>
        public static bool TryParse(string? expression, out BitConstraint? constraint)
        {
            var result = BitConstraintParser.Parse(expression);
            constraint = result.Constraint;
            return result.IsSuccess;
        }

        /// <summary>
        /// Tries to obtain the exact per-bit solution without enumerating candidates.
        /// </summary>
        public bool TrySolve(out BitPattern? solution)
        {
            var bits = new bool?[Width];
            for (var bitIndex = 0; bitIndex < Width; bitIndex++)
            {
                GetAllowedValues(bitIndex, out var canBeZero, out var canBeOne);
                if (!canBeZero && !canBeOne)
                {
                    solution = null;
                    return false;
                }

                bits[bitIndex] = canBeZero && canBeOne ? (bool?)null : canBeOne;
            }

            solution = new BitPattern(bits);
            return true;
        }

        /// <summary>
        /// Obtains the exact per-bit solution without enumerating candidates.
        /// </summary>
        public BitPattern Solve()
        {
            if (TrySolve(out var solution)) return solution!;
            throw new InvalidOperationException($"The constraint '{this}' has no solution.");
        }

        /// <inheritdoc />
        public bool Equals(BitConstraint? other) =>
            other is not null &&
            string.Equals(VariableName, other.VariableName, StringComparison.OrdinalIgnoreCase) &&
            Operation == other.Operation &&
            Operand.Equals(other.Operand) &&
            ExpectedResult.Equals(other.ExpectedResult);

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as BitConstraint);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = StringComparer.OrdinalIgnoreCase.GetHashCode(VariableName);
                hash = hash * 31 + Operation.GetHashCode();
                hash = hash * 31 + Operand.GetHashCode();
                hash = hash * 31 + ExpectedResult.GetHashCode();
                return hash;
            }
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{VariableName} {OperatorText(Operation)} {Operand} = {ExpectedResult}";

        /// <summary>Compares two constraints by value.</summary>
        public static bool operator ==(BitConstraint? left, BitConstraint? right) =>
            ReferenceEquals(left, right) || left?.Equals(right) == true;

        /// <summary>Compares two constraints by value.</summary>
        public static bool operator !=(BitConstraint? left, BitConstraint? right) => !(left == right);

        internal void GetAllowedValues(int bitIndex, out bool canBeZero, out bool canBeOne)
        {
            if (bitIndex < 0 || bitIndex >= Width)
                throw new ArgumentOutOfRangeException(nameof(bitIndex));

            canBeZero = CanProduce(false, Operand[bitIndex], ExpectedResult[bitIndex]);
            canBeOne = CanProduce(true, Operand[bitIndex], ExpectedResult[bitIndex]);
        }

        private bool CanProduce(bool left, bool? right, bool? expected)
        {
            if (right.HasValue)
                return !expected.HasValue || Evaluate(left, right.Value) == expected.Value;

            return !expected.HasValue ||
                   Evaluate(left, false) == expected.Value ||
                   Evaluate(left, true) == expected.Value;
        }

        private bool Evaluate(bool left, bool right) => Operation switch
        {
            BitConstraintOperator.And => left && right,
            BitConstraintOperator.Or => left || right,
            BitConstraintOperator.Xor => left ^ right,
            BitConstraintOperator.Nand => !(left && right),
            _ => throw new ArgumentOutOfRangeException(nameof(Operation))
        };

        private static string OperatorText(BitConstraintOperator operation) => operation switch
        {
            BitConstraintOperator.And => "&",
            BitConstraintOperator.Or => "|",
            BitConstraintOperator.Xor => "^",
            BitConstraintOperator.Nand => "nand",
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };

        private static bool IsValidIdentifier(string value)
        {
            if (value.Length == 0 || value[0] != '_' && !char.IsLetter(value[0])) return false;
            for (var index = 1; index < value.Length; index++)
            {
                if (value[index] != '_' && !char.IsLetterOrDigit(value[index])) return false;
            }

            return true;
        }
    }
}
