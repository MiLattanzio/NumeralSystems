using System;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        public static int ToInt(this bool[] s) {
            if (null == s)
                s = Enumerable.Repeat(false, sizeof(int)).ToArray();
            else
                s = s.Length switch
                {
                    > sizeof(int) * 8 => s[0..(sizeof(int)*8)],
                    < sizeof(int) * 8 => Enumerable.Repeat(false, (sizeof(int)*8) - s.Length).Concat(s).ToArray(),
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
        
        public static int SetBoolAtIndex(this int b, uint index, bool value)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            bytes[byteIndex] = bytes[byteIndex].SetBoolAtIndex(bitIndex, value);
            return bytes.Select(x => x.ToBoolArray()).SelectMany(x =>x).ToArray().ToInt();
        }
    }
}