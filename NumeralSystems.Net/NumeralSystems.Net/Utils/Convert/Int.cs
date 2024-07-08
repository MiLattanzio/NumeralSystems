using System;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        public static int ToInt(this bool[] s) {
            if (null == s)
                s = Enumerable.Repeat(false, 32).ToArray();
            else
                s = s.Length switch
                {
                    > 32 => s[0..32],
                    < 32 => Enumerable.Repeat(false, 32 - s.Length).Concat(s).ToArray(),
                    _ => s
                };
            int b = 0;
            foreach (var t in s.Reverse())
            {
                b <<= 1;
                if (t) b |= 1;
            }
            return b;
        }
        public static int ToInt(this byte[] s) => BitConverter.ToInt32(s, 0);
    }
}