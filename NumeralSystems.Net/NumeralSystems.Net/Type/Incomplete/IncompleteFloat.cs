using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;
using Convert = Polecola.Primitive.Convert;
using Polecola.Primitive;
using Float = NumeralSystems.Net.Type.Base.Float;

namespace NumeralSystems.Net.Type.Incomplete
{
    /// <summary>
    /// Represents an incomplete float with various bitwise operations.
    /// </summary>
    public class IncompleteFloat: IIRregularOperable<IncompleteFloat, Float, float, uint>
    {
        private bool?[] _binary;

        /// <summary>
        /// Gets or sets the binary representation of the float.
        /// </summary>
        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8 * sizeof(float)).Select(x => x as bool?).ToArray();
            set
            {
                if (null == value)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8 * sizeof(float)).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= (8 * sizeof(float)))
                    {
                        _binary = value.Take(8 * sizeof(float)).ToArray();
                    }
                    else
                    {
                        _binary = System.Linq.Enumerable.Repeat(false, (8 * sizeof(float)) - value.Length).Select(x => x as bool?)
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
        /// Gets the float representation for the specified value.
        /// </summary>
        /// <param name="value">The value to get the float representation for.</param>
        /// <returns>The float representation.</returns>
        public Float this[uint value]
        {
            get {
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
                return new Float()
                {
                    Binary = resultBinary
                };
            }
        }
        
        /// <summary>
        /// Gets the enumerable of float representations.
        /// </summary>
        public IEnumerable<Float> Enumerable => Sequence.Range(0, Permutations).Select(x => this[x]);

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
        /// Performs a bitwise OR operation with the specified float.
        /// </summary>
        /// <param name="other">The float to perform the OR operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteFloat Or(Float other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Determines whether the binary representation contains the specified float.
        /// </summary>
        /// <param name="value">The float to check for.</param>
        /// <returns><c>true</c> if the binary representation contains the float; otherwise, <c>false</c>.</returns>
        public bool Contains(Float value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        /// <summary>
        /// Determines whether the binary representation contains the specified incomplete float.
        /// </summary>
        /// <param name="value">The incomplete float to check for.</param>
        /// <returns><c>true</c> if the binary representation contains the incomplete float; otherwise, <c>false</c>.</returns>
        public bool Contains(IncompleteFloat value)
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
        public IncompleteFloat Not() => new()
        {
            Binary = Binary.Not()
        };

        /// <summary>
        /// Performs a bitwise XOR operation with the specified incomplete float.
        /// </summary>
        /// <param name="other">The incomplete float to perform the XOR operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteFloat Xor(IncompleteFloat other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise XOR operation with the specified float.
        /// </summary>
        /// <param name="other">The float to perform the XOR operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteFloat Xor(Float other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise AND operation with the specified incomplete float.
        /// </summary>
        /// <param name="other">The incomplete float to perform the AND operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteFloat And(IncompleteFloat other) => new() {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise AND operation with the specified float.
        /// </summary>
        /// <param name="other">The float to perform the AND operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteFloat And(Float other) => new() {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise OR operation with the specified incomplete float.
        /// </summary>
        /// <param name="other">The incomplete float to perform the OR operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteFloat Or(IncompleteFloat other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Performs a reverse bitwise AND operation with the specified float.
        /// </summary>
        /// <param name="right">The float to perform the reverse AND operation with.</param>
        /// <param name="result">The result of the reverse AND operation.</param>
        /// <returns><c>true</c> if the reverse AND operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseAnd(Float right, out IncompleteFloat result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteFloat()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a reverse bitwise AND operation with the specified incomplete float.
        /// </summary>
        /// <param name="right">The incomplete float to perform the reverse AND operation with.</param>
        /// <param name="result">The result of the reverse AND operation.</param>
        /// <returns><c>true</c> if the reverse AND operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseAnd(IncompleteFloat right, out IncompleteFloat result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteFloat()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a reverse bitwise OR operation with the specified float.
        /// </summary>
        /// <param name="right">The float to perform the reverse OR operation with.</param>
        /// <param name="result">The result of the reverse OR operation.</param>
        /// <returns><c>true</c> if the reverse OR operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseOr(Float right, out IncompleteFloat result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteFloat()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a reverse bitwise OR operation with the specified incomplete float.
        /// </summary>
        /// <param name="right">The incomplete float to perform the reverse OR operation with.</param>
        /// <param name="result">The result of the reverse OR operation.</param>
        /// <returns><c>true</c> if the reverse OR operation was successful; otherwise, <c>false</c>.</returns>
        public bool ReverseOr(IncompleteFloat right, out IncompleteFloat result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteFloat()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }
    }
}