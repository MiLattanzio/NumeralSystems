#nullable enable
using System;

namespace NumeralSystems.Net
{
    /// <summary>Base exception for failures while materializing positional fractional digits.</summary>
    public class NumeralExpansionException : ArithmeticException
    {
        /// <summary>Creates an expansion exception.</summary>
        public NumeralExpansionException(string message) : base(message)
        {
        }
    }

    /// <summary>Thrown when an infinite positional expansion is forbidden by the selected policy.</summary>
    public sealed class InfiniteNumeralExpansionException : NumeralExpansionException
    {
        /// <summary>Creates an infinite-expansion exception.</summary>
        public InfiniteNumeralExpansionException(string message) : base(message)
        {
        }
    }

    /// <summary>Thrown when an exact period cannot be found within the configured digit limit.</summary>
    public sealed class NumeralExpansionLimitException : NumeralExpansionException
    {
        /// <summary>Gets the configured digit limit.</summary>
        public int MaxFractionalDigits { get; }

        /// <summary>Creates an expansion-limit exception.</summary>
        public NumeralExpansionLimitException(string message, int maxFractionalDigits) : base(message)
        {
            MaxFractionalDigits = maxFractionalDigits;
        }
    }
}
