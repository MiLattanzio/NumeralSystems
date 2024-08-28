using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace NumeralSystems.Net.Type.Base
{
    public sealed partial class UInt
    {
        /// <summary>
        /// Converts an integer value to its indices representation in a specified base.
        /// </summary>
        /// <param name="val">The integer value to convert.</param>
        /// <param name="destinationBase">The base to convert to.</param>
        /// <param name="positive">Indicates if the value is positive.</param>
        /// <returns>An array of uint representing the indices in the specified base.</returns>
        public static uint[] ToIndicesOfBase(int val, int destinationBase, out bool positive)
        {
            positive = val >= 0;
            return ULong.ToIndicesOfBase((ulong)System.Math.Abs(val), destinationBase).Select(x => (uint)x).ToArray();
        }

        /// <summary>
        /// Converts indices representation in a specified base to an integer value.
        /// </summary>
        /// <param name="val">The array of uint representing the indices.</param>
        /// <param name="sourceBase">The base of the indices.</param>
        /// <param name="positive">Indicates if the value is positive.</param>
        /// <returns>The integer value.</returns>
        public static int FromIndicesOfBase(uint[] val, int sourceBase, bool positive)
        {
            return (int)ULong.FromIndicesOfBase(val.Select(x => (ulong)x).ToArray(), sourceBase) * (positive ? 1 : -1);
        }
    }
}