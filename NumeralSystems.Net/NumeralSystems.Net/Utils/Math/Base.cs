namespace NumeralSystems.Net.Utils
{
    /// <summary>
    /// Provides mathematical functions and operations.
    /// </summary>
    internal static partial class Math
    {
        /// <summary>
        /// Calculates the number of digits needed to represent a given number in a specified base.
        /// </summary>
        /// <param name="number">The number to calculate the number of digits for.</param>
        /// <param name="numeralBase">The base to represent the number in.</param>
        /// <returns>The number of digits needed to represent the given number in the specified base.</returns>
        public static int DigitsInBase(int number, int numeralBase) => (int) System.Math.Ceiling(System.Math.Log(number + 1, numeralBase));

        /// <summary>
        /// Calculates the number of digits needed to represent a given number in a specified base.
        /// </summary>
        /// <param name="number">The number to calculate the number of digits for.</param>
        /// <param name="numeralBase">The base to represent the number in.</param>
        /// <returns>The number of digits needed to represent the given number in the specified base.</returns>
        public static long DigitsInBase(long number, int numeralBase) => (long) System.Math.Ceiling(System.Math.Log(number + 1, numeralBase));

        /// <summary>
        /// Calculates the number of digits required to represent a number in the specified numeral system base.
        /// </summary>
        /// <param name="number">The number to calculate the digits for.</param>
        /// <param name="numeralBase">The numeral system base.</param>
        /// <returns>The number of digits required to represent the number in the specified numeral system base.</returns>
        public static ulong DigitsInBase(ulong number, int numeralBase) => (ulong) System.Math.Ceiling(System.Math.Log(number + 1, numeralBase));
    }
}