using System;
using System.Collections.Generic;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public class Sequence
    {
        public static List<T> SequenceOfIdentityAtIndex<T>(List<T> identity, int index)
        { 
            IEnumerable<T> result = new List<T>();
            while (index != 0)
            {
                index = Math.DivRem(index, identity.Count(), out var remainder);
                result = result.Prepend(identity[remainder]);
            }
            return result.ToList();
        }

        public static IEnumerable<List<T>> IdentityEnumerableOfSize<T>(List<T> identity, int size)
        {
            var idx = 0;
            while (idx < size)
            {
                if (idx < identity.Count())
                {
                    yield return new List<T>() { identity[idx] };
                }
                else
                {
                    yield return SequenceOfIdentityAtIndex(identity, idx);
                }
                idx++;
            }
            
            
        }
    }
}