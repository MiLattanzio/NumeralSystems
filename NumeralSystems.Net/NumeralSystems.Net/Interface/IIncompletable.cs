namespace NumeralSystems.Net.Interface
{
    public interface IIncompletable<out TIncomplete, TValue, TType, TIndexer> where TIncomplete : IIncompleteValue<TValue, TType, TIndexer> where TValue : INumeralValue<TType> where TIndexer : struct
    {
        public TIncomplete Incomplete();
    }
}