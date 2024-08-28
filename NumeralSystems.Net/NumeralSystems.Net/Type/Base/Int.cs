using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = NumeralSystems.Net.Utils.Convert;

namespace NumeralSystems.Net.Type.Base
{
    /// <summary>
    /// Represents a 32-bit signed integer with various binary operations.
    /// </summary>
    public sealed class Int: IRegularOperable<IncompleteInt, Int, int, uint>
    {
        /// <summary>
        /// Gets or sets the integer value.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Gets or sets the byte representation of the integer value.
        /// </summary>
        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value).ToArray();
            // ReSharper disable once UnusedMember.Local
            set => Value = value.Length >= sizeof(int) ? BitConverter.ToInt32(value.Take(sizeof(int)).ToArray(),0) : BitConverter.ToInt32(value.Concat(System.Linq.Enumerable.Repeat((byte)0, sizeof(int) - value.Length)).ToArray(), 0);
        }

        /// <summary>
        /// Gets the bit length of the integer value.
        /// </summary>
        public int BitLength => sizeof(int) * 8;

        /// <summary>
        /// Gets or sets the binary representation of the integer value.
        /// </summary>
        public bool[] Binary
        {
            get => Convert.ToBoolArray(Value);
            set => Value = value.Length * 8 >= sizeof(int) ? Convert.ToInt(value.Take(sizeof(int)*8).ToArray()) : Convert.ToInt(value.Concat(System.Linq.Enumerable.Repeat(false, sizeof(int)*8 - value.Length*8)).ToArray());
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
        /// Performs a reverse AND operation with another Int and returns the result as an IncompleteInt.
        /// </summary>
        /// <param name="right">The Int to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(Int right, out IncompleteInt result)
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
        /// Performs a reverse AND operation with an IncompleteInt and returns the result as an IncompleteInt.
        /// </summary>
        /// <param name="right">The IncompleteInt to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(IncompleteInt right, out IncompleteInt result)
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
        /// Performs a reverse OR operation with another Int and returns the result as an IncompleteInt.
        /// </summary>
        /// <param name="right">The Int to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(Int right, out IncompleteInt result)
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
        /// Performs a reverse OR operation with an IncompleteInt and returns the result as an IncompleteInt.
        /// </summary>
        /// <param name="right">The IncompleteInt to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(IncompleteInt right, out IncompleteInt result)
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
        /// Performs a NOT operation on the Int and returns the result.
        /// </summary>
        /// <returns>The result of the NOT operation.</returns>
        public Int Not() => new()
        {
            Binary = Math.Not(Binary)
        };

        /// <summary>
        /// Performs an XOR operation with another Int and returns the result.
        /// </summary>
        /// <param name="value">The Int to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public Int Xor(Int value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an XOR operation with an IncompleteInt and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteInt to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteInt Xor(IncompleteInt value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with another Int and returns the result.
        /// </summary>
        /// <param name="value">The Int to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public Int And(Int value) => new()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with an IncompleteInt and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteInt to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteInt And(IncompleteInt value) => new()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with another Int and returns the result.
        /// </summary>
        /// <param name="value">The Int to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public Int Or(Int value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with an IncompleteInt and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteInt to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteInt Or(IncompleteInt value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with another Int and returns the result.
        /// </summary>
        /// <param name="value">The Int to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public Int Nand(Int value) => new()
        {
            Binary = Math.Nand(Binary, value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with an IncompleteInt and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteInt to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public IncompleteInt Nand(IncompleteInt value) => new()
        {
            Binary = Math.Nand(Binary, value.Binary)
        };

        /// <summary>
        /// Converts the Int to an IncompleteInt.
        /// </summary>
        /// <returns>The IncompleteInt representation of the Int.</returns>
        public IncompleteInt Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        /// <summary>
        /// Returns the string representation of the Int value.
        /// </summary>
        /// <returns>The string representation of the Int value.</returns>
        public override string ToString() => string.Join(string.Empty, Bytes.Select(x => new Byte(){ Value =  x }.ToString()));

        /// <summary>
        /// Converts the Int to a string using the specified format.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <returns>The formatted string representation of the Int.</returns>
        public string ToString(string format) => Value.ToString(format);

    }
}