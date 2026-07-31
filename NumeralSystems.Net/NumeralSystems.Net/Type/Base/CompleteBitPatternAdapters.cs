using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Type.Base
{
    public sealed partial class Byte
    {
        protected override bool[] CompletePatternBits => Binary;

        protected override IncompleteByte FromPattern(BitPattern pattern) => new IncompleteByte
        {
            Binary = pattern.ToArray()
        };
    }

    public sealed partial class Char
    {
        protected override bool[] CompletePatternBits => Binary;

        protected override IncompleteChar FromPattern(BitPattern pattern) => new IncompleteChar
        {
            Binary = pattern.ToArray()
        };
    }

    public partial class Decimal
    {
        protected override bool[] CompletePatternBits => Binary;

        protected override IncompleteDecimal FromPattern(BitPattern pattern) => new IncompleteDecimal
        {
            Binary = pattern.ToArray()
        };
    }

    public sealed partial class Double
    {
        protected override bool[] CompletePatternBits => Binary;

        protected override IncompleteDouble FromPattern(BitPattern pattern) => new IncompleteDouble
        {
            Binary = pattern.ToArray()
        };
    }

    public sealed partial class Float
    {
        protected override bool[] CompletePatternBits => Binary;

        protected override IncompleteFloat FromPattern(BitPattern pattern) => new IncompleteFloat
        {
            Binary = pattern.ToArray()
        };
    }

    public sealed partial class Int
    {
        protected override bool[] CompletePatternBits => Binary;

        protected override IncompleteInt FromPattern(BitPattern pattern) => new IncompleteInt
        {
            Binary = pattern.ToArray()
        };
    }

    public sealed partial class Long
    {
        protected override bool[] CompletePatternBits => Binary;

        protected override IncompleteLong FromPattern(BitPattern pattern) => new IncompleteLong
        {
            Binary = pattern.ToArray()
        };
    }

    public sealed partial class Short
    {
        protected override bool[] CompletePatternBits => Binary;

        protected override IncompleteShort FromPattern(BitPattern pattern) => new IncompleteShort
        {
            Binary = pattern.ToArray()
        };
    }

    public sealed partial class UInt
    {
        protected override bool[] CompletePatternBits => Binary;

        protected override IncompleteUInt FromPattern(BitPattern pattern) => new IncompleteUInt
        {
            Binary = pattern.ToArray()
        };
    }

    public sealed partial class ULong
    {
        protected override bool[] CompletePatternBits => Binary;

        protected override IncompleteULong FromPattern(BitPattern pattern) => new IncompleteULong
        {
            Binary = pattern.ToArray()
        };
    }

    public sealed partial class UShort
    {
        protected override bool[] CompletePatternBits => Binary;

        protected override IncompleteUShort FromPattern(BitPattern pattern) => new IncompleteUShort
        {
            Binary = pattern.ToArray()
        };
    }
}
