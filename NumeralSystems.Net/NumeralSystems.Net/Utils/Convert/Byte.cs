using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        public static byte ToByte(this bool[] s) {
            if (null == s)
                s = Enumerable.Repeat(false, 8).ToArray();
            else
                s = s.Length switch
                {
                    > 8 => s[0..8],
                    < 8 => Enumerable.Repeat(false, 8 - s.Length).Concat(s).ToArray(),
                    _ => s
                };
            byte b = 0;
            foreach (var t in s.Reverse())
            {
                b <<= 1;
                if (t) b |= 1;
            }
            return b;
        }
    }
}