using System;
using System.Diagnostics.CodeAnalysis;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static partial class Math
    {
        public static bool? Xor(this bool left, bool? right) => (left, right) switch
        {
            (false, null) => null,
            (true, null) => null,
            (false, false) => false,
            (false, true) => true,
            (true, false) => true,
            (true, true) => false
        };
        public static bool? Xor(this bool? left, bool? right) => (left, right) switch
        {
            (null, null) => null,
            (false, null) => null,
            (true, null) => null,
            (null, false) => null,
            (null, true) => null,
            (false, false) => false,
            (false, true) => true,
            (true, false) => true,
            (true, true) => false
        };
        
        public static bool? Xor(this bool? left, bool right) => (left, right) switch
        {
            (null, false) => null,
            (null, true) => null,
            (false, false) => false,
            (false, true) => true,
            (true, false) => true,
            (true, true) => false
        };
        
        public static bool Xor(this bool left, bool right) => left ^ right;
        public static bool?[] Xor(this bool[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Xor(right[i]);
            return result;
        }
        public static bool[] Xor(this bool[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Xor(right[i]);
            return result;
        }
        public static bool?[] Xor(this bool?[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Xor(right[i]);
            return result;
        }

        public static bool?[] Xor(this bool?[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Xor(right[i]);
            return result;
        }
    }
}