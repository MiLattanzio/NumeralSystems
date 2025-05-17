using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Type.Base
{
    /// <summary>
    /// Represents a 64-bit unsigned integer with various binary operations.
    /// </summary>
    public sealed partial class ULong: IRegularOperable<IncompleteULong, ULong, ulong, ulong>
    {
        /// <summary>
        /// Creates a ULong instance from a binary array.
        /// </summary>
        /// <param name="binary">The binary array representing the ulong value.</param>
        /// <returns>A new ULong instance.</returns>
        public static ULong FromBinary(bool[] binary) => new ()
        {
            Value = Polecola.Primitive.Convert.ToULong(binary)
        };

        /// <summary>
        /// Gets or sets the unsigned long value.
        /// </summary>
        public ulong Value { get; set; }

        /// <summary>
        /// Gets or sets the byte representation of the unsigned long value.
        /// </summary>
        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value).ToArray();
            // ReSharper disable once UnusedMember.Local
            set => Value = value.Length >= sizeof(ulong) ? BitConverter.ToUInt64(value, 0) : BitConverter.ToUInt64(value.Concat(System.Linq.Enumerable.Repeat((byte)0, sizeof(ulong) - value.Length)).ToArray(), 0);
        }

        /// <summary>
        /// Gets the bit length of the unsigned long value.
        /// </summary>
        public int BitLength => sizeof(ulong) * 8;

        /// <summary>
        /// Gets or sets the binary representation of the unsigned long value.
        /// </summary>
        public bool[] Binary
        {
            get => Polecola.Primitive.Convert.ToBoolArray(Value);
            set => Value = value.Length * 8 >= sizeof(ulong) ? Polecola.Primitive.Convert.ToULong(value) : Polecola.Primitive.Convert.ToULong(value.Concat(System.Linq.Enumerable.Repeat(false, sizeof(ulong) * 8 - value.Length * 8)).ToArray());
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
        /// Performs a reverse AND operation with another ULong and returns the result as an IncompleteULong.
        /// </summary>
        /// <param name="right">The ULong to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(ULong right, out IncompleteULong result)
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
        /// Performs a reverse AND operation with an IncompleteULong and returns the result as an IncompleteULong.
        /// </summary>
        /// <param name="right">The IncompleteULong to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(IncompleteULong right, out IncompleteULong result)
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
        /// Performs a reverse OR operation with another ULong and returns the result as an IncompleteULong.
        /// </summary>
        /// <param name="right">The ULong to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(ULong right, out IncompleteULong result)
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
        /// Performs a reverse OR operation with an IncompleteULong and returns the result as an IncompleteULong.
        /// </summary>
        /// <param name="right">The IncompleteULong to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(IncompleteULong right, out IncompleteULong result)
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
        /// Performs a NOT operation on the ULong and returns the result.
        /// </summary>
        /// <returns>The result of the NOT operation.</returns>
        public ULong Not() => new()
        {
            Binary = Utils.Math.Not(Binary)
        };

        /// <summary>
        /// Performs an XOR operation with another ULong and returns the result.
        /// </summary>
        /// <param name="value">The ULong to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public ULong Xor(ULong value) => new()
        {
            Binary = Utils.Math.Xor(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an XOR operation with an IncompleteULong and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteULong to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteULong Xor(IncompleteULong value) => new()
        {
            Binary = Utils.Math.Xor(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with another ULong and returns the result.
        /// </summary>
        /// <param name="value">The ULong to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public ULong And(ULong value) => new()
        {
            Binary = Utils.Math.And(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with an IncompleteULong and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteULong to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteULong And(IncompleteULong value) => new()
        {
            Binary = Utils.Math.And(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with another ULong and returns the result.
        /// </summary>
        /// <param name="value">The ULong to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public ULong Or(ULong value) => new()
        {
            Binary = Utils.Math.Or(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with an IncompleteULong and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteULong to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteULong Or(IncompleteULong value) => new()
        {
            Binary = Utils.Math.Or(Binary, value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with another ULong and returns the result.
        /// </summary>
        /// <param name="value">The ULong to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public ULong Nand(ULong value) => new()
        {
            Binary = Utils.Math.Nand(Binary, value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with an IncompleteULong and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteULong to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public IncompleteULong Nand(IncompleteULong value) => new()
        {
            Binary = Utils.Math.Nand(Binary, value.Binary)
        };

        /// <summary>
        /// Converts the ULong to an IncompleteULong.
        /// </summary>
        /// <returns>The IncompleteULong representation of the ULong.</returns>
        public IncompleteULong Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        /// <summary>
        /// Returns the string representation of the ULong value.
        /// </summary>
        /// <returns>The string representation of the ULong value.</returns>
        public override string ToString() => string.Join(string.Empty, Bytes.Select(x => new Byte(){ Value =  x }.ToString()));

        /// <summary>
        /// Converts the ULong to a string using the specified format.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <returns>The formatted string representation of the ULong.</returns>
        public string ToString(string format) => Value.ToString(format);
    }
}