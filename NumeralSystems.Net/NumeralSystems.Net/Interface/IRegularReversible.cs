namespace NumeralSystems.Net.Interface
{
    /// <summary>
    /// Interface for reverse operations on a numeral value.
    /// </summary>
    /// <typeparam name="TIncomplete">Incomplete type representation of the type value</typeparam>
    /// <typeparam name="TNumaralValue">Complete representation of the value</typeparam>
    /// <typeparam name="TValue">Native type representation of the value</typeparam>
    /// <typeparam name="TIndexer"> Type used to index the incomplete permutation of the incomplete value type</typeparam>
    public interface IRegularReversible<TIncomplete, in TNumaralValue, TValue, TIndexer> where TNumaralValue : INumeralValue<TValue> where TIncomplete : IIncompleteValue<TNumaralValue, TValue, TIndexer> where TIndexer : struct
    {
        /// <summary>
        /// Reverse operation for the bitwise and operation.
        /// </summary>
        /// <param name="right">Second parameter of the operation</param>
        /// <param name="result">Result of the operation if any</param>
        /// <returns>True if the operation was successful otherwise false</returns>
        public bool ReverseAnd(TNumaralValue right, out TIncomplete result);
        /// <summary>
        /// Reverse operation for the bitwise and operation.
        /// </summary>
        /// <param name="right">Second parameter of the operation</param>
        /// <param name="result">Result of the operation if any</param>
        /// <returns>True if the operation was successful otherwise false</returns>
        public bool ReverseAnd(TIncomplete right, out TIncomplete result);
        /// <summary>
        /// Reverse operation for the bitwise or operation.
        /// </summary>
        /// <param name="right">Second parameter of the operation</param>
        /// <param name="result">Result of the operation if any</param>
        /// <returns>True if the operation was successful otherwise false</returns>
        public bool ReverseOr(TNumaralValue right, out TIncomplete result);
        /// <summary>
        /// Reverse operation for the bitwise or operation.
        /// </summary>
        /// <param name="right">Second parameter of the operation</param>
        /// <param name="result">Result of the operation if any</param>
        /// <returns>True if the operation was successful otherwise false</returns>
        public bool ReverseOr(TIncomplete right, out TIncomplete result);
    }
}