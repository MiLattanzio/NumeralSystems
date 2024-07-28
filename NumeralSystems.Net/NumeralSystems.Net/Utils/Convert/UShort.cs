using System;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        public static ushort ToUShort(this bool[] s)
        {
            const int bitsInUint = sizeof(short) * 8;
            if (s == null)
            {
                s = new bool[bitsInUint];
            }
            else if (s.Length > bitsInUint)
            {
                s = s.TakeLast(bitsInUint).ToArray();
            }
            else if (s.Length < bitsInUint)
            {
                s = Enumerable.Repeat(false, bitsInUint - s.Length).Concat(s).ToArray();
            }

            return s.Reverse().Aggregate<bool, ushort>(0, (current, bit) => (ushort)((current << 1) | (bit ? 1 : 0)));
        }
        
        public static ushort ToUShort(this byte[] s) => BitConverter.ToUInt16(s, 0);
    }
}