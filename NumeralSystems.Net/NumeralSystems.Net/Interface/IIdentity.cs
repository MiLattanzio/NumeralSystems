namespace NumeralSystems.Net.Interface
{
    public interface IIdentity<TValue>: ICount, IGet<TValue>
    {
        bool Contains(TValue value);
        TValue Separator { get; }
    }
}