using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using Math = NumeralSystems.Net.Utils.Math;

namespace NumeralSystems.Net.Type.Base
{
    /// <summary>
    /// Represents a double-precision floating-point number with various binary operations.
    /// </summary>
    public sealed partial class Double : IRegularOperable<IncompleteDouble, Double, double, ulong>
    {
        /// <summary>
        /// Creates a Double instance from a binary array.
        /// </summary>
        /// <param name="binary">The binary array representing the double value.</param>
        /// <returns>A new Double instance.</returns>
        public static Double FromBinary(bool[] binary) => new ()
        {
            Value = Utils.Convert.ToDouble(binary)
        };

        /// <summary>
        /// Gets or sets the double value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the byte representation of the double value.
        /// </summary>
        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value).ToArray();
            set => Value = value.Length >= sizeof(double) ? BitConverter.ToDouble(value, 0) : BitConverter.ToDouble(value.Concat(System.Linq.Enumerable.Repeat((byte)0, sizeof(double) - value.Length)).ToArray(), 0);
        }

        /// <summary>
        /// Gets the bit length of the double value.
        /// </summary>
        public int BitLength => sizeof(double) * 8;

        /// <summary>
        /// Gets or sets the binary representation of the double value.
        /// </summary>
        public bool[] Binary
        {
            get => Utils.Convert.ToBoolArray(Value);
            set => Value = value.Length * 8 >= sizeof(double) ? Utils.Convert.ToDouble(value) : Utils.Convert.ToDouble(value.Concat(System.Linq.Enumerable.Repeat(false, sizeof(double)*8 - value.Length*8)).ToArray());
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
        /// Performs a reverse AND operation with another Double and returns the result as an IncompleteDouble.
        /// </summary>
        /// <param name="right">The Double to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(Double right, out IncompleteDouble result)
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
        /// Performs a reverse AND operation with an IncompleteDouble and returns the result as an IncompleteDouble.
        /// </summary>
        /// <param name="right">The IncompleteDouble to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(IncompleteDouble right, out IncompleteDouble result)
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
        /// Performs a reverse OR operation with another Double and returns the result as an IncompleteDouble.
        /// </summary>
        /// <param name="right">The Double to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(Double right, out IncompleteDouble result)
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
        /// Performs a reverse OR operation with an IncompleteDouble and returns the result as an IncompleteDouble.
        /// </summary>
        /// <param name="right">The IncompleteDouble to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(IncompleteDouble right, out IncompleteDouble result)
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
        /// Performs a NOT operation on the Double and returns the result.
        /// </summary>
        /// <returns>The result of the NOT operation.</returns>
        public Double Not() => new()
        {
            Binary = Math.Not(Binary)
        };

        /// <summary>
        /// Performs an XOR operation with another Double and returns the result.
        /// </summary>
        /// <param name="value">The Double to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public Double Xor(Double value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an XOR operation with an IncompleteDouble and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteDouble to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteDouble Xor(IncompleteDouble value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with another Double and returns the result.
        /// </summary>
        /// <param name="value">The Double to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public Double And(Double value) => new()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with an IncompleteDouble and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteDouble to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteDouble And(IncompleteDouble value) => new()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with another Double and returns the result.
        /// </summary>
        /// <param name="value">The Double to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public Double Or(Double value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with an IncompleteDouble and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteDouble to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteDouble Or(IncompleteDouble value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with another Double and returns the result.
        /// </summary>
        /// <param name="value">The Double to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public Double Nand(Double value) => new()
        {
            Binary = Math.Nand(Binary, value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with an IncompleteDouble and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteDouble to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public IncompleteDouble Nand(IncompleteDouble value) => new()
        {
            Binary = Math.Nand(Binary, value.Binary)
        };

        /// <summary>
        /// Converts the Double to an IncompleteDouble.
        /// </summary>
        /// <returns>The IncompleteDouble representation of the Double.</returns>
        public IncompleteDouble Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        /// <summary>
        /// Returns the string representation of the Double value.
        /// </summary>
        /// <returns>The string representation of the Double value.</returns>
        public override string ToString() => string.Join(string.Empty, Bytes.Select(x => new Byte(){ Value = x}.ToString()));

        /// <summary>
        /// Converts the Double to a string using the specified format.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <returns>The formatted string representation of the Double.</returns>
        public string ToString(string format) =>  Value.ToString(format);
    }
}