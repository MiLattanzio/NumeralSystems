using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using NumeralSystems.Net.Utils;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = NumeralSystems.Net.Utils.Convert;

namespace NumeralSystems.Net.Type.Base
{
    /// <summary>
    /// Represents a single-precision floating-point number with various binary operations.
    /// </summary>
    public sealed partial class Float : IRegularOperable<IncompleteFloat, Float, float, uint>
    {
        /// <summary>
        /// Creates a Float instance from a binary array.
        /// </summary>
        /// <param name="binary">The binary array representing the float value.</param>
        /// <returns>A new Float instance.</returns>
        public static Float FromBinary(bool[] binary) => new ()
        {
            Value = binary.ToFloat()
        };

        /// <summary>
        /// Gets or sets the float value.
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Gets or sets the byte representation of the float value.
        /// </summary>
        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value).ToArray();
            // ReSharper disable once UnusedMember.Local
            set => Value = value.Length >= sizeof(float) ? BitConverter.ToSingle(value.Take(sizeof(float)).ToArray(),0) : BitConverter.ToSingle(value.Concat(System.Linq.Enumerable.Repeat((byte)0, sizeof(float) - value.Length)).ToArray(), 0);
        }

        /// <summary>
        /// Gets the bit length of the float value.
        /// </summary>
        public int BitLength => sizeof(float) * 8;

        /// <summary>
        /// Gets or sets the binary representation of the float value.
        /// </summary>
        public bool[] Binary
        {
            get => Value.ToBoolArray();
            set => Value = value.Length * 8 >= sizeof(float) ? value.Take(sizeof(float)*8).ToArray().ToFloat() : value.Concat(System.Linq.Enumerable.Repeat(false, sizeof(float)*8 - value.Length*8)).ToArray().ToFloat();
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
        /// Performs a reverse AND operation with another Float and returns the result as an IncompleteFloat.
        /// </summary>
        /// <param name="right">The Float to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(Float right, out IncompleteFloat result)
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
        /// Performs a reverse AND operation with an IncompleteFloat and returns the result as an IncompleteFloat.
        /// </summary>
        /// <param name="right">The IncompleteFloat to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(IncompleteFloat right, out IncompleteFloat result)
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
        /// Performs a reverse OR operation with another Float and returns the result as an IncompleteFloat.
        /// </summary>
        /// <param name="right">The Float to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(Float right, out IncompleteFloat result)
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
        /// Performs a reverse OR operation with an IncompleteFloat and returns the result as an IncompleteFloat.
        /// </summary>
        /// <param name="right">The IncompleteFloat to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(IncompleteFloat right, out IncompleteFloat result)
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
        /// Performs a NOT operation on the Float and returns the result.
        /// </summary>
        /// <returns>The result of the NOT operation.</returns>
        public Float Not() => new ()
        {
            Binary = Binary.Not()
        };

        /// <summary>
        /// Performs an XOR operation with another Float and returns the result.
        /// </summary>
        /// <param name="value">The Float to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public Float Xor(Float value) => new ()
        {
            Binary = Binary.Xor(value.Binary)
        };

        /// <summary>
        /// Performs an XOR operation with an IncompleteFloat and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteFloat to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteFloat Xor(IncompleteFloat value) => new ()
        {
            Binary = Binary.Xor(value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with another Float and returns the result.
        /// </summary>
        /// <param name="value">The Float to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public Float And(Float value) => new ()
        {
            Binary = Binary.And(value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with an IncompleteFloat and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteFloat to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteFloat And(IncompleteFloat value) => new ()
        {
            Binary = Binary.And(value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with another Float and returns the result.
        /// </summary>
        /// <param name="value">The Float to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public Float Or(Float value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with an IncompleteFloat and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteFloat to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteFloat Or(IncompleteFloat value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with another Float and returns the result.
        /// </summary>
        /// <param name="value">The Float to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public Float Nand(Float value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with an IncompleteFloat and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteFloat to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public IncompleteFloat Nand(IncompleteFloat value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        /// <summary>
        /// Converts the Float to an IncompleteFloat.
        /// </summary>
        /// <returns>The IncompleteFloat representation of the Float.</returns>
        public IncompleteFloat Incomplete() => new ()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        /// <summary>
        /// Returns the string representation of the Float value.
        /// </summary>
        /// <returns>The string representation of the Float value.</returns>
        public override string ToString() => string.Join(string.Empty, Bytes.Select(x => (new Byte() { Value = x})));

        /// <summary>
        /// Converts the Float to a string using the specified format.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <returns>The formatted string representation of the Float.</returns>
        public string ToString(string format) => Value.ToString(format);
    }
}
