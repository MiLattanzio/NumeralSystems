using System;
using System.Linq;

namespace NumeralSystens.Math.Net.Utils
{
    public static class Sequence
    {
        public static int PermutationsCount(int identity, int size, bool repetition = false)
        => repetition ? System.Convert.ToInt32(System.Math.Pow(identity, size)) : (Factorial(identity) / Factorial(identity -size));
        
        public static int CombinationsCount(int identity, int size, bool repetition = false)
        => repetition ? ((Factorial(identity + size - 1))/(Factorial(size)*(identity-1))) : (Factorial(identity) / ((Factorial(identity))/(Factorial(size)*(identity-1))));
        
        private static int Factorial(int n)
        {
            if (n == 0) return 1;
            return n * Factorial(n - 1);
        }
        
        public static T[][] Group<T>(this T[] sequence, int count)
        {
            if (sequence.Length % count != 0) throw new InvalidOperationException();
            var array = new T[sequence.Length / count][];
            for (var i = 0; i < sequence.Length / count; i++)
            {
                array[i] = sequence.Skip(i * count).Take(count).ToArray();
            }
            return array;
        }
        
    }
}