using System;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        public static ulong ToULong(this bool[] s) {
            if (null == s)
                s = Enumerable.Repeat(false, sizeof(ulong)).ToArray();
            else
                s = s.Length switch
                {
                    > sizeof(ulong) => s[0..sizeof(ulong)],
                    < sizeof(ulong) => Enumerable.Repeat(false, sizeof(ulong) - s.Length).Concat(s).ToArray(),
                    _ => s
                };
            ulong b = 0;
            foreach (var t in s.Reverse())
            {
                b <<= 1;
                if (t) b |= 1;
            }
            return b;
        }
        public static ulong ToULong(this byte[] s) => BitConverter.ToUInt64(s, 0);
    }
}