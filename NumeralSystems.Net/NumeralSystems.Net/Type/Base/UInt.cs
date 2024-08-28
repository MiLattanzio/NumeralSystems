using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Type.Base
{
    /// <summary>
    /// Represents a 32-bit unsigned integer with various binary operations.
    /// </summary>
    public sealed partial class UInt: IRegularOperable<IncompleteUInt, UInt, uint, uint>
    {
        /// <summary>
        /// Gets or sets the unsigned integer value.
        /// </summary>
        public uint Value { get; set; }

        /// <summary>
        /// Gets or sets the byte representation of the unsigned integer value.
        /// </summary>
        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value).ToArray();
            // ReSharper disable once UnusedMember.Local
            set => Value = value.Length >= sizeof(uint) ? BitConverter.ToUInt32(value.Take(sizeof(uint)).ToArray(), 0) : BitConverter.ToUInt32(value.Concat(System.Linq.Enumerable.Repeat((byte)0, sizeof(uint) - value.Length)).ToArray(), 0);
        }

        /// <summary>
        /// Gets the bit length of the unsigned integer value.
        /// </summary>
        public int BitLength => sizeof(uint) * 8;

        /// <summary>
        /// Gets or sets the binary representation of the unsigned integer value.
        /// </summary>
        public bool[] Binary
        {
            get => Utils.Convert.ToBoolArray(Value);
            set => Value = value.Length * 8 >= sizeof(uint) ? Utils.Convert.ToUInt(value.Take(sizeof(uint)*8).ToArray()) : Utils.Convert.ToUInt(value.Concat(System.Linq.Enumerable.Repeat(false, sizeof(uint)*8 - value.Length*8)).ToArray());
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
        /// Performs a reverse AND operation with another UInt and returns the result as an IncompleteUInt.
        /// </summary>
        /// <param name="right">The UInt to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(UInt right, out IncompleteUInt result)
        {
            if (Utils.Math.CanReverseAnd(Binary, right.Binary))
            {
                result = new ()
                {
                    Binary = Utils.Math.ReverseAnd(Binary, right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Performs a reverse AND operation with an IncompleteUInt and returns the result as an IncompleteUInt.
        /// </summary>
        /// <param name="right">The IncompleteUInt to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(IncompleteUInt right, out IncompleteUInt result)
        {
            if (Utils.Math.CanReverseAnd(Binary, right.Binary))
            {
                result = new ()
                {
                    Binary = Utils.Math.ReverseAnd(Binary, right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Performs a reverse OR operation with another UInt and returns the result as an IncompleteUInt.
        /// </summary>
        /// <param name="right">The UInt to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(UInt right, out IncompleteUInt result)
        {
            if (Utils.Math.CanReverseOr(Binary, right.Binary))
            {
                result = new ()
                {
                    Binary = Utils.Math.ReverseOr(Binary, right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Performs a reverse OR operation with an IncompleteUInt and returns the result as an IncompleteUInt.
        /// </summary>
        /// <param name="right">The IncompleteUInt to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(IncompleteUInt right, out IncompleteUInt result)
        {
            if (Utils.Math.CanReverseOr(Binary, right.Binary))
            {
                result = new ()
                {
                    Binary = Utils.Math.ReverseOr(Binary, right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Performs a NOT operation on the UInt and returns the result.
        /// </summary>
        /// <returns>The result of the NOT operation.</returns>
        public UInt Not() => new()
        {
            Binary = Utils.Math.Not(Binary)
        };

        /// <summary>
        /// Performs an XOR operation with another UInt and returns the result.
        /// </summary>
        /// <param name="value">The UInt to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public UInt Xor(UInt value) => new()
        {
            Binary = Utils.Math.Xor(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an XOR operation with an IncompleteUInt and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteUInt to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteUInt Xor(IncompleteUInt value) => new()
        {
            Binary = Utils.Math.Xor(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with another UInt and returns the result.
        /// </summary>
        /// <param name="value">The UInt to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public UInt And(UInt value) => new()
        {
            Binary = Utils.Math.And(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with an IncompleteUInt and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteUInt to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteUInt And(IncompleteUInt value) => new()
        {
            Binary = Utils.Math.And(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with another UInt and returns the result.
        /// </summary>
        /// <param name="value">The UInt to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public UInt Or(UInt value) => new()
        {
            Binary = Utils.Math.Or(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with an IncompleteUInt and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteUInt to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteUInt Or(IncompleteUInt value) => new()
        {
            Binary = Utils.Math.Or(Binary, value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with another UInt and returns the result.
        /// </summary>
        /// <param name="value">The UInt to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public UInt Nand(UInt value) => new()
        {
            Binary = Utils.Math.Nand(Binary, value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with an IncompleteUInt and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteUInt to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public IncompleteUInt Nand(IncompleteUInt value) => new()
        {
            Binary = Utils.Math.Nand(Binary, value.Binary)
        };

        /// <summary>
        /// Converts the UInt to an IncompleteUInt.
        /// </summary>
        /// <returns>The IncompleteUInt representation of the UInt.</returns>
        public IncompleteUInt Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        /// <summary>
        /// Returns the string representation of the UInt value.
        /// </summary>
        /// <returns>The string representation of the UInt value.</returns>
        public override string ToString() => string.Join(string.Empty, Bytes.Select(x => new Byte(){ Value =  x }.ToString()));

        /// <summary>
        /// Converts the UInt to a string using the specified format.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <returns>The formatted string representation of the UInt.</returns>
        public string ToString(string format) => Value.ToString(format);
    }
}