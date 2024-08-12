using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NumeralSystems.Net.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static partial class Convert
    {
        public static int[] ToIntArray(this byte[] s)
        {
            var result = new List<int>();
            for (var i = 0; i < s.Length; i += 4)
                result.Add(BitConverter.ToInt32(s, i));
            return result.ToArray();
        }
        
        
    }
}