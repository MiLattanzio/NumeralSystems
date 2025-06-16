using System.Linq;

namespace NumeralSystems.Net.Utils
{
    internal static partial class Math
    {
        public static bool[] ShiftRight(this bool[] value, int count)
        {
            if (count <= 0) return value.ToArray();
            if (count >= value.Length) return new bool[value.Length];
            var result = new bool[value.Length];
            for (var i = 0; i < value.Length - count; i++)
                result[i] = value[i + count];
            return result;
        }

        public static bool?[] ShiftRight(this bool?[] value, int count)
        {
            if (count <= 0) return value.ToArray();
            if (count >= value.Length) return new bool?[value.Length];
            var result = new bool?[value.Length];
            for (var i = 0; i < value.Length - count; i++)
                result[i] = value[i + count];
            return result;
        }
    }
}
