using System;

namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        public static double ToDouble(this bool[] s){
            var bytes = ToByteArray(s);
            return BitConverter.ToDouble(bytes);
        }
    }
}