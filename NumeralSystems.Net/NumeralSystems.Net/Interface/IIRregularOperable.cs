namespace NumeralSystems.Net.Interface
{
    public interface IIRregularOperable<TIncomplete, TValue, TType, TIndexer> : IIncompleteValue<TValue, TType, TIndexer>,
        IRregularReversible<TIncomplete, TValue, TType, TIndexer>
        where TValue : IRegularOperable<TIncomplete, TValue, TType, TIndexer>
        where TIncomplete : IIncompleteValue<TValue, TType, TIndexer>
        where TIndexer : struct
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