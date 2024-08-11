namespace NumeralSystems.Net.Interface
{
    public interface IRregularReversible<TIncomplete, TValue, TType, TIndexer> where TValue : INumeralValue<TType> where TIncomplete: IIncompleteValue<TValue, TType, TIndexer> where TIndexer : struct
    {
        bool ReverseAnd(TValue right, out TIncomplete result);
        bool ReverseAnd(TIncomplete right, out TIncomplete result);
        bool ReverseOr(TValue right, out TIncomplete result);
        bool ReverseOr(TIncomplete right, out TIncomplete result);
    }
}