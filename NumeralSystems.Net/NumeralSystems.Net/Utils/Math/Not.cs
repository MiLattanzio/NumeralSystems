using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static partial class Math
    {
        public static bool? Not(this bool? value) => value switch
        {
            null => null,
            false => true,
            true => false
        };
        public static bool Not(this bool value) => !value;
        public static bool[] Not(this bool[] value) => value.Select(Not).ToArray();
        public static bool?[] Not(this bool?[] value) => value.Select(Not).ToArray();
    }
}