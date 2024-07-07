﻿using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;
using Convert = System.Convert;
using Math = System.Math;

namespace NumeralSystems.Net.Type.Incomplete
{
    public class IncompleteByteArray
    {
        private bool?[] _binary;
        public bool?[] Binary
        {
            get => _binary ?? Enumerable.Repeat(false, 8).Select(x => x as bool?).ToArray();
            set
            {
                if (null == _binary)
                {
                    _binary = Enumerable.Repeat(false, 8).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= 8)
                    {
                        _binary = value.Take(Convert.ToInt32(Math.Abs(value.Length / 8.0d))*8).ToArray();
                    }else {
                        _binary = Enumerable.Repeat(false, 8 - value.Length).Select(x => x as bool?).Concat(value).ToArray();
                    }
                }
            }
        }
        public int Permutations => Sequence.PermutationsCount(2, Binary.Count(x => x is null));
        public int Size => Binary.Length / 8;
        public NumeralByte[] this[int value]{
            get
            {
                var resultBinary = value.ToBoolArray();
                var result = new List<NumeralByte>();
                for (var i = 0; i < (resultBinary.Length / 8); i++)
                {
                    result.Add(Byte.FromBinary(resultBinary.Skip(i * 8).Take(8).ToArray()));
                }
                return result.ToArray();
            }
        }
        public IncompleteByte[] Array
        {
            get
            {
                var result = new List<IncompleteByte>();
                for (var i = 0; i < (Binary.Length / 8); i++)
                {
                    result.Add(new IncompleteByte()
                    {
                        Binary = Binary.Skip(i*8).Take(8).ToArray()
                    });
                }
                return result.ToArray();
            }
            
            set
            {
                Binary = null == value ? new IncompleteByte().Binary : value.Select(x => x.Binary).SelectMany(x => x).ToArray();
            }
        }
        
    }
}