using System;
using System.Diagnostics.CodeAnalysis;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal static partial class Math
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

    }
}