namespace NumeralSystems.Net.Interface
{
    /// <summary>
    /// Interface for reverse operations on a incomplete value.
    /// </summary>
    /// <typeparam name="TIncomplete">Incomplete type representation of the type value</typeparam>
    /// <typeparam name="TNumaralValue">Complete representation of the value</typeparam>
    /// <typeparam name="TValue">Native type representation of the value</typeparam>
    /// <typeparam name="TIndexer"> Type used to index the incomplete permutation of the incomplete value type</typeparam>
    public interface IRregularReversible<TIncomplete, in TNumeralValue, TValue, TIndexer> where TNumeralValue : INumeralValue<TValue> where TIncomplete: IIncompleteValue<TNumeralValue, TValue, TIndexer> where TIndexer : struct
    {
        /// <summary>
        /// Reverse operation for the bitwise and operation.
        /// </summary>
        /// <param name="right">Second parameter of the operation</param>
        /// <param name="result">Result of the operation if any</param>
        /// <returns>True if the operation was successful otherwise false</returns>
        bool ReverseAnd(TNumeralValue right, out TIncomplete result);
        /// <summary>
        /// Reverse operation for the bitwise and operation.
        /// </summary>
        /// <param name="right">Second parameter of the operation</param>
        /// <param name="result">Result of the operation if any</param>
        /// <returns>True if the operation was successful otherwise false</returns>
        bool ReverseAnd(TIncomplete right, out TIncomplete result);
        /// <summary>
        /// Reverse operation for the bitwise or operation.
        /// </summary>
        /// <param name="right">Second parameter of the operation</param>
        /// <param name="result">Result of the operation if any</param>
        /// <returns>True if the operation was successful otherwise false</returns>
        bool ReverseOr(TNumeralValue right, out TIncomplete result);
        /// <summary>
        /// Reverse operation for the bitwise or operation.
        /// </summary>
        /// <param name="right">Second parameter of the operation</param>
        /// <param name="result">Result of the operation if any</param>
        /// <returns>True if the operation was successful otherwise false</returns>
        bool ReverseOr(TIncomplete right, out TIncomplete result);
    }
}