using System;

namespace NumeralSystems.Net
{
    /// <summary>
    /// Describes why numeral parsing failed.
    /// </summary>
    public enum ParseErrorReason
    {
        /// <summary>No error occurred.</summary>
        None,
        /// <summary>The input reference was null.</summary>
        NullInput,
        /// <summary>The input was empty.</summary>
        EmptyInput,
        /// <summary>The alphabet size differs from the numeral-system base.</summary>
        AlphabetSizeMismatch,
        /// <summary>The alphabet conflicts with a separator or sign.</summary>
        InvalidConfiguration,
        /// <summary>No alphabet symbol starts at the reported position.</summary>
        UnknownSymbol,
        /// <summary>A digit was required at the reported position.</summary>
        MissingDigit,
        /// <summary>A digit separator appeared where no separator was allowed.</summary>
        UnexpectedSeparator,
        /// <summary>A configured digit separator was missing.</summary>
        MissingSeparator,
        /// <summary>The negative sign appeared outside the first position.</summary>
        MisplacedNegativeSign,
        /// <summary>More than one decimal separator was present.</summary>
        RepeatedDecimalSeparator
    }

    /// <summary>
    /// Contains either a parsed <see cref="Numeral"/> or a structured parsing
    /// error with its UTF-16 position.
    /// </summary>
    public sealed class ParseResult
    {
        private ParseResult(
            bool success,
            Numeral value,
            int position,
            int errorLength,
            ParseErrorReason reason,
            string message)
        {
            Success = success;
            Value = value;
            Position = position;
            ErrorLength = errorLength;
            Reason = reason;
            Message = message ?? string.Empty;
        }

        /// <summary>
        /// Gets whether parsing succeeded.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets the parsed numeral, or <see langword="null"/> on failure.
        /// </summary>
        public Numeral Value { get; }

        /// <summary>
        /// Gets the zero-based UTF-16 error position, or -1 on success.
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Gets the number of UTF-16 code units associated with the error.
        /// </summary>
        public int ErrorLength { get; }

        /// <summary>
        /// Gets the machine-readable error reason.
        /// </summary>
        public ParseErrorReason Reason { get; }

        /// <summary>
        /// Gets the human-readable error description.
        /// </summary>
        public string Message { get; }

        internal static ParseResult Succeeded(Numeral value) =>
            new ParseResult(true, value ?? throw new ArgumentNullException(nameof(value)),
                -1, 0, ParseErrorReason.None, string.Empty);

        internal static ParseResult Failed(
            ParseErrorReason reason,
            int position,
            string message,
            int errorLength = 1) =>
            new ParseResult(false, null, Math.Max(0, position), Math.Max(0, errorLength), reason, message);
    }
}
