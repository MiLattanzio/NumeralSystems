namespace NumeralSystems.Net.Interface
{
    public interface IRegularReversible<TIncomplete, in TNumaralValue, TValue> where TNumaralValue : INumeralValue<TValue> where TIncomplete : IIncompleteValue<TNumaralValue, TValue>
    {
        public bool ReverseAnd(TNumaralValue right, out TIncomplete result);
        public bool ReverseAnd(TIncomplete right, out TIncomplete result);
        public bool ReverseOr(TNumaralValue right, out TIncomplete result);
        public bool ReverseOr(TIncomplete right, out TIncomplete result);
    }
}