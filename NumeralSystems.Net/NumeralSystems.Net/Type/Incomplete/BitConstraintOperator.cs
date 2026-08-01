namespace NumeralSystems.Net.Type.Incomplete
{
    /// <summary>
    /// Identifies the bitwise operation used by a <see cref="BitConstraint"/>.
    /// </summary>
    public enum BitConstraintOperator
    {
        /// <summary>Bitwise AND (<c>&amp;</c>).</summary>
        And,

        /// <summary>Bitwise OR (<c>|</c>).</summary>
        Or,

        /// <summary>Bitwise exclusive OR (<c>^</c>).</summary>
        Xor,

        /// <summary>Bitwise NAND (<c>nand</c>).</summary>
        Nand
    }
}
