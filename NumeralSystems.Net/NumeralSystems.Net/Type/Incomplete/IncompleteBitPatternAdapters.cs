namespace NumeralSystems.Net.Type.Incomplete
{
    public partial class IncompleteByte
    {
        protected override bool?[] PatternBits => Binary;

        protected override IncompleteByte FromPattern(BitPattern pattern) => new IncompleteByte
        {
            Binary = pattern.ToArray()
        };
    }

    public partial class IncompleteByteArray
    {
        protected override bool?[] PatternBits => Binary;

        protected override IncompleteByteArray FromPattern(BitPattern pattern) => new IncompleteByteArray
        {
            Binary = pattern.ToArray()
        };
    }

    public partial class IncompleteChar
    {
        protected override bool?[] PatternBits => Binary;

        protected override IncompleteChar FromPattern(BitPattern pattern) => new IncompleteChar
        {
            Binary = pattern.ToArray()
        };
    }

    public partial class IncompleteDecimal
    {
        protected override bool?[] PatternBits => Binary;

        protected override IncompleteDecimal FromPattern(BitPattern pattern) => new IncompleteDecimal
        {
            Binary = pattern.ToArray()
        };
    }

    public partial class IncompleteDouble
    {
        protected override bool?[] PatternBits => Binary;

        protected override IncompleteDouble FromPattern(BitPattern pattern) => new IncompleteDouble
        {
            Binary = pattern.ToArray()
        };
    }

    public partial class IncompleteFloat
    {
        protected override bool?[] PatternBits => Binary;

        protected override IncompleteFloat FromPattern(BitPattern pattern) => new IncompleteFloat
        {
            Binary = pattern.ToArray()
        };
    }

    public partial class IncompleteInt
    {
        protected override bool?[] PatternBits => Binary;

        protected override IncompleteInt FromPattern(BitPattern pattern) => new IncompleteInt
        {
            Binary = pattern.ToArray()
        };
    }

    public partial class IncompleteLong
    {
        protected override bool?[] PatternBits => Binary;

        protected override IncompleteLong FromPattern(BitPattern pattern) => new IncompleteLong
        {
            Binary = pattern.ToArray()
        };
    }

    public partial class IncompleteShort
    {
        protected override bool?[] PatternBits => Binary;

        protected override IncompleteShort FromPattern(BitPattern pattern) => new IncompleteShort
        {
            Binary = pattern.ToArray()
        };
    }

    public partial class IncompleteUInt
    {
        protected override bool?[] PatternBits => Binary;

        protected override IncompleteUInt FromPattern(BitPattern pattern) => new IncompleteUInt
        {
            Binary = pattern.ToArray()
        };
    }

    public partial class IncompleteULong
    {
        protected override bool?[] PatternBits => Binary;

        protected override IncompleteULong FromPattern(BitPattern pattern) => new IncompleteULong
        {
            Binary = pattern.ToArray()
        };
    }

    public partial class IncompleteUShort
    {
        protected override bool?[] PatternBits => Binary;

        protected override IncompleteUShort FromPattern(BitPattern pattern) => new IncompleteUShort
        {
            Binary = pattern.ToArray()
        };
    }
}
