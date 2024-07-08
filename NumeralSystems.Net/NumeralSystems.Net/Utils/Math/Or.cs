using System;
using System.Diagnostics.CodeAnalysis;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static partial class Math
    {
        public static bool? Or(this bool? left, bool? right) => (left, right) switch
        {
            (null, null) => null,
            (false, null) => null,
            (true, null) => true,
            (null, false) => null,
            (null, true) => true,
            (false, false) => false,
            (false, true) => true,
            (true, false) => true,
            (true, true) => true
        };
        
        public static bool? Or(this bool left, bool? right) => (left, right) switch
        {
            (false, null) => null,
            (true, null) => true,
            (false, false) => false,
            (false, true) => true,
            (true, false) => true,
            (true, true) => true
        };

        public static bool[] Or(this bool[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool[left.Length];
            for (var i = 0; i < left.Length; i++)
                 result[i] = left[i].Or(right[i]);
            return result;
        }
        
        public static bool?[] Or(this bool?[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Or(right[i]);
            return result;
        }
        
        public static bool?[] Or(this bool?[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Or(right[i]);
            return result;
        }
        
        public static bool?[] Or(this bool[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Or(right[i]);
            return result;
        }
        
        public static bool Or(this bool left, bool right) => left | right;
    }
}