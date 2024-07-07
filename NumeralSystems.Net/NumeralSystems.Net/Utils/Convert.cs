using System;
using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Utils
{
    public static class Convert
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
        public static byte[] ToByteArray(this int s) => BitConverter.GetBytes(s);
        public static byte[] ToByteArray(this char c) => BitConverter.GetBytes(c);
        public static byte[] ToByteArray(this long s) => BitConverter.GetBytes(s);
        public static byte[] ToByteArray(this double s) => BitConverter.GetBytes(s);
        public static byte[] ToByteArray(this float s) => BitConverter.GetBytes(s);
        public static byte[] ToByteArray(this IEnumerable<int> s) => s.Select(ToByteArray).SelectMany(x => x).ToArray();
        public static byte[] ToByteArray(this decimal s) => decimal.GetBits(s).Select(ToByteArray).SelectMany(x => x).ToArray();
        public static bool[] ToBoolArray(this byte b) {
            var s = new bool[8];
            for (var i = 0; i < s.Length; i++)
            {
                s[i] = (b & (1 << i)) != 0;
            }
            return s;
        }

        public static bool[] ToBoolArray(this char c) => ToByteArray(c).Select(ToBoolArray).SelectMany(x => x).ToArray();
        public static bool[] ToBoolArray(this int b) => ToByteArray(b).Select(ToBoolArray).SelectMany(x => x).ToArray();
        public static bool[] ToBoolArray(this float b) => ToByteArray(b).Select(ToBoolArray).SelectMany(x => x).ToArray();
        public static bool[] ToBoolArray(this double b) => ToByteArray(b).Select(ToBoolArray).SelectMany(x => x).ToArray();
        public static bool[] ToBoolArray(this long b) => ToByteArray(b).Select(ToBoolArray).SelectMany(x => x).ToArray();
        public static bool[] ToBoolArray(this decimal b) => ToByteArray(b).Select(ToBoolArray).SelectMany(x => x).ToArray();
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
        public static float ToFloat(this bool[] s)
        {
            var bytes = ToByteArray(s);
            return BitConverter.ToSingle(bytes);
        }
        public static double ToDouble(this bool[] s){
            var bytes = ToByteArray(s);
            return BitConverter.ToDouble(bytes);
        }
        public static decimal ToDecimal(this bool[] s){
            var bytes = ToByteArray(s);
            return ToDecimal(bytes);
        }
        public static decimal ToDecimal(this byte[] s) => new (ToIntArray(s));
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
        public static int[] ToIntArray(this byte[] s)
        {
            var result = new List<int>();
            for (var i = 0; i < s.Length; i += 4)
                result.Add(BitConverter.ToInt32(s, i));
            return result.ToArray();
        }
        public static IncompleteChar[] ToIncompleteCharArray(this IEnumerable<IncompleteByte> s) => ToIncompleteCharArray(new IncompleteByteArray() {
            Binary = s.Select(x => x.Binary).SelectMany(x => x).ToArray(),
        });
        public static IncompleteChar[] ToIncompleteCharArray(this IncompleteByteArray s)
        {
            var chars = new List<IncompleteChar>();
            for (var i = 0; i < (s.Binary.Length / (8 * sizeof(char))); i++)
            {
                chars.Add(new()
                {
                    Binary = s.Binary.Skip(i * (8 * sizeof(char))).Take((8 * sizeof(char))).ToArray()
                });
            }
            return chars.ToArray();
        }
        
        public static IncompleteInt32[] ToIncompleteInt32Array(this IEnumerable<IncompleteByte> s) => ToIncompleteInt32Array(new IncompleteByteArray() {
            Binary = s.Select(x => x.Binary).SelectMany(x => x).ToArray(),
        });
        public static IncompleteInt32[] ToIncompleteInt32Array(this IncompleteByteArray s)
        {
            var chars = new List<IncompleteInt32>();
            for (var i = 0; i < (s.Binary.Length / (8 * sizeof(int))); i++)
            {
                chars.Add(new()
                {
                    Binary = s.Binary.Skip(i * (8 * sizeof(int))).Take((8 * sizeof(int))).ToArray()
                });
            }
            return chars.ToArray();
        }
        
        public static IncompleteInt64[] ToIncompleteInt64Array(this IEnumerable<IncompleteByte> s) => ToIncompleteInt64Array(new IncompleteByteArray() {
            Binary = s.Select(x => x.Binary).SelectMany(x => x).ToArray(),
        });
        public static IncompleteInt64[] ToIncompleteInt64Array(this IncompleteByteArray s)
        {
            var chars = new List<IncompleteInt64>();
            for (var i = 0; i < (s.Binary.Length / (8 * sizeof(long))); i++)
            {
                chars.Add(new()
                {
                    Binary = s.Binary.Skip(i * (8 * sizeof(long))).Take((8 * sizeof(long))).ToArray()
                });
            }
            return chars.ToArray();
        }
        
        public static string ToStringOfBase(int destinationBase, byte[] input)
        {
            var numeralSystem = Numeral.System.OfBase(destinationBase);
            var numerals = ToIntArray(input).Select(x => new Numeral(numeralSystem)
            {
                Integer = x
            });
            var joined = string.Join(numeralSystem.Separator, numerals);
            return joined;
        }
        
        public static byte[] FromStringOfBase(int sourceBase, string input)
        {
            var numeralSystem = Numeral.System.OfBase(sourceBase);
            var elements = input.SplitAndKeep(numeralSystem.Identity.ToArray());
            var numerals = elements.Select(x => numeralSystem.Parse(x)).ToArray();
            var bytes = numerals.Select(x => x.Bytes).SelectMany(x=>x).ToArray();
            return bytes;
        }
    }
}