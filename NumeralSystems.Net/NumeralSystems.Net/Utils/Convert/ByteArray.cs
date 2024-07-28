using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static partial class Convert
    {
        public static byte[] ToByteArray(this bool[] s) {
            if (null == s)
                s = Enumerable.Repeat(false, 8).ToArray();
            else
            {
                System.Math.DivRem(s.Length, 8, out var rem);
                if (rem > 0)
                    s = s.Concat(Enumerable.Repeat(false, 8 - rem)).ToArray();
            }
            var b = new byte[s.Length / 8];
            for (var i = 0; i < b.Length; i++)
                b[i] = ToByte(s.Skip(i * 8).Take(8).ToArray());
            return b;
        }
        public static byte[] ToByteArray(this uint s) => BitConverter.GetBytes(s);
        public static byte[] ToByteArray(this int s) => BitConverter.GetBytes(s);
        public static byte[] ToByteArray(this IEnumerable<int> s) => s.Select(ToByteArray).SelectMany(x => x).ToArray();

        public static byte[] ToByteArray(this char c) => BitConverter.GetBytes(c);
        public static byte[] ToByteArray(this short s) => BitConverter.GetBytes(s);
        public static byte[] ToByteArray(this ushort s) => BitConverter.GetBytes(s);
        public static byte[] ToByteArray(this long s) => BitConverter.GetBytes(s);
        public static byte[] ToByteArray(this double s) => BitConverter.GetBytes(s);
        public static byte[] ToByteArray(this float s) => BitConverter.GetBytes(s);
        public static byte[] ToByteArray(this decimal s) => decimal.GetBits(s).Select(ToByteArray).SelectMany(x => x).ToArray();
        public static byte[] ToByteArray(this ulong s) => BitConverter.GetBytes(s);
    }
}