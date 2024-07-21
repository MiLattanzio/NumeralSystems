namespace NumeralSystems.Net.Utils
{
    internal static partial class Math
    {
        public static int DigitsInBase(int number, int numeralBase) => (int) System.Math.Ceiling(System.Math.Log(number + 1, numeralBase));

        public static long DigitsInBase(long number, int numeralBase) => (long) System.Math.Ceiling(System.Math.Log(number + 1, numeralBase));

        public static ulong DigitsInBase(ulong number, int numeralBase) => (ulong) System.Math.Ceiling(System.Math.Log(number + 1, numeralBase));
    }
}