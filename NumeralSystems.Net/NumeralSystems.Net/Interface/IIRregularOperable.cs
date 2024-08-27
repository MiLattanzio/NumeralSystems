namespace NumeralSystems.Net.Interface
{
    /// <summary>
    /// Interface for regular operations on incomplete values.
    /// </summary>
    /// <typeparam name="TIncomplete">Incomplete source type</typeparam>
    /// <typeparam name="TValue">Complete source type</typeparam>
    /// <typeparam name="TType">Complete native source type representation</typeparam>
    /// <typeparam name="TIndexer">Indexer for the incomplete type</typeparam>
    public interface IIRregularOperable<TIncomplete, TValue, TType, TIndexer> : IIncompleteValue<TValue, TType, TIndexer>,
        IRregularReversible<TIncomplete, TValue, TType, TIndexer>
        where TValue : IRegularOperable<TIncomplete, TValue, TType, TIndexer>
        where TIncomplete : IIncompleteValue<TValue, TType, TIndexer>
        where TIndexer : struct
    {
        /// <summary>
        /// Bitwise not operation.
        /// </summary>
        /// <returns>Bitwise not result</returns>
        TIncomplete Not();
        /// <summary>
        /// Bitwise xor operation.
        /// </summary>
        /// <param name="other">Second parameter of the operation</param>
        /// <returns>Bitwise xor on the second parameter</returns>
        TIncomplete Xor(TIncomplete other);
        /// <summary>
        /// Bitwise xor operation.
        /// </summary>
        /// <param name="other">Second parameter of the operation</param>
        /// <returns>Bitwise xor on the second parameter</returns>
        TIncomplete Xor(TValue other);
        /// <summary>
        /// Bitwise and operation.
        /// </summary>
        /// <param name="other">Second parameter of the operation</param>
        /// <returns>Bitwise and on the second parameter</returns>
        TIncomplete And(TIncomplete other);
        /// <summary>
        /// Bitwise and operation.
        /// </summary>
        /// <param name="other">Second parameter of the operation</param>
        /// <returns>Bitwise and on the second parameter</returns>
        TIncomplete And(TValue other);
        /// <summary>
        /// Bitwise or operation.
        /// </summary>
        /// <param name="other">Second parameter of the operation</param>
        /// <returns>Bitwise or on the second parameter</returns>
        TIncomplete Or(TIncomplete other);
        /// <summary>
        /// Bitwise or operation.
        /// </summary>
        /// <param name="other">Second parameter of the operation</param>
        /// <returns>Bitwise or on the second parameter</returns>
        TIncomplete Or(TValue other);
        /// <summary>
        /// Checks if the incomplete value contains the given value.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the incomplete includes the value otherwhise false</returns>
        public bool Contains(TValue value);
        /// <summary>
        /// Checks if the incomplete value contains the given value.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>True if the incomplete includes the value otherwhise false</returns>
        public bool Contains(TIncomplete value);
    }
}