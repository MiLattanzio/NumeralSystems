namespace NumeralSystems.Net.Interface
{
    public interface IRegularOperable<TIncomplete, TValue, TType, TIndexer> : IIncompletable<TIncomplete, TValue, TType, TIndexer>, INumeralValue<TType>, IRegularReversible<TIncomplete, TValue, TType, TIndexer> where TValue: INumeralValue<TType> where TIncomplete : IIncompleteValue<TValue, TType, TIndexer> where TIndexer : struct
    {
        public TValue Not();
        public TValue Xor(TValue value);
        public TIncomplete Xor(TIncomplete value);
        public TValue And(TValue value);
        public TIncomplete And(TIncomplete value);
        public TValue Or(TValue value);
        public TIncomplete Or(TIncomplete value);
        public TValue Nand(TValue value);
        public TIncomplete Nand(TIncomplete value);
    }
    
}