using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal static partial class Math
    {
        public static bool? ReverseAnd(this bool? left, bool? right) => (left, right) switch
        {
            (null, null) => null,
            (false, null) => false,
            (true, null) => throw new InvalidOperationException(),
            (null, false) => throw new InvalidOperationException(),
            (null, true) => null,
            (false, false) => null,
            (false, true) => false,
            (true, false) => throw new InvalidOperationException(),
            (true, true) => true
        };
        
        public static bool?[] ReverseAnd(this bool[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("left and right must be the same length");

            var result = new bool?[left.Length];
            
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ReverseAnd(left[i], right[i]);
            }

            return result;
        }
        
        public static bool?[] ReverseAnd(this bool[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("left and right must be the same length");

            var result = new bool?[left.Length];
            
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ReverseAnd(left[i], right[i]);
            }

            return result;
        }
        
        public static bool?[] ReverseAnd(this bool?[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("left and right must be the same length");

            var result = new bool?[left.Length];
            
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ReverseAnd(left[i], right[i]);
            }

            return result;
        }
        
        public static bool?[] ReverseAnd(this bool?[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("left and right must be the same length");

            var result = new bool?[left.Length];
            
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ReverseAnd(left[i], right[i]);
            }

            return result;
        }
        
        public static bool CanReverseAnd(this bool? left, bool? right) => (left, right) switch 
        {
            (true, null) => false,
            (null, false) => false,
            (true, false) => false,
            _ => true
        };
        
        public static bool CanReverseAnd(this bool?[] left, bool?[] right)
        {
            if (left.Length != right.Length) return false;
            return !left.Where((t, i) => !CanReverseAnd(t, right[i])).Any();
        }
        
        public static bool CanReverseAnd(this bool[] left, bool?[] right)
        {
            if (left.Length != right.Length) return false;
            return !left.Where((t, i) => !CanReverseAnd(t, right[i])).Any();
        }
        
        public static bool CanReverseAnd(this bool[] left, bool[] right)
        {
            if (left.Length != right.Length) return false;
            return !left.Where((t, i) => !CanReverseAnd(t, right[i])).Any();
        }
        
        public static bool CanReverseAnd(this bool?[] left, bool[] right)
        {
            if (left.Length != right.Length) return false;
            return !left.Where((t, i) => !CanReverseAnd(t, right[i])).Any();
        }
    }
}