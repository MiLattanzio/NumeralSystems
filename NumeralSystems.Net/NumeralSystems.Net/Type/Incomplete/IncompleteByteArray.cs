using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;
using Polecola.Primitive;
using Convert = System.Convert;
using Math = System.Math;

namespace NumeralSystems.Net.Type.Incomplete
{
    public class IncompleteByteArray
    {
        private bool?[] _binary;

        /// <summary>
        /// Gets or sets the binary representation of the byte array.
        /// </summary>
        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8).Select(x => x as bool?).ToArray();
            internal set
            {
                if (null == _binary)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= 8)
                    {
                        _binary = value.Take(Convert.ToInt32(Math.Abs(value.Length / 8.0d))*8).ToArray();
                    }else {
                        _binary = System.Linq.Enumerable.Repeat(false, 8 - value.Length).Select(x => x as bool?).Concat(value).ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the number of permutations of the binary representation.
        /// </summary>
        public int Permutations => Sequence.PermutationsCount(2, Binary.Count(x => x is null));

        /// <summary>
        /// Gets the size of the binary representation in bytes.
        /// </summary>
        public int Size => Binary.Length / 8;

        /// <summary>
        /// Gets the byte representation for the specified value.
        /// </summary>
        /// <param name="value">The value to get the byte representation for.</param>
        /// <returns>An array of bytes representing the value.</returns>
        public Byte[] this[int value]{
            get
            {
                var resultBinary = value.ToBoolArray();
                var result = new List<Byte>();
                for (var i = 0; i < (resultBinary.Length / 8); i++)
                {
                    result.Add(new Byte()
                    {
                        Binary = resultBinary.Skip(i * 8).Take(8).ToArray()
                    });
                }
                return result.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the array of incomplete byte representations.
        /// </summary>
        public IncompleteByte[] Array
        {
            get => ArrayOf(Binary);
            set
            {
                Binary = null == value ? new IncompleteByte().Binary : value.Select(x => x.Binary).SelectMany(x => x).ToArray();
            }
        }

        /// <summary>
        /// Converts the binary representation to an array of incomplete byte representations.
        /// </summary>
        /// <param name="binary">The binary representation to convert.</param>
        /// <returns>The array of incomplete byte representations.</returns>
        public static IncompleteByte[] ArrayOf(bool?[] binary)
        {
            var result = new List<IncompleteByte>();
            for (var i = 0; i < (binary.Length / 8); i++)
            {
                result.Add(new IncompleteByte()
                {
                    Binary = binary.Skip(i*8).Take(8).ToArray()
                });
            }
            return result.ToArray();
        }

    }
}