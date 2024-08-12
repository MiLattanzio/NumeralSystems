using System;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        public static double ToDouble(this bool[] s){
            var bytes = ToByteArray(s);
            return BitConverter.ToDouble(bytes);
        }
        public static double SetBoolAtIndex(this double b, uint index, bool value)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            bytes[byteIndex] = bytes[byteIndex].SetBoolAtIndex(bitIndex, value);
            return BitConverter.ToDouble(bytes);
        }
    }
}