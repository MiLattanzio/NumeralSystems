using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        public static long ToLong(this bool[] s) {
            if (null == s)
                s = Enumerable.Repeat(false, 64).ToArray();
            else
                s = s.Length switch
                {
                    > 64 => s[0..64],
                    < 64 => Enumerable.Repeat(false, 64 - s.Length).Concat(s).ToArray(),
                    _ => s
                };
            long b = 0;
            foreach (var t in s.Reverse())
            {
                b <<= 1;
                if (t) b |= 1;
            }
            return b;
        }
    }
}