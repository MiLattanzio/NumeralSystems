using System;
using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;
using Polecola.Primitive;
using Char = NumeralSystems.Net.Type.Base.Char;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = Polecola.Primitive.Convert;

namespace NumeralSystems.Net.Type.Incomplete
{
    public class IncompleteChar : IIRregularOperable<IncompleteChar, Char, char, uint>
    {
        private bool?[] _binary;

        /// <summary>
        /// Gets or sets the binary representation of the character.
        /// </summary>
        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8 * sizeof(char)).Select(x => x as bool?).ToArray();
            internal set
            {
                if (null == value)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8 * sizeof(char)).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= (8 * sizeof(char)))
                    {
                        _binary = value.Take(8 * sizeof(char)).ToArray();
                    }
                    else
                    {
                        _binary = System.Linq.Enumerable.Repeat(false, (8 * sizeof(char)) - value.Length)
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
        public uint Permutations => Sequence.PermutationsCount(2, Sequence.CountToUInt(Binary.Where(x => x is null)), true);

        /// <summary>
        /// Gets the character representation for the specified value.
        /// </summary>
        /// <param name="value">The value to get the character representation for.</param>
        /// <returns>The character representation.</returns>
        public Char this[uint value]
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
                return new Char
                {
                    Binary = resultBinary
                };
            }
        }

        /// <summary>
        /// Gets the enumerable of character representations.
        /// </summary>
        public IEnumerable<Char> Enumerable => Sequence.Range(0, Permutations).Select(x => this[x]);

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
        /// Performs a bitwise OR operation with the specified character.
        /// </summary>
        /// <param name="other">The character to perform the OR operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteChar Or(Char other) => new()
        {
            Binary = Binary.Or(other.Binary)
        };

        /// <summary>
        /// Determines whether the binary representation contains the specified character.
        /// </summary>
        /// <param name="value">The character to check for.</param>
        /// <returns><c>true</c> if the binary representation contains the character; otherwise, <c>false</c>.</returns>
        public bool Contains(Char value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        /// <summary>
        /// Determines whether the binary representation contains the specified incomplete character.
        /// </summary>
        /// <param name="value">The incomplete character to check for.</param>
        /// <returns><c>true</c> if the binary representation contains the incomplete character; otherwise, <c>false</c>.</returns>
        public bool Contains(IncompleteChar value)
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
        public IncompleteChar Not() => new()
        {
            Binary = Binary.Not()
        };

        /// <summary>
        /// Performs a bitwise XOR operation with the specified incomplete character.
        /// </summary>
        /// <param name="other">The incomplete character to perform the XOR operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteChar Xor(IncompleteChar other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise XOR operation with the specified character.
        /// </summary>
        /// <param name="other">The character to perform the XOR operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteChar Xor(Char other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise AND operation with the specified incomplete character.
        /// </summary>
        /// <param name="other">The incomplete character to perform the AND operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteChar And(IncompleteChar other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise AND operation with the specified character.
        /// </summary>
        /// <param name="other">The character to perform the AND operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteChar And(Char other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise OR operation with the specified incomplete character.
        /// </summary>
        /// <param name="other">The incomplete character to perform the OR operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteChar Or(IncompleteChar other) => new()
        {
            Binary = Binary.Or(other.Binary)
        };

        public IncompleteChar Nand(IncompleteChar other) => new()
        {
            Binary = Binary.Nand(other.Binary)
        };

        public IncompleteChar Nand(Char other) => new()
        {
            Binary = Binary.Nand(other.Binary)
        };

        public IncompleteChar Nor(IncompleteChar other) => new()
        {
            Binary = Binary.Nor(other.Binary)
        };

        public IncompleteChar Nor(Char other) => new()
        {
            Binary = Binary.Nor(other.Binary)
        };

        public IncompleteChar Xnor(IncompleteChar other) => new()
        {
            Binary = Binary.Xnor(other.Binary)
        };

        public IncompleteChar Xnor(Char other) => new()
        {
            Binary = Binary.Xnor(other.Binary)
        };

        public IncompleteChar ShiftLeft(int count) => new()
        {
            Binary = Binary.ShiftLeft(count)
        };

        public IncompleteChar ShiftRight(int count) => new()
        {
            Binary = Binary.ShiftRight(count)
        };

        /// <summary>
        /// Performs a reverse bitwise AND operation with the specified character.
        /// </summary>
        /// <param name="right">The character to perform the reverse AND operation with.</param>
        /// <param name="result">The result of the reverse AND operation.</param>
        /// <returns><c>true</c> if the reverse AND operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseAnd(Char right, out IncompleteChar result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteChar()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a reverse bitwise AND operation with the specified incomplete character.
        /// </summary>
        /// <param name="right">The incomplete character to perform the reverse AND operation with.</param>
        /// <param name="result">The result of the reverse AND operation.</param>
        /// <returns><c>true</c> if the reverse AND operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseAnd(IncompleteChar right, out IncompleteChar result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteChar()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a reverse bitwise OR operation with the specified character.
        /// </summary>
        /// <param name="right">The character to perform the reverse OR operation with.</param>
        /// <param name="result">The result of the reverse OR operation.</param>
        /// <returns><c>true</c> if the reverse OR operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseOr(Char right, out IncompleteChar result)
        {
            if (!Binary.CanReverseOr(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteChar()
            {
                Binary = Binary.ReverseOr(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a reverse bitwise OR operation with the specified incomplete character.
        /// </summary>
        /// <param name="right">The incomplete character to perform the reverse OR operation with.</param>
        /// <param name="result">The result of the reverse OR operation.</param>
        /// <returns><c>true</c> if the reverse OR operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseOr(IncompleteChar right, out IncompleteChar result)
        {
            if (!Binary.CanReverseOr(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteChar()
            {
                Binary = Binary.ReverseOr(right.Binary)
            };
            return true;
        }
    }
}