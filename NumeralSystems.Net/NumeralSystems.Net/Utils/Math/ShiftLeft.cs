using System.Linq;

namespace NumeralSystems.Net.Utils
{
    internal static partial class Math
    {
        public static bool[] ShiftLeft(this bool[] value, int count)
        {
            if (count <= 0) return value.ToArray();
            if (count >= value.Length) return new bool[value.Length];
            var result = new bool[value.Length];
            for (var i = count; i < value.Length; i++)
                result[i] = value[i - count];
            return result;
        }

        public static bool?[] ShiftLeft(this bool?[] value, int count)
        {
            if (count <= 0) return value.ToArray();
            if (count >= value.Length) return new bool?[value.Length];
            var result = new bool?[value.Length];
            for (var i = count; i < value.Length; i++)
                result[i] = value[i - count];
            return result;
        }
    }
}
