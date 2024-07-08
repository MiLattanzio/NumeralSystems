namespace NumeralSystems.Net.Utils
{
    public static partial class Convert
    {
        public static decimal ToDecimal(this bool[] s){
            var bytes = ToByteArray(s);
            return ToDecimal(bytes);
        }
        public static decimal ToDecimal(this byte[] s) => new (ToIntArray(s));
    }
}