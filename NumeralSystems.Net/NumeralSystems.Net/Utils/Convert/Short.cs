using System;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        public static short ToShort(this bool[] s)
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

            return s.Reverse().Aggregate<bool, short>(0, (current, bit) => (short)((current << 1) | (bit ? 1 : 0)));
        }
        
        public static short ToShort(this byte[] s) => BitConverter.ToInt16(s, 0);
    }
}