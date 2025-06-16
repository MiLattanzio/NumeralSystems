using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static partial class Math
    {
        public static bool Nor(this bool left, bool right) => !(left | right);
        public static bool? Nor(this bool left, bool? right) => left.Or(right).Not();
        public static bool? Nor(this bool? left, bool right) => left.Or(right).Not();
        public static bool? Nor(this bool? left, bool? right) => left.Or(right).Not();
        public static bool[] Nor(this bool[] left, bool[] right) => left.Or(right).Not();
        public static bool?[] Nor(this bool[] left, bool?[] right) => left.Or(right).Not();
        public static bool?[] Nor(this bool?[] left, bool[] right) => left.Or(right).Not();
        public static bool?[] Nor(this bool?[] left, bool?[] right) => left.Or(right).Not();
    }
}
