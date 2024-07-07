namespace NumeralSystems.Net.Interface
{
    public interface IGet<TValue>
    {
        TValue this[int index] { get; }
    }
}