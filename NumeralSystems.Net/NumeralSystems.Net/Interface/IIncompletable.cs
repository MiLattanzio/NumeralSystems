namespace NumeralSystems.Net.Interface
{
    public interface IIncompletable<out TIncomplete, TValue, TType> where TIncomplete : IIncompleteValue<TValue, TType> where TValue : INumeralValue<TType>
    {
        public TIncomplete Incomplete();
    }
}