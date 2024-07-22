using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
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
    }
}