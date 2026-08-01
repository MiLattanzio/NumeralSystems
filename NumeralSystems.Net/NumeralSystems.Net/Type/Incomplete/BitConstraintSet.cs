#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace NumeralSystems.Net.Type.Incomplete
{
    /// <summary>
    /// Represents an immutable, composable collection of constraints for one variable.
    /// </summary>
    public sealed class BitConstraintSet : IReadOnlyList<BitConstraint>
    {
        private readonly BitConstraint[] _constraints;

        /// <summary>Creates a constraint set for one variable and one fixed width.</summary>
        public BitConstraintSet(IEnumerable<BitConstraint> constraints)
        {
            if (constraints is null) throw new ArgumentNullException(nameof(constraints));
            _constraints = constraints.ToArray();
            if (_constraints.Length == 0)
                throw new ArgumentException("At least one bit constraint is required.", nameof(constraints));
            if (_constraints.Any(constraint => constraint is null))
                throw new ArgumentException("A constraint set cannot contain null values.", nameof(constraints));

            VariableName = _constraints[0].VariableName;
            Width = _constraints[0].Width;
            for (var index = 1; index < _constraints.Length; index++)
            {
                var constraint = _constraints[index];
                if (!string.Equals(VariableName, constraint.VariableName, StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException(
                        "Every constraint in a set must target the same variable.",
                        nameof(constraints));
                if (Width != constraint.Width)
                    throw new ArgumentException(
                        "Every constraint in a set must use the same bit width.",
                        nameof(constraints));
            }
        }

        /// <summary>Gets the shared variable name.</summary>
        public string VariableName { get; }

        /// <summary>Gets the shared fixed bit width.</summary>
        public int Width { get; }

        /// <inheritdoc />
        public int Count => _constraints.Length;

        /// <inheritdoc />
        public BitConstraint this[int index] => _constraints[index];

        /// <summary>Parses constraints separated by semicolons or line breaks.</summary>
        public static BitConstraintSet Parse(string expressions)
        {
            if (expressions is null) throw new ArgumentNullException(nameof(expressions));
            var segments = expressions
                .Split(new[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(segment => !string.IsNullOrWhiteSpace(segment))
                .ToArray();
            if (segments.Length == 0)
                throw new FormatException("At least one bit constraint is required.");

            var constraints = new BitConstraint[segments.Length];
            for (var index = 0; index < segments.Length; index++)
            {
                try
                {
                    constraints[index] = BitConstraint.Parse(segments[index]);
                }
                catch (FormatException exception)
                {
                    throw new FormatException($"Constraint {index + 1}: {exception.Message}", exception);
                }
            }

            return new BitConstraintSet(constraints);
        }

        /// <summary>Attempts to parse constraints separated by semicolons or line breaks.</summary>
        public static bool TryParse(string? expressions, out BitConstraintSet? constraints)
        {
            try
            {
                constraints = expressions is null ? null : Parse(expressions);
                return constraints is not null;
            }
            catch (Exception exception) when (exception is ArgumentException or FormatException)
            {
                constraints = null;
                return false;
            }
        }

        /// <summary>Returns a new set with one additional compatible constraint.</summary>
        public BitConstraintSet Add(BitConstraint constraint)
        {
            if (constraint is null) throw new ArgumentNullException(nameof(constraint));
            return new BitConstraintSet(_constraints.Concat(new[] { constraint }));
        }

        /// <summary>
        /// Solves all constraints per bit without enumerating concrete candidates.
        /// </summary>
        public BitConstraintSolution Solve(
            BitConstraintSolverOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            options ??= BitConstraintSolverOptions.Default;
            if (Count > options.MaximumConstraints)
                throw new BitConstraintLimitException(
                    nameof(BitConstraintSolverOptions.MaximumConstraints),
                    $"The constraint count {Count} exceeds the configured maximum of {options.MaximumConstraints}.");
            if (Width > options.MaximumBitWidth)
                throw new BitConstraintLimitException(
                    nameof(BitConstraintSolverOptions.MaximumBitWidth),
                    $"The bit width {Width} exceeds the configured maximum of {options.MaximumBitWidth}.");

            var stopwatch = Stopwatch.StartNew();
            var bits = new bool?[Width];
            var explanations = new List<BitConstraintBitExplanation>(Width);
            var satisfiable = true;

            for (var bitIndex = 0; bitIndex < Width; bitIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                BitConstraintSolution.ThrowIfTimedOut(stopwatch, options.Timeout);

                var canBeZero = true;
                var canBeOne = true;
                var requiresZero = new List<BitConstraint>();
                var requiresOne = new List<BitConstraint>();
                var impossible = new List<BitConstraint>();

                foreach (var constraint in _constraints)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    BitConstraintSolution.ThrowIfTimedOut(stopwatch, options.Timeout);
                    constraint.GetAllowedValues(bitIndex, out var constraintAllowsZero, out var constraintAllowsOne);

                    canBeZero &= constraintAllowsZero;
                    canBeOne &= constraintAllowsOne;
                    if (!constraintAllowsZero && !constraintAllowsOne) impossible.Add(constraint);
                    else if (constraintAllowsZero && !constraintAllowsOne) requiresZero.Add(constraint);
                    else if (!constraintAllowsZero && constraintAllowsOne) requiresOne.Add(constraint);
                }

                var sources = impossible.Concat(requiresZero).Concat(requiresOne).Distinct().ToArray();
                var message = Explain(canBeZero, canBeOne, impossible, requiresZero, requiresOne);
                explanations.Add(new BitConstraintBitExplanation(
                    bitIndex,
                    canBeZero,
                    canBeOne,
                    message,
                    sources));

                if (!canBeZero && !canBeOne)
                {
                    satisfiable = false;
                    bits[bitIndex] = null;
                }
                else
                {
                    bits[bitIndex] = canBeZero && canBeOne ? (bool?)null : canBeOne;
                }
            }

            return new BitConstraintSolution(
                satisfiable ? new BitPattern(bits) : null,
                explanations,
                options);
        }

        /// <inheritdoc />
        public IEnumerator<BitConstraint> GetEnumerator() =>
            ((IEnumerable<BitConstraint>)_constraints).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static string Explain(
            bool canBeZero,
            bool canBeOne,
            IReadOnlyCollection<BitConstraint> impossible,
            IReadOnlyCollection<BitConstraint> requiresZero,
            IReadOnlyCollection<BitConstraint> requiresOne)
        {
            if (canBeZero && canBeOne)
                return "Both 0 and 1 satisfy every constraint.";
            if (canBeZero)
                return $"The bit must be 0 because {JoinConstraints(requiresZero)}.";
            if (canBeOne)
                return $"The bit must be 1 because {JoinConstraints(requiresOne)}.";
            if (impossible.Count > 0)
                return $"No bit value satisfies {JoinConstraints(impossible)}.";

            return "No bit value is possible: " +
                   $"{JoinConstraints(requiresZero)} requires 0, while " +
                   $"{JoinConstraints(requiresOne)} requires 1.";
        }

        private static string JoinConstraints(IEnumerable<BitConstraint> constraints) =>
            string.Join(", ", constraints.Select(constraint => $"'{constraint}'"));
    }
}
