using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Utils;
using Polecola.Primitive;
using Convert = Polecola.Primitive.Convert;
using Math = NumeralSystems.Net.Utils.Math;
using Double = NumeralSystems.Net.Type.Base.Double;

namespace NumeralSystems.Net.Type.Incomplete
{
    public class IncompleteDouble: IIRregularOperable<IncompleteDouble, Double, double, ulong>
    {
        private bool?[] _binary;

        /// <summary>
        /// Gets or sets the binary representation of the double.
        /// </summary>
        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8 * sizeof(double)).Select(x => x as bool?).ToArray();
            set
            {
                if (null == value)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8 * sizeof(double)).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= (8 * sizeof(double)))
                    {
                        _binary = value.Take(8 * sizeof(double)).ToArray();
                    }
                    else
                    {
                        _binary = System.Linq.Enumerable.Repeat(false, (8 * sizeof(double)) - value.Length).Select(x => x as bool?)
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
        /// Gets the double representation for the specified value.
        /// </summary>
        /// <param name="value">The value to get the double representation for.</param>
        /// <returns>The double representation.</returns>
        public Double this[ulong value]
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
                return new Double()
                {
                    Binary = resultBinary
                };
            }
        }

        /// <summary>
        /// Gets the enumerable of double representations.
        /// </summary>
        public IEnumerable<Double> Enumerable => Sequence.Range(0, Permutations).Select(x => this[x]);

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
        /// Performs a bitwise OR operation with the specified double.
        /// </summary>
        /// <param name="other">The double to perform the OR operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteDouble Or(Double other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Determines whether the binary representation contains the specified double.
        /// </summary>
        /// <param name="value">The double to check for.</param>
        /// <returns><c>true</c> if the binary representation contains the double; otherwise, <c>false</c>.</returns>
        public bool Contains(Double value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        /// <summary>
        /// Determines whether the binary representation contains the specified incomplete double.
        /// </summary>
        /// <param name="value">The incomplete double to check for.</param>
        /// <returns><c>true</c> if the binary representation contains the incomplete double; otherwise, <c>false</c>.</returns>
        public bool Contains(IncompleteDouble value)
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
        public IncompleteDouble Not() => new()
        {
            Binary = Binary.Not()
        };

        /// <summary>
        /// Performs a bitwise XOR operation with the specified incomplete double.
        /// </summary>
        /// <param name="other">The incomplete double to perform the XOR operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteDouble Xor(IncompleteDouble other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise XOR operation with the specified double.
        /// </summary>
        /// <param name="other">The double to perform the XOR operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteDouble Xor(Double other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise AND operation with the specified incomplete double.
        /// </summary>
        /// <param name="other">The incomplete double to perform the AND operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteDouble And(IncompleteDouble other) => new() {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise AND operation with the specified double.
        /// </summary>
        /// <param name="other">The double to perform the AND operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteDouble And(Double other) => new() {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise OR operation with the specified incomplete double.
        /// </summary>
        /// <param name="other">The incomplete double to perform the OR operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteDouble Or(IncompleteDouble other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteDouble Nand(IncompleteDouble other) => new()
        {
            Binary = Binary.Nand(other.Binary)
        };

        public IncompleteDouble Nand(Double other) => new()
        {
            Binary = Binary.Nand(other.Binary)
        };

        public IncompleteDouble Nor(IncompleteDouble other) => new()
        {
            Binary = Binary.Nor(other.Binary)
        };

        public IncompleteDouble Nor(Double other) => new()
        {
            Binary = Binary.Nor(other.Binary)
        };

        public IncompleteDouble Xnor(IncompleteDouble other) => new()
        {
            Binary = Binary.Xnor(other.Binary)
        };

        public IncompleteDouble Xnor(Double other) => new()
        {
            Binary = Binary.Xnor(other.Binary)
        };

        public IncompleteDouble ShiftLeft(int count) => new()
        {
            Binary = Binary.ShiftLeft(count)
        };

        public IncompleteDouble ShiftRight(int count) => new()
        {
            Binary = Binary.ShiftRight(count)
        };

        /// <summary>
        /// Performs a reverse bitwise AND operation with the specified double.
        /// </summary>
        /// <param name="right">The double to perform the reverse AND operation with.</param>
        /// <param name="result">The result of the reverse AND operation.</param>
        /// <returns><c>true</c> if the reverse AND operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseAnd(Double right, out IncompleteDouble result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteDouble()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a reverse bitwise AND operation with the specified incomplete double.
        /// </summary>
        /// <param name="right">The incomplete double to perform the reverse AND operation with.</param>
        /// <param name="result">The result of the reverse AND operation.</param>
        /// <returns><c>true</c> if the reverse AND operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseAnd(IncompleteDouble right, out IncompleteDouble result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteDouble()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a reverse bitwise OR operation with the specified double.
        /// </summary>
        /// <param name="right">The double to perform the reverse OR operation with.</param>
        /// <param name="result">The result of the reverse OR operation.</param>
        /// <returns><c>true</c> if the reverse OR operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseOr(Double right, out IncompleteDouble result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteDouble()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a reverse bitwise OR operation with the specified incomplete double.
        /// </summary>
        /// <param name="right">The incomplete double to perform the reverse OR operation with.</param>
        /// <param name="result">The result of the reverse OR operation.</param>
        /// <returns><c>true</c> if the reverse OR operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseOr(IncompleteDouble right, out IncompleteDouble result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteDouble()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }
    }
}