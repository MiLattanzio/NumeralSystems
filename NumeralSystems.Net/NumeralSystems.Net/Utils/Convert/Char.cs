using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        public static char ToChar(this bool[] s) {
            if (null == s)
                s = Enumerable.Repeat(false, sizeof(char)).ToArray();
            else
                s = s.Length switch
                {
                    > sizeof(char) * 8 => s[0..(sizeof(char)*8)],
                    < sizeof(char) * 8 => Enumerable.Repeat(false, (sizeof(char)*8) - s.Length).Concat(s).ToArray(),
                    _ => s
                };
            char b = (char)0;
            foreach (var t in s.Reverse())
            {
                b <<= 1;
                if (t) b |= (char)1;
            }
            return b;
        }
        
        public static char SetBoolAtIndex(this char b, uint index, bool value)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            bytes[byteIndex] = bytes[byteIndex].SetBoolAtIndex(bitIndex, value);
            return bytes.Select(x => x.ToBoolArray()).SelectMany(x =>x).ToArray().ToChar();
        }
    }
}