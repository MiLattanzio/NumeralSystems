using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using NumeralSystems.Net.Utils;
using Polecola.Primitive;

namespace NumeralSystems.Net.Type.Base
{
    /// <summary>
    /// Represents a 16-bit unsigned integer with various binary operations.
    /// </summary>
    public sealed partial class UShort: IRegularOperable<IncompleteUShort, UShort, ushort, uint>
    {
        /// <summary>
        /// Creates a UShort instance from a binary array.
        /// </summary>
        /// <param name="binary">The binary array representing the ushort value.</param>
        /// <returns>A new UShort instance.</returns>
        public static UShort FromBinary(bool[] binary) => new ()
        {
            Value = binary.ToUShort()
        };
        
        /// <summary>
        /// Gets or sets the unsigned short value.
        /// </summary>
        public ushort Value { get; set; }

        /// <summary>
        /// Gets or sets the byte representation of the unsigned short value.
        /// </summary>
        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value);
            set => Value = value.Length >= sizeof(ushort) ? BitConverter.ToUInt16(value.Take(sizeof(short)).ToArray(),0) : BitConverter.ToUInt16(value.Concat(System.Linq.Enumerable.Repeat((byte)0, sizeof(ushort) - value.Length)).ToArray(), 0);
        }

        /// <summary>
        /// Gets the bit length of the unsigned short value.
        /// </summary>
        public int BitLength => sizeof(ushort) * 8;

        /// <summary>
        /// Gets or sets the binary representation of the unsigned short value.
        /// </summary>
        public bool[] Binary
        {
            get => Value.ToBoolArray();
            set => Value = value.Length * 8 >= sizeof(ushort) ? value.Take(sizeof(ushort)*8).ToArray().ToUShort() : value.Concat(System.Linq.Enumerable.Repeat(false, sizeof(ushort)*8 - value.Length*8)).ToArray().ToUShort();
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
        /// Converts the UShort to a string using the specified format.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <returns>The formatted string representation of the UShort.</returns>
        public string ToString(string format)
        {
            try
            {
                return Value.ToString(format);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Value.ToString();
            }
        }

        /// <summary>
        /// Performs a reverse AND operation with another UShort and returns the result as an IncompleteUShort.
        /// </summary>
        /// <param name="right">The UShort to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(UShort right, out IncompleteUShort result)
        {
            if (Binary.CanReverseAnd(right.Binary))
            {
                result = new IncompleteUShort
                {
                    Binary = Binary.ReverseAnd(right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Performs a reverse AND operation with an IncompleteUShort and returns the result as an IncompleteUShort.
        /// </summary>
        /// <param name="right">The IncompleteUShort to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(IncompleteUShort right, out IncompleteUShort result)
        {
            if (Binary.CanReverseAnd(right.Binary))
            {
                result = new IncompleteUShort
                {
                    Binary = Binary.ReverseAnd(right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Performs a reverse OR operation with another UShort and returns the result as an IncompleteUShort.
        /// </summary>
        /// <param name="right">The UShort to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(UShort right, out IncompleteUShort result)
        {
            if (Binary.CanReverseOr(right.Binary))
            {
                result = new IncompleteUShort
                {
                    Binary = Binary.ReverseOr(right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Performs a reverse OR operation with an IncompleteUShort and returns the result as an IncompleteUShort.
        /// </summary>
        /// <param name="right">The IncompleteUShort to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(IncompleteUShort right, out IncompleteUShort result)
        {
            if (Binary.CanReverseOr(right.Binary))
            {
                result = new IncompleteUShort
                {
                    Binary = Binary.ReverseOr(right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Returns the string representation of the UShort value.
        /// </summary>
        /// <returns>The string representation of the UShort value.</returns>
        public override string ToString() => string.Join(string.Empty, Binary.Reverse().Select(x => x ? "1" : "0"));

        /// <summary>
        /// Performs a NOT operation on the UShort and returns the result.
        /// </summary>
        /// <returns>The result of the NOT operation.</returns>
        public UShort Not() => new ()
        {
            Binary = Binary.Not()
        };

        /// <summary>
        /// Performs an XOR operation with another UShort and returns the result.
        /// </summary>
        /// <param name="value">The UShort to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public UShort Xor(UShort value) => new()
        {
            Binary = Binary.Xor(value.Binary)
        };

        /// <summary>
        /// Performs an XOR operation with an IncompleteUShort and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteUShort to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteUShort Xor(IncompleteUShort value) => new()
        {
            Binary = Binary.Xor(value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with another UShort and returns the result.
        /// </summary>
        /// <param name="value">The UShort to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public UShort And(UShort value) => new()
        {
            Binary = Binary.And(value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with an IncompleteUShort and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteUShort to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteUShort And(IncompleteUShort value) => new()
        {
            Binary = Binary.And(value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with another UShort and returns the result.
        /// </summary>
        /// <param name="value">The UShort to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public UShort Or(UShort value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with an IncompleteUShort and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteUShort to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteUShort Or(IncompleteUShort value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with another UShort and returns the result.
        /// </summary>
        /// <param name="value">The UShort to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public UShort Nand(UShort value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with an IncompleteUShort and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteUShort to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public IncompleteUShort Nand(IncompleteUShort value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        /// <summary>
        /// Converts the UShort to an IncompleteUShort.
        /// </summary>
        /// <returns>The IncompleteUShort representation of the UShort.</returns>
        public IncompleteUShort Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };
    }
}