using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;
using Polecola.Primitive;

namespace NumeralSystems.Net.Type.Incomplete
{
    /// The IncompleteULong class represents an incomplete binary representation of an unsigned long integer.
    /// It implements the IIncompleteValue, IRregularReversible, and IIRregularOperable interfaces.
    /// This class provides methods for performing bitwise operations and checking containment of values.
    /// It also provides methods for converting the binary representation to other forms, such as a byte array or a string.
    /// @typeparam name="IncompleteULong">The type of the incomplete unsigned long integer.</typeparam> @typeparam name="ULong">The type of the complete unsigned long integer.</typeparam> @typeparam name="ulong">The underlying type of the incomplete unsigned long integer.</typeparam> @typeparam name="ulong">The underlying type of the complete unsigned long integer.</typeparam>
    /// @see IIncompleteValue
    /// @see IRregularReversible
    /// @see IIRregularOperable
    /// /
    public class IncompleteULong: IIRregularOperable<IncompleteULong, ULong, ulong, ulong>
    {
        private bool?[] _binary;

        /// <summary>
        /// Represents a property that holds a binary value.
        /// </summary>
        /// <remarks>
        /// This property is used in the <see cref="IncompleteULong"/> class to store and manipulate binary values.
        /// It is a nullable boolean array that represents a binary number, where each element in the array represents a bit in the binary value.
        /// The property is automatically initialized with a default binary value if it is not explicitly set.
        /// </remarks>
        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8 * sizeof(ulong)).Select(x => x as bool?).ToArray();
            set
            {
                if (null == value)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8 * sizeof(ulong)).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= (8 * sizeof(ulong)))
                    {
                        _binary = value.Take(8 * sizeof(uint)).ToArray();
                    }
                    else
                    {
                        _binary = System.Linq.Enumerable.Repeat(false, (8 * sizeof(ulong)) - value.Length).Select(x => x as bool?)
                            .Concat(value).ToArray();
                    }
                }
            }
        }
        public bool IsComplete => Binary.All(x => x != null);
        public ulong Permutations => Sequence.PermutationsCount(2, Sequence.CountToULong(Binary.Where(x => x is null)), true);

        public ULong this[ulong value]
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
                return new ULong()
                {
                    Binary = resultBinary
                };
            }
        }
        
        public IEnumerable<ULong> Enumerable => Sequence.Range(0, Permutations).Select(x => this[x]);

        public IncompleteByte[] ByteArray => IncompleteByteArray.ArrayOf(Binary);
        
        public IncompleteByte[] ToByteArray() => IncompleteByteArray.ArrayOf(Binary.Select(x => x).ToArray());

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString() => ToString("*");

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public string ToString(string missingSeparator) => string.Join(string.Empty, Binary.Group(8).Select(x => x.Reverse().ToArray()).SelectMany(x => x).Select(x => null == x ? missingSeparator : (x.Value ? 1 : 0).ToString()));
        public IncompleteULong Or(ULong other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Determines whether the binary representation of the current <see cref="IncompleteULong"/> value contains the binary representation of the specified <see cref="ULong"/> value.
        /// </summary>
        /// <param name="value">The <see cref="ULong"/> value to check for containment in the current <see cref="IncompleteULong"/> value.</param>
        /// <returns><c>true</c> if the binary representation of the current <see cref="IncompleteULong"/> value contains the binary representation of the specified <see cref="ULong"/> value; otherwise, <c>false</c>.</returns>
        public bool Contains(ULong value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        /// <summary>
        /// Checks whether the IncompleteULong contains a specified ULong value.
        /// </summary>
        /// <param name="value">The ULong value to check for.</param>
        /// <returns>True if the IncompleteULong contains the specified ULong value; otherwise, false.</returns>
        public bool Contains(IncompleteULong value)
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

        /// Represents an incomplete unsigned long value and provides operations and conversions for manipulating and working with such values.
        /// This class implements the IIRregularOperable interface.
        /// The IncompleteULong class provides properties and methods for retrieving, manipulating, and converting incomplete unsigned long values.
        /// /
        public IncompleteULong Not() => new()
        {
            Binary = Binary.Select(x => !x).ToArray()
        };

        /// <summary>
        /// Performs a bitwise exclusive OR (XOR) operation on two instances of <see cref="IncompleteULong"/> or between an instance of <see cref="IncompleteULong"/> and an instance of <see cref="ULong"/>.
        /// </summary>
        /// <param name="other">The other <see cref="IncompleteULong"/> or <see cref="ULong"/> to perform XOR with.</param>
        /// <returns>A new <see cref="IncompleteULong"/> that represents the result of the XOR operation.</returns>
        public IncompleteULong Xor(IncompleteULong other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        /// <summary>
        /// Performs the exclusive OR (XOR) operation on the binary representation of two incomplete unsigned long integers.
        /// </summary>
        /// <param name="other">The incomplete unsigned long integer to XOR with.</param>
        /// <returns>A new instance of <see cref="IncompleteULong"/> with the result of the XOR operation.</returns>
        public IncompleteULong Xor(ULong other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteULong And(IncompleteULong other) => new() {
            Binary = Binary.And(other.Binary)
        };

        /// Represents an incomplete unsigned long integer value.
        /// Provides operations for manipulating incomplete unsigned long values.
        /// Implements the `IIRregularOperable` interface.
        /// /
        public IncompleteULong And(ULong other) => new() {
            Binary = Binary.And(other.Binary)
        };

        /// Or operation between an IncompleteULong and an ULong.</summary>
        /// <param name="other">The ULong value to perform the Or operation with</param> <returns>A new IncompleteULong object representing the result of the Or operation</returns>
        /// <remarks>
        /// This method performs the logical Or operation between the Binary property of the current IncompleteULong object and the Binary property of the ULong value passed as the other parameter. The result is stored in a new IncompleteULong object, with the Binary property set to the result of the Or operation.
        /// </remarks>*/
        public IncompleteULong Or(IncompleteULong other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        /// <summary>
        /// Performs a reverse AND operation between the binary representation of the given ULong value and the binary representation of this IncompleteULong value.
        /// </summary>
        /// <param name="right">The ULong value to perform the reverse AND operation with.</param>
        /// <param name="result">When this method returns, contains the result of the reverse AND operation, or null if the operation cannot be performed.</param>
        /// <returns>True if the reverse AND operation was successful; otherwise, false.</returns>
        public bool ReverseAnd(ULong right, out IncompleteULong result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteULong()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Performs a reverse AND operation between two IncompleteULong objects.
        /// </summary>
        /// <param name="right">The second IncompleteULong object to perform the reverse AND operation with.</param>
        /// <param name="result">The result of the reverse AND operation.</param>
        /// <returns>
        /// Returns true if the reverse AND operation is successful and the result is assigned to the 'result' parameter.
        /// Returns false if the reverse AND operation is not possible and the 'result' parameter is null.
        /// </returns>
        public bool ReverseAnd(IncompleteULong right, out IncompleteULong result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteULong()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Reverses the bits of the current IncompleteULong instance and performs a bitwise OR operation with the specified ULong value.
        /// </summary>
        /// <param name="right">The ULong value to perform the bitwise OR operation with.</param>
        /// <param name="result">When this method returns, contains the result of the bitwise OR operation as an IncompleteULong instance. This parameter is passed uninitialized.</param>
        /// <returns><c>true</c> if the bitwise OR operation is successfully performed; otherwise, <c>false</c>.</returns>
        public bool ReverseOr(ULong right, out IncompleteULong result)
        {
            if (!Binary.CanReverseOr(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteULong()
            {
                Binary = Binary.ReverseOr(right.Binary)
            };
            return true;
        }

        /// <summary>
        /// Computes the reverse OR operation between two IncompleteULong instances.
        /// </summary>
        /// <param name="right">The IncompleteULong instance to perform the reverse OR operation with.</param>
        /// <param name="result">When this method returns, contains the result of the reverse OR operation if it succeeded, or null if it failed.</param>
        /// <returns>Returns true if the reverse OR operation succeeded; otherwise, false.</returns>
        public bool ReverseOr(IncompleteULong right, out IncompleteULong result)
        {
            if (!Binary.CanReverseOr(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteULong()
            {
                Binary = Binary.ReverseOr(right.Binary)
            };
            return true;
        }
    }
}