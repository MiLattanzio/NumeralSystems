namespace NumeralSystems.Net.Interface
{
    public interface IRegularOperable<TIncomplete, TValue, TType> : IIncompletable<TIncomplete, TValue, TType>, INumeralValue<TType>, IRegularReversible<TIncomplete, TValue, TType> where TValue: INumeralValue<TType> where TIncomplete : IIncompleteValue<TValue, TType>
    {
        public TValue And(TValue value);
        public TIncomplete And(TIncomplete value);
        public TValue Or(TValue value);
        public TIncomplete Or(TIncomplete value);
    }
    
}