using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = Polecola.Primitive.Convert;

namespace NumeralSystems.Net.Type.Base
{
    /// <summary>
    /// Represents a 64-bit signed integer with various binary operations.
    /// </summary>
    public sealed class Long : IRegularOperable<IncompleteLong, Long, long, ulong>
    {
        /// <summary>
        /// Creates a Long instance from a binary array.
        /// </summary>
        /// <param name="binary">The binary array representing the long value.</param>
        /// <returns>A new Long instance.</returns>
        public static Long FromBinary(bool[] binary) => new ()
        {
            Value = Polecola.Primitive.Convert.ToLong(binary)
        };

        /// <summary>
        /// Gets or sets the long value.
        /// </summary>
        public long Value { get; set; }

        /// <summary>
        /// Gets or sets the byte representation of the long value.
        /// </summary>
        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value);
            // ReSharper disable once UnusedMember.Local
            set => Value = value.Length >= sizeof(long) ? BitConverter.ToInt64(value, 0) : BitConverter.ToInt64(value.Concat(System.Linq.Enumerable.Repeat((byte)0, sizeof(long) - value.Length)).ToArray(), 0);
        }

        /// <summary>
        /// Gets the bit length of the long value.
        /// </summary>
        public int BitLength => sizeof(long) * 8;

        /// <summary>
        /// Gets or sets the binary representation of the long value.
        /// </summary>
        public bool[] Binary
        {
            get => Convert.ToBoolArray(Value);
            set => Value = value.Length * 8 >= sizeof(long) ? Convert.ToLong(value) : Convert.ToLong(value.Concat(System.Linq.Enumerable.Repeat(false, sizeof(long) * 8 - value.Length * 8)).ToArray());
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
        /// Performs a reverse AND operation with another Long and returns the result as an IncompleteLong.
        /// </summary>
        /// <param name="right">The Long to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(Long right, out IncompleteLong result)
        {
            if (Math.CanReverseAnd(Binary, right.Binary))
            {
                result = new ()
                {
                    Binary = Math.ReverseAnd(Binary, right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Performs a reverse AND operation with an IncompleteLong and returns the result as an IncompleteLong.
        /// </summary>
        /// <param name="right">The IncompleteLong to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(IncompleteLong right, out IncompleteLong result)
        {
            if (Math.CanReverseAnd(Binary, right.Binary))
            {
                result = new ()
                {
                    Binary = Math.ReverseAnd(Binary, right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Performs a reverse OR operation with another Long and returns the result as an IncompleteLong.
        /// </summary>
        /// <param name="right">The Long to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(Long right, out IncompleteLong result)
        {
            if (Math.CanReverseOr(Binary, right.Binary))
            {
                result = new ()
                {
                    Binary = Math.ReverseOr(Binary, right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Performs a reverse OR operation with an IncompleteLong and returns the result as an IncompleteLong.
        /// </summary>
        /// <param name="right">The IncompleteLong to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(IncompleteLong right, out IncompleteLong result)
        {
            if (Math.CanReverseOr(Binary, right.Binary))
            {
                result = new ()
                {
                    Binary = Math.ReverseOr(Binary, right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Performs a NOT operation on the Long and returns the result.
        /// </summary>
        /// <returns>The result of the NOT operation.</returns>
        public Long Not() => new()
        {
            Binary = Math.Not(Binary)
        };

        /// <summary>
        /// Performs an XOR operation with another Long and returns the result.
        /// </summary>
        /// <param name="value">The Long to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public Long Xor(Long value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an XOR operation with an IncompleteLong and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteLong to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteLong Xor(IncompleteLong value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with another Long and returns the result.
        /// </summary>
        /// <param name="value">The Long to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public Long And(Long value) => new ()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with an IncompleteLong and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteLong to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteLong And(IncompleteLong value) => new ()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with another Long and returns the result.
        /// </summary>
        /// <param name="value">The Long to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public Long Or(Long value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with an IncompleteLong and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteLong to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteLong Or(IncompleteLong value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with another Long and returns the result.
        /// </summary>
        /// <param name="value">The Long to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public Long Nand(Long value) => new()
        {
            Binary = Math.Nand(Binary, value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with an IncompleteLong and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteLong to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public IncompleteLong Nand(IncompleteLong value) => new()
        {
            Binary = Math.Nand(Binary, value.Binary)
        };

        /// <summary>
        /// Converts the Long to an IncompleteLong.
        /// </summary>
        /// <returns>The IncompleteLong representation of the Long.</returns>
        public IncompleteLong Incomplete() => new ()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        /// <summary>
        /// Returns the string representation of the Long value.
        /// </summary>
        /// <returns>The string representation of the Long value.</returns>
        public override string ToString() =>string.Join(string.Empty, Bytes.Select(x => (new Byte() {Value = x})));

        /// <summary>
        /// Converts the Long to a string using the specified format.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <returns>The formatted string representation of the Long.</returns>
        public string ToString(string format) => Value.ToString(format);

    }
}