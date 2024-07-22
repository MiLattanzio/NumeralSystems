namespace NumeralSystems.Net.Interface
{
    public interface IIRregularOperable<TIncomplete, TValue, TType> : IIncompleteValue<TValue, TType>,
        IRregularReversible<TIncomplete, TValue, TType>
        where TValue : IRegularOperable<TIncomplete, TValue, TType>
        where TIncomplete : IIncompleteValue<TValue, TType>
    {
        TIncomplete Not();
        TIncomplete Xor(TIncomplete other);
        TIncomplete Xor(TValue other);
        TIncomplete And(TIncomplete other);
        TIncomplete And(TValue other);
        TIncomplete Or(TIncomplete other);
        TIncomplete Or(TValue other);
        public bool Contains(TValue value);
        public bool Contains(TIncomplete value);
    }
}