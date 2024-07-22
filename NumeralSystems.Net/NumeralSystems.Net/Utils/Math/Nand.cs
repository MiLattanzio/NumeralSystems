using System;

namespace NumeralSystems.Net.Utils
{
    internal static partial class Math
    {
        public static bool Nand(this bool left, bool right) => (left, right) switch
        {
            (false, false) => true,
            (false, true) => true,
            (true, false) => true,
            (true, true) => false
        };
        
        public static bool? Nand(this bool left, bool? right) => (left, right) switch
        {
            (false, null) => true,
            (true, null) => null,
            (false, false) => true,
            (false, true) => true,
            (true, false) => true,
            (true, true) => false
        };
        public static bool? Nand(this bool? left, bool? right) => (left, right) switch
        {
            (null, null) => null,
            (false, null) => true,
            (true, null) => null,
            (null, false) => true,
            (null, true) => true,
            (false, false) => true,
            (false, true) => true,
            (true, false) => true,
            (true, true) => false
        };
        
        public static bool[] Nand(this bool[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Nand(right[i]);
            return result;
        }
        
        public static bool?[] Nand(this bool[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Nand(right[i]);
            return result;
        }
        
        public static bool?[] Nand(this bool?[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Nand(right[i]);
            return result;
        }
        
        public static bool?[] Nand(this bool?[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = left[i].Nand(right[i]);
            return result;
        }
    }
}