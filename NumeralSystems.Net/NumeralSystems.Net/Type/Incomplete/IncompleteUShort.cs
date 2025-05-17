using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;
using Polecola.Primitive;

namespace NumeralSystems.Net.Type.Incomplete
{
    /// <summary>
    /// Represents an incomplete unsigned short (ushort) value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>IncompleteUShort</c> class represents an unsigned short value with missing bits that are represented as null values in an array of nullable booleans. It implements various bitwise operations and can be used to perform operations on incomplete binary values.
    /// </para>
    /// <para>
    /// The <c>IncompleteUShort</c> class implements the <c>IIRregularOperable</c> interface, which defines the operations that can be performed on incomplete values.
    /// </para>
    /// </remarks>
    public class IncompleteUShort : IIRregularOperable<IncompleteUShort, UShort, ushort, uint>
    {
        private bool?[] _binary;

        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8 * sizeof(ushort)).Select(x => x as bool?).ToArray();
            internal set
            {
                if (null == value)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8 * sizeof(ushort)).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= (8 * sizeof(ushort)))
                    {
                        _binary = value.Take(8 * sizeof(ushort)).ToArray();
                    }
                    else
                    {
                        _binary = System.Linq.Enumerable.Repeat(false, (8 * sizeof(ushort)) - value.Length)
                            .Select(x => x as bool?)
                            .Concat(value).ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// Represents an incomplete unsigned short value.
        /// </summary>
        public bool IsComplete => Binary.All(x => x != null);
        public uint Permutations => Sequence.PermutationsCount(2, Sequence.CountToUInt(Binary.Where(x => x is null)), true);

        /// <summary>
        /// Represents an incomplete ushort value that can be used for bitwise operations.
        /// </summary>
        /// <typeparam name="TValue">The type of numeral value.</typeparam>
        /// <typeparam name="TType">The underlying type of the numeral value.</typeparam>
        /// <typeparam name="TIndexer">The type of the indexer for the incomplete value.</typeparam>
        /// <remarks>
        /// This class implements the <see cref="Interface.IIncompleteValue{TValue, TType, TIndexer}"/> interface.
        /// </remarks>
        public UShort this[uint value]
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
                return new UShort
                {
                    Binary = resultBinary
                };
            }
        }

        public IEnumerable<UShort> Enumerable => Sequence.Range(0, Permutations).Select(x => this[x]);

        /// *Property: ByteArray**
        /// *Description:**
        public IncompleteByte[] ByteArray => IncompleteByteArray.ArrayOf(Binary);

        /// <summary>
        /// Returns the IncompleteUShort object represented as an array of IncompleteByte objects.
        /// </summary>
        /// <returns>An array of IncompleteByte objects representing the IncompleteUShort object.</returns>
        public IncompleteByte[] ToByteArray() => IncompleteByteArray.ArrayOf(Binary.Select(x => x).ToArray());

        /// <summary>
        /// Returns a string representation of the IncompleteUShort object.
        /// </summary>
        /// <param name="missingSeparator">The separator to use for missing values.</param>
        /// <returns>A string representation of the IncompleteUShort object.</returns>
        public string ToString(string missingSeparator = "*") => string.Join(string.Empty,
            Binary.Group(8).Select(x => x.Reverse().ToArray()).SelectMany(x => x)
                .Select(x => null == x ? missingSeparator : (x.Value ? 1 : 0).ToString()));

        /// <summary>
        /// Represents an incomplete unsigned short (ushort) value.
        /// This class implements the <see cref="IIRregularOperable{TIncomplete, TValue, TType, TIndexer}"/> interface.
        /// </summary>
        public IncompleteUShort Or(UShort other) => new()
        {
            Binary = Binary.Or(other.Binary)
        };

        /// <summary>
        /// Determines whether the binary representation of the <see cref="UShort"/> value contains the binary representation of the specified <see cref="UShort"/> value.
        /// </summary>
        /// <param name="value">The <see cref="UShort"/> value to check.</param>
        /// <returns>
        /// true if the binary representation of the current <see cref="UShort"/> value contains the binary representation of the specified <see cref="UShort"/> value; otherwise, false.
        /// </returns>
        public bool Contains(UShort value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        /// Determines whether the IncompleteUShort object contains the specified value.
        /// </summary>
        /// <param name="value">The value to check for containment.</param>
        /// <returns>true if the IncompleteUShort object contains the specified value; otherwise, false.</returns>
        public bool Contains(IncompleteUShort value)
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
        /// Returns a new IncompleteUShort object with the binary representation of the current IncompleteUShort object after applying the bitwise logical NOT operation to each bit.
        /// </summary>
        /// <returns>A new IncompleteUShort object representing the result of the bitwise logical NOT operation.</returns>
        /// <remarks>
        /// The Not method returns a new IncompleteUShort object with the binary representation of the current IncompleteUShort object after applying the bitwise logical NOT operation to each bit. The resulting binary array will have the same length as the original binary array, with true values representing false and false values representing true.
        /// </remarks>
        public IncompleteUShort Not() => new()
        {
            Binary = Binary.Not()
        };

        /// Performs the exclusive OR operation between two IncompleteUShort objects or an IncompleteUShort object and a UShort object.
        /// @param other The second operand of the XOR operation.
        /// @return A new IncompleteUShort object representing the result of the XOR operation.
        /// /
        public IncompleteUShort Xor(IncompleteUShort other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise exclusive OR operation between the binary representation of the current IncompleteUShort object and the binary representation of the specified UShort object.
        /// </summary>
        /// <param name="other">The UShort object to perform the XOR operation with.</param>
        /// <returns>A new IncompleteUShort object with the result of the XOR operation.</returns>
        public IncompleteUShort Xor(UShort other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        /// <summary>
        /// Represents an incomplete unsigned short value. Provides methods for bitwise operations and conversion to other types.
        /// </summary>
        public IncompleteUShort And(IncompleteUShort other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise AND operation between the binary representation of the current instance
        /// and the binary representation of the specified UShort value.
        /// </summary>
        /// <param name="other">The UShort value to perform the bitwise AND operation with.</param>
        /// <returns>A new instance of IncompleteUShort with the result of the bitwise AND operation.</returns>
        public IncompleteUShort And(UShort other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        /// Calculates the bitwise OR of the current IncompleteUShort object with another IncompleteUShort object.
        /// @param other The other IncompleteUShort object to perform the bitwise OR with.
        /// @return A new IncompleteUShort object that represents the result of the bitwise OR operation.
        /// /
        public IncompleteUShort Or(IncompleteUShort other) => new()
        {
            Binary = Binary.Or(other.Binary)
        };

        /// <summary>
        /// Performs a bitwise reverse AND operation on the binary representations of two UShort objects.
        /// </summary>
        /// <param name="right">The UShort object to reverse AND with.</param>
        /// <param name="result">When this method returns, contains the result of the reverse AND operation.</param>
        /// <returns>true if the reverse AND operation is successful; otherwise, false.</returns>
        public bool ReverseAnd(UShort right, out IncompleteUShort result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }

            result = new IncompleteUShort()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a logical AND operation between the binary representation of two IncompleteUShort instances and reverses the result.
        /// </summary>
        /// <param name="right">The IncompleteUShort instance to perform the operation with.</param>
        /// <param name="result">When this method returns, contains the IncompleteUShort instance that represents the result of the operation, if the operation succeeded; otherwise, null. This parameter is passed uninitialized.</param>
        /// <returns>true if the operation succeeded; otherwise, false.</returns>
        public bool ReverseAnd(IncompleteUShort right, out IncompleteUShort result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }

            result = new IncompleteUShort()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// Reverses the or operation between the binary representations of two IncompleteUShort values.
        /// </summary>
        /// <param name="right">The IncompleteUShort value to perform the operation with.</param>
        /// <param name="result">When this method returns, contains the result of the operation, if the operation succeeded, or null if the operation failed. The result parameter is passed uninitialized.</param>
        /// <returns>true if the operation succeeded, false otherwise.</returns>
        public bool ReverseOr(UShort right, out IncompleteUShort result)
        {
            if (!Binary.CanReverseOr(right.Binary))
            {
                result = null;
                return false;
            }

            result = new IncompleteUShort()
            {
                Binary = Binary.ReverseOr(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Reverses the binary representation and performs the logical OR operation on two instances of IncompleteUShort.
        /// </summary>
        /// <param name="right">The second IncompleteUShort instance to reverse OR.</param>
        /// <param name="result">When this method returns, contains the IncompleteUShort instance that is the result of the reverse OR operation. If the operation fails, the result parameter is set to null.</param>
        /// <returns>
        /// true if the reverse OR operation is successful; otherwise, false.
        /// </returns>
        public bool ReverseOr(IncompleteUShort right, out IncompleteUShort result)
        {
            if (!Binary.CanReverseOr(right.Binary))
            {
                result = null;
                return false;
            }

            result = new IncompleteUShort()
            {
                Binary = Binary.ReverseOr(right.Binary)
            };
            return true;
        }
    }
}