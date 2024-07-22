﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace NumeralSystems.Net.Utils.Encode
{
    public static class ULong
    {
        public static ulong[] ToIndicesOfBase(ulong val, int destinationBase)
        {
            if (destinationBase <= 0)
            {
                throw new ArgumentException("Destination base must be greater than 0");
            }
            if (destinationBase == 1)
            {
                if (val == 0) return new ulong[] { 0 }; // Special handling for 0 in base 1
                var array = new ulong[val];
                for (ulong i = 0; i < val; i++)
                {
                    array[i] = 1;
                }
                return array;
            }
            if (val == 0) return new ulong[] { 0 };

            List<ulong> result = new List<ulong>();
            while (val != 0)
            {
                ulong remainder = val % (ulong)destinationBase;
                val /= (ulong)destinationBase;
                result.Insert(0, remainder); // Prepend operation using Insert at index 0
            }
            return result.ToArray();
        }
        
        public static ulong FromIndicesOfBase(ulong[] val, int sourceBase)
        {
            switch (sourceBase)
            {
                case <= 0:
                    throw new ArgumentException("Source base must be greater than 0");
                case 1:
                    return (ulong)val.Length;
            }
            ulong result = 0;
            for (var i = 0; i < val.Length; i++)
            {
                result += val[i] * (ulong) System.Math.Pow(sourceBase, val.Length - i - 1);
            }
            return result;
        }
    }
}