using System;
using System.Collections.Generic;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static class Sequence
    {
        public static IEnumerable<T> SequenceOfIdentityAtIndex<T>(List<T> identity, int index)
        { 
            IEnumerable<T> result = new List<T>();
            while (index != 0)
            {
                index = System.Math.DivRem(index, identity.Count(), out var remainder);
                result = result.Prepend(identity[remainder]);
            }
            return result;
        }

        public static IEnumerable<IEnumerable<T>> IdentityEnumerableOfSize<T>(List<T> identity, int size)
        {
            var idx = 0;
            while (idx < size)
            {
                if (idx < identity.Count())
                {
                    yield return new List<T> { identity[idx] };
                }
                else
                {
                    yield return SequenceOfIdentityAtIndex(identity, idx);
                }
                idx++;
            }
            
            
        }
        
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