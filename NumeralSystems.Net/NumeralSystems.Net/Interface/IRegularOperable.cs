namespace NumeralSystems.Net.Interface
{
    /// <summary>
    /// Interface for regular operations on a numeral value.
    /// </summary>
    /// <typeparam name="TIncomplete">Incomplete type representation of the type value</typeparam>
    /// <typeparam name="TValue">Complete representation of the value</typeparam>
    /// <typeparam name="TType">Native type representation of the value</typeparam>
    /// <typeparam name="TIndexer">Type used to index the incomplete permutation of the incomplete value type</typeparam>
    public interface IRegularOperable<TIncomplete, TValue, TType, TIndexer> : IIncompletable<TIncomplete, TValue, TType, TIndexer>, INumeralValue<TType>, IRegularReversible<TIncomplete, TValue, TType, TIndexer> where TValue: INumeralValue<TType> where TIncomplete : IIncompleteValue<TValue, TType, TIndexer> where TIndexer : struct
    {
        /// <summary>
        /// Bitwise not operation.
        /// </summary>
        /// <returns>The bitwise not result</returns>
        public TValue Not();
        /// <summary>
        /// Bitwise xor operation.
        /// </summary>
        /// <param name="value">Second parameter of the operation</param>
        /// <returns>Bitwise xor on the second parameter</returns>
        public TValue Xor(TValue value);
        /// <summary>
        /// Bitwise xor operation.
        /// </summary>
        /// <param name="value">Second parameter of the operation</param>
        /// <returns>Bitwise xor on the second parameter</returns>
        public TIncomplete Xor(TIncomplete value);
        /// <summary>
        /// Bitwise and operation.
        /// </summary>
        /// <param name="value">Second parameter of the operation</param>
        /// <returns>Bitwise and on the second parameter</returns>
        public TValue And(TValue value);
        /// <summary>
        /// Bitwise and operation.
        /// </summary>
        /// <param name="value">Second parameter of the operation</param>
        /// <returns>Bitwise and on the second parameter</returns>
        public TIncomplete And(TIncomplete value);
        /// <summary>
        /// Bitwise or operation.
        /// </summary>
        /// <param name="value">Second parameter of the operation</param>
        /// <returns>Bitwise or on the second parameter</returns>
        public TValue Or(TValue value);
        /// <summary>
        /// Bitwise or operation.
        /// </summary>
        /// <param name="value">Second parameter of the operation</param>
        /// <returns>Bitwise or on the second parameter</returns>
        public TIncomplete Or(TIncomplete value);
        /// <summary>
        /// Bitwise nand operation.
        /// </summary>
        /// <param name="value">Second parameter of the operation</param>
        /// <returns>Bitwise nand on the second parameter</returns>
        public TValue Nand(TValue value);
        /// <summary>
        /// Bitwise nand operation.
        /// </summary>
        /// <param name="value">Second parameter of the operation</param>
        /// <returns>Bitwise nand on the second parameter</returns>
        public TIncomplete Nand(TIncomplete value);
    }
    
}