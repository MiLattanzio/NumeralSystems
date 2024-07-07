using System;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static class Math
    {
        public static bool[] And(this bool[] a, bool[] b)
        {
            if (a.Length != b.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool[a.Length];
            for (var i = 0; i < a.Length; i++)
                result[i] = a[i] && b[i];
            return result;
        }
        
        public static bool?[] And(this bool?[] a, bool?[] b)
        {
            if (a.Length != b.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[a.Length];
            for (var i = 0; i < a.Length; i++)
                if (null == a[i] || null == b[i])
                    result[i] = null;
                else
                    result[i] = a[i].Value && b[i].Value;
            return result;
        }
        
        public static bool?[] And(this bool[] a, bool?[] b)
        {
            if (a.Length != b.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[a.Length];
            for (var i = 0; i < a.Length; i++)
                if (null == b[i])
                    result[i] = null;
                else
                    result[i] = a[i] && b[i].Value;
            return result;
        }
        
        public static bool?[] And(this bool?[] a, bool[] b)
        {
            if (a.Length != b.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[a.Length];
            for (var i = 0; i < a.Length; i++)
                if (null == a[i])
                    result[i] = null;
                else
                    result[i] = a[i].Value && b[i];
            return result;
        }

        public static decimal And(this decimal a, decimal b) =>
            And(a.ToBoolArray(), b.ToBoolArray()).ToDecimal();
        public static double And(this double a, double b) =>
            And(a.ToBoolArray(), b.ToBoolArray()).ToDouble();
        public static float And(this float a, float b) =>
            And(a.ToBoolArray(), b.ToBoolArray()).ToFloat();

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

        public static bool[] Or(this bool[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool[left.Length];
            for (var i = 0; i < left.Length; i++)
                 result[i] = Or(left[i], right[i]);
            return result;
        }
        
        public static bool?[] Or(this bool?[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = Or(left[i], right[i]);
            return result;
        }
        
        public static bool?[] Or(this bool?[] left, bool[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = Or(left[i], right[i]);
            return result;
        }
        
        public static bool?[] Or(this bool[] left, bool?[] right)
        {
            if (left.Length != right.Length) throw new ArgumentException("Arrays must be of equal length");
            var result = new bool?[left.Length];
            for (var i = 0; i < left.Length; i++)
                result[i] = Or(left[i], right[i]);
            return result;
        }
        
        public static bool Or(this bool left, bool right) => Or((bool?)left, (bool?)right) ?? throw new InvalidOperationException();
        
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

        public static bool CanReverseAnd(this bool? left, bool? right) => (left, right) switch 
        {
            (true, null) => false,
            (null, false) => false,
            (true, false) => false,
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
        
        public static bool CanReverseOr(this bool? left, bool? right) => (left, right) switch 
        {
            (false, null) => false,
            (null, true) => false,
            (false, true) => false,
            _ => true
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
    }
}