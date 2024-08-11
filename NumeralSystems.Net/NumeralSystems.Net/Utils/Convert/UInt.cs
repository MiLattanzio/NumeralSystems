using System;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        public static uint ToUInt(this bool[] s)
        {
            const int bitsInUint = sizeof(uint) * 8;
            if (s == null)
            {
                s = new bool[bitsInUint];
            }
            else if (s.Length > bitsInUint)
            {
                s = s.TakeLast(bitsInUint).ToArray();
            }
            else if (s.Length < bitsInUint)
            {
                s = Enumerable.Repeat(false, bitsInUint - s.Length).Concat(s).ToArray();
            }

            uint result = 0;
            foreach (var bit in s.Reverse())
            {
                result = (result << 1) | (bit ? 1u : 0u);
            }

            return result;
        }
        public static uint ToUInt(this byte[] s) => BitConverter.ToUInt32(s, 0);
        
        public static uint SetBoolAtIndex(this uint b, uint index, bool value)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            bytes[byteIndex] = bytes[byteIndex].SetBoolAtIndex(bitIndex, value);
            return bytes.Select(x => x.ToBoolArray()).SelectMany(x => x).ToArray().ToUInt();
        }
    }
}