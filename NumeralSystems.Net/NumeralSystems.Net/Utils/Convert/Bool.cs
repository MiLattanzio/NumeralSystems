using System.Numerics;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        public static bool GetBoolAtIndex(this byte b, uint index)
        {
            return (b & (1 << (int)index)) != 0;
        }
        
        public static bool GetBoolAtIndex(this short b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }
        
        public static bool GetBoolAtIndex(this ushort b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }
        
        public static bool GetBoolAtIndex(this int b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }
        
        public static bool GetBoolAtIndex(this uint b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }
        
        
        public static bool GetBoolAtIndex(this float b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }
        
        public static bool GetBoolAtIndex(this long b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }
        
        public static bool GetBoolAtIndex(this double b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }
        public static bool GetBoolAtIndex(this decimal b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }
        public static bool GetBoolAtIndex(this char b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[byteIndex].GetBoolAtIndex(bitIndex);
        }
        
        public static bool GetBoolAtIndex(this BigInteger b, uint index)
        {
            var bytes = b.ToByteArray();
            var byteIndex = index / 8;
            var bitIndex = index % 8;
            return bytes[(int)byteIndex].GetBoolAtIndex(bitIndex);
        }
        
        
    }
}