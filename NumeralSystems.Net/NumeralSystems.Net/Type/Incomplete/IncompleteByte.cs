using System;
using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;
using Polecola.Primitive;
using Byte = NumeralSystems.Net.Type.Base.Byte;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = Polecola.Primitive.Convert;

namespace NumeralSystems.Net.Type.Incomplete
{
    public class IncompleteByte : IIRregularOperable<IncompleteByte, Byte, byte, uint>
    {
        private bool?[] _binary;

        /// <summary>
        /// Gets or sets the binary representation of the byte.
        /// </summary>
        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8).Select(x => x as bool?).ToArray();
            internal set
            {
                if (null == value)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= 8)
                    {
                        _binary = value.Take(8).ToArray();
                    }
                    else
                    {
                        _binary = System.Linq.Enumerable.Repeat(false, 8 - value.Length).Select(x => x as bool?)
                            .Concat(value).ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the number of permutations of the binary representation.
        /// </summary>
        public uint Permutations => Sequence.PermutationsCount(2, Sequence.CountToUInt(Binary.Where(x => x is null)), true);

        /// <summary>
        /// Gets a value indicating whether the binary representation is complete.
        /// </summary>
        public bool IsComplete => Binary.All(x => x != null);

        /// <summary>
        /// Gets the byte representation for the specified value.
        /// </summary>
        /// <param name="value">The value to get the byte representation for.</param>
        /// <returns>The byte representation.</returns>
        public Byte this[uint value]
        {
            get
            {
                var binary = Binary;
                var valueBinary = value.ToBoolArray();
                var resultBinary = new bool[binary.Length];
                var lastValueBinaryIndex = 0;
                for (var i = 0; i < binary.Length; i++)
                {
                    for (var i1 = lastValueBinaryIndex; i1 < valueBinary.Length; i1++)
                    {
                        if (binary[i] is null)
                        {
                            resultBinary[i] = valueBinary[i1];
                            lastValueBinaryIndex = i1 + 1;
                            break;
                        }
                        resultBinary[i] = binary[i].Value;
                        break;
                    }
                }
                return new Byte()
                {
                    Binary = resultBinary
                };
            }
        }

        /// <summary>
        /// Gets the enumerable of byte representations.
        /// </summary>
        private IEnumerable<Byte> Bytes => Sequence.Range(0, Permutations).Select(x => this[x]);

        /// <summary>
        /// Gets the enumerable of byte representations.
        /// </summary>
        public IEnumerable<Byte> Enumerable => Bytes;

        /// <summary>
        /// Gets the array of incomplete byte representations.
        /// </summary>
        public IncompleteByte[] ByteArray => IncompleteByteArray.ArrayOf(Binary);

        /// <summary>
        /// Converts the binary representation to an array of incomplete byte representations.
        /// </summary>
        /// <returns>The array of incomplete byte representations.</returns>
        public IncompleteByte[] ToByteArray() => IncompleteByteArray.ArrayOf(Binary.Select(x => x).ToArray());

        /// <summary>
        /// Converts the binary representation to a string.
        /// </summary>
        /// <param name="missingSeparator">The separator for missing bits.</param>
        /// <returns>The string representation.</returns>
        public string ToString(string missingSeparator = "*") => string.Join(string.Empty, Binary.Group(8).Select(x => x.Reverse().ToArray()).SelectMany(x => x).Select(x => null == x ? missingSeparator : (x.Value ? 1 : 0).ToString()));

        /// <summary>
        /// Performs a bitwise OR operation with the specified byte.
        /// </summary>
        /// <param name="other">The byte to perform the OR operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteByte Or(Byte other)=> new()
        {
            Binary = Binary.Or(other.Binary)
        };

        /// <summary>
        /// Determines whether the binary representation contains the specified byte.
        /// </summary>
        /// <param name="value">The byte to check for.</param>
        /// <returns><c>true</c> if the binary representation contains the byte; otherwise, <c>false</c>.</returns>
        public bool Contains(Byte value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        /// <summary>
        /// Determines whether the binary representation contains the specified incomplete byte.
        /// </summary>
        /// <param name="value">The incomplete byte to check for.</param>
        /// <returns><c>true</c> if the binary representation contains the incomplete byte; otherwise, <c>false</c>.</returns>
        public bool Contains(IncompleteByte value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            for (var i = 0; i < bytes.Length; i++)
            {
                var bit = bytes[i];
                if (bit is null) continue;
                var bit2 = bytesBinary[i];
                if (bit2 is null) continue;
                if (bytesBinary[i] != bit) return false;
            }
            return true;
        }

        /// <summary>
        /// Performs a bitwise NOT operation on the binary representation.
        /// </summary>
        /// <returns>The result of the NOT operation.</returns>
        public IncompleteByte Not() => new()
        {
            Binary = Binary.Not()
        };

        /// <summary>
        /// Performs a bitwise XOR operation with the specified incomplete byte.
        /// </summary>
        /// <param name="other">The incomplete byte to perform the XOR operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteByte Xor(IncompleteByte other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise XOR operation with the specified byte.
        /// </summary>
        /// <param name="other">The byte to perform the XOR operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteByte Xor(Byte other) => new ()
        {
            Binary = Binary.Xor(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise AND operation with the specified incomplete byte.
        /// </summary>
        /// <param name="other">The incomplete byte to perform the AND operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteByte And(IncompleteByte other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise AND operation with the specified byte.
        /// </summary>
        /// <param name="other">The byte to perform the AND operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteByte And(Byte other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise OR operation with the specified incomplete byte.
        /// </summary>
        /// <param name="other">The incomplete byte to perform the OR operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteByte Or(IncompleteByte other) => new()
        {
            Binary = Binary.Or(other.Binary)
        };

        /// <summary>
        /// Performs a reverse bitwise AND operation with the specified byte.
        /// </summary>
        /// <param name="right">The byte to perform the reverse AND operation with.</param>
        /// <param name="result">The result of the reverse AND operation.</param>
        /// <returns><c>true</c> if the reverse AND operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseAnd(Byte right, out IncompleteByte result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteByte()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a reverse bitwise AND operation with the specified incomplete byte.
        /// </summary>
        /// <param name="right">The incomplete byte to perform the reverse AND operation with.</param>
        /// <param name="result">The result of the reverse AND operation.</param>
        /// <returns><c>true</c> if the reverse AND operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseAnd(IncompleteByte right, out IncompleteByte result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteByte()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a reverse bitwise OR operation with the specified byte.
        /// </summary>
        /// <param name="right">The byte to perform the reverse OR operation with.</param>
        /// <param name="result">The result of the reverse OR operation.</param>
        /// <returns><c>true</c> if the reverse OR operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseOr(Byte right, out IncompleteByte result)
        {
            if (!Binary.CanReverseOr(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteByte()
            {
                Binary = Binary.ReverseOr(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a reverse bitwise OR operation with the specified incomplete byte.
        /// </summary>
        /// <param name="right">The incomplete byte to perform the reverse OR operation with.</param>
        /// <param name="result">The result of the reverse OR operation.</param>
        /// <returns><c>true</c> if the reverse OR operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseOr(IncompleteByte right, out IncompleteByte result)
        {
            if (!Binary.CanReverseOr(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteByte()
            {
                Binary = Binary.ReverseOr(right.Binary)
            };
            return true;
        }
    }
}