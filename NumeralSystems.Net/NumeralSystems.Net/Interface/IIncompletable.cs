namespace NumeralSystems.Net.Interface
{
    /// <summary>
    /// Represents a type that can be converted to an incomplete value.
    /// </summary>
    /// <typeparam name="TIncomplete">The incomplete type that it can be converted to</typeparam>
    /// <typeparam name="TValue">The source value type</typeparam>
    /// <typeparam name="TType">The internal .net base type that the source value represents</typeparam>
    /// <typeparam name="TIndexer">The index type used to get the complete representation</typeparam>
    public interface IIncompletable<out TIncomplete, TValue, TType, TIndexer> where TIncomplete : IIncompleteValue<TValue, TType, TIndexer> where TValue : INumeralValue<TType> where TIndexer : struct
    {
        /// <summary>
        /// Gets the incomplete representation of the value.
        /// </summary>
        /// <returns>The source value as an incomplete value</returns>
        public TIncomplete Incomplete();
    }
}