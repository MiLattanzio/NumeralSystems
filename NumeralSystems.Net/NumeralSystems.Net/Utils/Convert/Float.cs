using System;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        public static float ToFloat(this bool[] s)
        {
            var bytes = ToByteArray(s);
            return BitConverter.ToSingle(bytes);
        }

        public static float SetBoolAtIndex(this float b, uint index, bool value)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            bytes[byteIndex] = bytes[byteIndex].SetBoolAtIndex(bitIndex, value);
            return BitConverter.ToSingle(bytes);
        }
    }
}