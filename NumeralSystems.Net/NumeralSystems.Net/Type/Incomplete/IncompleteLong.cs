using System;
using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;
using Polecola.Primitive;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = Polecola.Primitive.Convert;

namespace NumeralSystems.Net.Type.Incomplete
{
    /// <summary>
    /// Represents an incomplete long integer with various bitwise operations.
    /// </summary>
    public class IncompleteLong : IIRregularOperable<IncompleteLong, Long, long, ulong>
    {
        private bool?[] _binary;

        /// <summary>
        /// Gets or sets the binary representation of the long integer.
        /// </summary>
        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8 * sizeof(long)).Select(x => x as bool?).ToArray();
            set
            {
                if (null == value)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8 * sizeof(long)).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= (8 * sizeof(long)))
                    {
                        _binary = value.Take(8 * sizeof(long)).ToArray();
                    }
                    else
                    {
                        _binary = System.Linq.Enumerable.Repeat(false, (8 * sizeof(long)) - value.Length)
                            .Select(x => x as bool?)
                            .Concat(value).ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the binary representation is complete.
        /// </summary>
        public bool IsComplete => Binary.All(x => x != null);

        /// <summary>
        /// Gets the number of permutations of the binary representation.
        /// </summary>
        public ulong Permutations => Sequence.PermutationsCount(2, Sequence.CountToULong(Binary.Where(x => x is null)), true);

        /// <summary>
        /// Gets the long integer representation for the specified value.
        /// </summary>
        /// <param name="value">The value to get the long integer representation for.</param>
        /// <returns>The long integer representation.</returns>
        public Long this[ulong value]
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
                        resultBinary[i] = binary[i] ?? false;
                    }
                }
                return new Long()
                {
                    Binary = resultBinary
                };
            }
        }

        /// <summary>
        /// Gets the enumerable of long integer representations.
        /// </summary>
        public IEnumerable<Long> Enumerable => Sequence.Range(0, Permutations).Select(x => this[x]);

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
        /// Performs a bitwise OR operation with the specified long integer.
        /// </summary>
        /// <param name="other">The long integer to perform the OR operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteLong Or(Long other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Determines whether the binary representation contains the specified long integer.
        /// </summary>
        /// <param name="value">The long integer to check for.</param>
        /// <returns><c>true</c> if the binary representation contains the long integer; otherwise, <c>false</c>.</returns>
        public bool Contains(Long value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        /// <summary>
        /// Determines whether the binary representation contains the specified incomplete long integer.
        /// </summary>
        /// <param name="value">The incomplete long integer to check for.</param>
        /// <returns><c>true</c> if the binary representation contains the incomplete long integer; otherwise, <c>false</c>.</returns>
        public bool Contains(IncompleteLong value)
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
        public IncompleteLong Not() => new()
        {
            Binary = Binary.Not()
        };

        /// <summary>
        /// Performs a bitwise XOR operation with the specified incomplete long integer.
        /// </summary>
        /// <param name="other">The incomplete long integer to perform the XOR operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteLong Xor(IncompleteLong other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise XOR operation with the specified long integer.
        /// </summary>
        /// <param name="other">The long integer to perform the XOR operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteLong Xor(Long other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise AND operation with the specified incomplete long integer.
        /// </summary>
        /// <param name="other">The incomplete long integer to perform the AND operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteLong And(IncompleteLong other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise AND operation with the specified long integer.
        /// </summary>
        /// <param name="other">The long integer to perform the AND operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteLong And(Long other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise OR operation with the specified incomplete long integer.
        /// </summary>
        /// <param name="other">The incomplete long integer to perform the OR operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteLong Or(IncompleteLong other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Performs a reverse bitwise AND operation with the specified long integer.
        /// </summary>
        /// <param name="right">The long integer to perform the reverse AND operation with.</param>
        /// <param name="result">The result of the reverse AND operation.</param>
        /// <returns><c>true</c> if the reverse AND operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseAnd(Long right, out IncompleteLong result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteLong()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a reverse bitwise AND operation with the specified incomplete long integer.
        /// </summary>
        /// <param name="right">The incomplete long integer to perform the reverse AND operation with.</param>
        /// <param name="result">The result of the reverse AND operation.</param>
        /// <returns><c>true</c> if the reverse AND operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseAnd(IncompleteLong right, out IncompleteLong result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteLong()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a reverse bitwise OR operation with the specified long integer.
        /// </summary>
        /// <param name="right">The long integer to perform the reverse OR operation with.</param>
        /// <param name="result">The result of the reverse OR operation.</param>
        /// <returns><c>true</c> if the reverse OR operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseOr(Long right, out IncompleteLong result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteLong()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a reverse bitwise OR operation with the specified incomplete long integer.
        /// </summary>
        /// <param name="right">The incomplete long integer to perform the reverse OR operation with.</param>
        /// <param name="result">The result of the reverse OR operation.</param>
        /// <returns><c>true</c> if the reverse OR operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseOr(IncompleteLong right, out IncompleteLong result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteLong()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }
    }
}