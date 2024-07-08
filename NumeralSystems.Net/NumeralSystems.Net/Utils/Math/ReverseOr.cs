using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal partial class Math
    {
        public static bool? ReverseOr(this bool? left, bool? right) => (left, right) switch
        {
            (null, null) => null,
            (false, null) => throw new InvalidOperationException(),
            (true, null) => true,
            (null, false) => null,
            (null, true) => throw new InvalidOperationException(),
            (false, false) => false,
            (false, true) => throw new InvalidOperationException(),
            (true, false) => true,
            (true, true) => null
        };
        
        public static bool?[] ReverseOr(this bool[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("left and right must be the same length");

            var result = new bool?[left.Length];
            
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ReverseOr(left[i], right[i]);
            }

            return result;
        }

        public static bool?[] ReverseOr(this bool[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("left and right must be the same length");

            var result = new bool?[left.Length];
            
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ReverseOr(left[i], right[i]);
            }

            return result;
        }

        public static bool?[] ReverseOr(this bool?[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("left and right must be the same length");

            var result = new bool?[left.Length];
            
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ReverseOr(left[i], right[i]);
            }

            return result;
        }

        public static bool?[] ReverseOr(this bool?[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("left and right must be the same length");

            var result = new bool?[left.Length];
            
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = ReverseOr(left[i], right[i]);
            }

            return result;
        }
        
        public static bool CanReverseOr(this bool? left, bool? right) => (left, right) switch 
        {
            (false, null) => false,
            (null, true) => false,
            (false, true) => false,
            _ => true
        };
        
        public static bool CanReverseOr(this bool?[] left, bool?[] right)
        {
            if (left.Length != right.Length) return false;
            return !left.Where((t, i) => !CanReverseOr(t, right[i])).Any();
        }
        
        public static bool CanReverseOr(this bool[] left, bool?[] right)
        {
            if (left.Length != right.Length) return false;
            return !left.Where((t, i) => !CanReverseOr(t, right[i])).Any();
        }
        
        public static bool CanReverseOr(this bool[] left, bool[] right)
        {
            if (left.Length != right.Length) return false;
            return !left.Where((t, i) => !CanReverseOr(t, right[i])).Any();
        }
        
        public static bool CanReverseOr(this bool?[] left, bool[] right)
        {
            if (left.Length != right.Length) return false;
            return !left.Where((t, i) => !CanReverseOr(t, right[i])).Any();
        }
    }
}