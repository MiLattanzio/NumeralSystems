using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using NumeralSystems.Net.Utils;
using Polecola.Primitive;

namespace NumeralSystems.Net.Type.Base
{
    /// <summary>
    /// Represents a 16-bit signed integer with various binary operations.
    /// </summary>
    public sealed partial class Short: IRegularOperable<IncompleteShort, Short, short, uint>
    {
        /// <summary>
        /// Creates a Short instance from a binary array.
        /// </summary>
        /// <param name="binary">The binary array representing the short value.</param>
        /// <returns>A new Short instance.</returns>
        public static Short FromBinary(bool[] binary) => new ()
        {
            Value = binary.ToShort()
        };

        /// <summary>
        /// Gets or sets the short value.
        /// </summary>
        public short Value { get; set; }

        /// <summary>
        /// Gets or sets the byte representation of the short value.
        /// </summary>
        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value);
            set => Value = value.Length >= sizeof(short) ? BitConverter.ToInt16(value.Take(sizeof(short)).ToArray(),0) : BitConverter.ToInt16(value.Concat(System.Linq.Enumerable.Repeat((byte)0, sizeof(short) - value.Length)).ToArray(), 0);
        }

        /// <summary>
        /// Gets the bit length of the short value.
        /// </summary>
        public int BitLength => sizeof(short) * 8;

        /// <summary>
        /// Gets or sets the binary representation of the short value.
        /// </summary>
        public bool[] Binary
        {
            get => Value.ToBoolArray();
            set => Value = value.Length * 8 >= sizeof(short) ? value.Take(sizeof(short)*8).ToArray().ToShort() : value.Concat(System.Linq.Enumerable.Repeat(false, sizeof(short)*8 - value.Length*8)).ToArray().ToShort();
        }

        /// <summary>
        /// Gets or sets the binary value at the specified index.
        /// </summary>
        /// <param name="index">The index of the binary value.</param>
        /// <returns>The binary value at the specified index.</returns>
        public bool this[int index]
        {
            get => Binary[index];
            set {
                var binary = Binary;
                binary[index] = value;
                Binary = binary;
            }
        }

        /// <summary>
        /// Converts the Short to a string using the specified format.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <returns>The formatted string representation of the Short.</returns>
        public string ToString(string format)
        {
            try
            {
                return ((int)Value).ToString(format);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Value.ToString();
            }
        }

        /// <summary>
        /// Performs a reverse AND operation with another Short and returns the result as an IncompleteShort.
        /// </summary>
        /// <param name="right">The Short to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(Short right, out IncompleteShort result)
        {
            if (Binary.CanReverseAnd(right.Binary))
            {
                result = new ()
                {
                    Binary = Binary.ReverseAnd(right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Performs a reverse AND operation with an IncompleteShort and returns the result as an IncompleteShort.
        /// </summary>
        /// <param name="right">The IncompleteShort to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(IncompleteShort right, out IncompleteShort result)
        {
            if (Binary.CanReverseAnd(right.Binary))
            {
                result = new ()
                {
                    Binary = Binary.ReverseAnd(right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Performs a reverse OR operation with another Short and returns the result as an IncompleteShort.
        /// </summary>
        /// <param name="right">The Short to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(Short right, out IncompleteShort result)
        {
            if (Binary.CanReverseOr(right.Binary))
            {
                result = new ()
                {
                    Binary = Binary.ReverseOr(right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Performs a reverse OR operation with an IncompleteShort and returns the result as an IncompleteShort.
        /// </summary>
        /// <param name="right">The IncompleteShort to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(IncompleteShort right, out IncompleteShort result)
        {
            if (Binary.CanReverseOr(right.Binary))
            {
                result = new ()
                {
                    Binary = Binary.ReverseOr(right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Returns the string representation of the Short value.
        /// </summary>
        /// <returns>The string representation of the Short value.</returns>
        public override string ToString() => string.Join(string.Empty, Binary.Reverse().Select(x => x ? "1" : "0"));

        /// <summary>
        /// Performs a NOT operation on the Short and returns the result.
        /// </summary>
        /// <returns>The result of the NOT operation.</returns>
        public Short Not() => new ()
        {
            Binary = Binary.Not()
        };

        /// <summary>
        /// Performs an XOR operation with another Short and returns the result.
        /// </summary>
        /// <param name="value">The Short to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public Short Xor(Short value) => new()
        {
            Binary = Binary.Xor(value.Binary)
        };

        /// <summary>
        /// Performs an XOR operation with an IncompleteShort and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteShort to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteShort Xor(IncompleteShort value) => new()
        {
            Binary = Binary.Xor(value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with another Short and returns the result.
        /// </summary>
        /// <param name="value">The Short to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public Short And(Short value) => new()
        {
            Binary = Binary.And(value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with an IncompleteShort and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteShort to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteShort And(IncompleteShort value) => new()
        {
            Binary = Binary.And(value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with another Short and returns the result.
        /// </summary>
        /// <param name="value">The Short to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public Short Or(Short value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with an IncompleteShort and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteShort to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteShort Or(IncompleteShort value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with another Short and returns the result.
        /// </summary>
        /// <param name="value">The Short to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public Short Nand(Short value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with an IncompleteShort and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteShort to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public IncompleteShort Nand(IncompleteShort value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        /// <summary>
        /// Converts the Short to an IncompleteShort.
        /// </summary>
        /// <returns>The IncompleteShort representation of the Short.</returns>
        public IncompleteShort Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };
    }
}