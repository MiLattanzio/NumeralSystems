using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        public static long ToLong(this bool[] s) {
            if (null == s)
                s = Enumerable.Repeat(false, sizeof(long)).ToArray();
            else
                s = s.Length switch
                {
                    > sizeof(long) * 8 => s[0..(sizeof(long)*8)],
                    < sizeof(long) * 8 => Enumerable.Repeat(false, (sizeof(long)*8) - s.Length).Concat(s).ToArray(),
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
        
        public static long SetBoolAtIndex(this long b, uint index, bool value)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            bytes[byteIndex] = bytes[byteIndex].SetBoolAtIndex(bitIndex, value);
            return bytes.Select(x => x.ToBoolArray()).SelectMany(x =>x).ToArray().ToLong();
        }
    }
}