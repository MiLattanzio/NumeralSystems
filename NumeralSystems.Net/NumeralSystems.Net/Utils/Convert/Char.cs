using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        public static char ToChar(this bool[] s) {
            if (null == s)
                s = Enumerable.Repeat(false, 16).ToArray();
            else
                s = s.Length switch
                {
                    > 16 => s[0..16],
                    < 16 => Enumerable.Repeat(false, 16 - s.Length).Concat(s).ToArray(),
                    _ => s
                };
            int b = 0;
            foreach (var t in s.Reverse())
            {
                b <<= 1;
                if (t) b |= 1;
            }
            return System.Convert.ToChar(b);
        }
    }
}