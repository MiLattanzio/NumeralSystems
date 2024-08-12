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
                    > sizeof(ulong) * 8 => s[0..(sizeof(ulong)*8)],
                    < sizeof(ulong) * 8 => Enumerable.Repeat(false, (sizeof(ulong)*8) - s.Length).Concat(s).ToArray(),
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
        
        public static ulong SetBoolAtIndex(this ulong b, uint index, bool value)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            bytes[byteIndex] = bytes[byteIndex].SetBoolAtIndex(bitIndex, value);
            return bytes.Select(x => x.ToBoolArray()).SelectMany(x =>x).ToArray().ToULong();
        }
    }
}