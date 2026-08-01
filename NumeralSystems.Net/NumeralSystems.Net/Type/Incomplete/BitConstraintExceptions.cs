#nullable enable
using System;

namespace NumeralSystems.Net.Type.Incomplete
{
    /// <summary>Thrown when an explicit constraint-engine resource limit is exceeded.</summary>
    public sealed class BitConstraintLimitException : InvalidOperationException
    {
        /// <summary>Creates a limit error.</summary>
        public BitConstraintLimitException(string limitName, string message)
            : base(message)
        {
            LimitName = limitName ?? throw new ArgumentNullException(nameof(limitName));
        }

        /// <summary>Gets the name of the exceeded limit.</summary>
        public string LimitName { get; }
    }

    /// <summary>Thrown when constraint solving or candidate enumeration times out.</summary>
    public sealed class BitConstraintTimeoutException : TimeoutException
    {
        /// <summary>Creates a timeout error.</summary>
        public BitConstraintTimeoutException(TimeSpan timeout)
            : base($"The bit-constraint operation exceeded its timeout of {timeout.TotalMilliseconds:0.###} ms.")
        {
            Timeout = timeout;
        }

        /// <summary>Gets the configured timeout.</summary>
        public TimeSpan Timeout { get; }
    }
}
