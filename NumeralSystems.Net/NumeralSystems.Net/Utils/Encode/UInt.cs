using System;
using System.Collections.Generic;
using System.Linq;

namespace NumeralSystems.Net.Utils.Encode
{
    public static class UInt
    {
        public static uint[] ToIndicesOfBase(int val, int destinationBase, out bool positive)
        {
            positive = val >= 0;
            return ULong.ToIndicesOfBase((ulong)System.Math.Abs(val), destinationBase).Select(x => (uint)x).ToArray();
        }
        
        public static int FromIndicesOfBase(uint[] val, int sourceBase, bool positive)
        {
            return (int)ULong.FromIndicesOfBase(val.Select(x => (ulong)x).ToArray(), sourceBase) * (positive ? 1 : -1);
        }
    }
}