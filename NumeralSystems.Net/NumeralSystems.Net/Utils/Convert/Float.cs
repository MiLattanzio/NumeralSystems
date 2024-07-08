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
    }
}