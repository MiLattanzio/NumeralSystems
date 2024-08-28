using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using NumeralSystems.Net.Utils;

namespace NumeralSystems.Net.Type.Base
{
    /// <summary>
    /// Class representing a byte.
    /// </summary>
    public sealed class Byte : IRegularOperable<IncompleteByte, Byte, byte, uint>
    {
        /// <summary>
        /// Value of the byte.
        /// </summary>
        public byte Value { get; set; }
        
        /// <summary>
        /// Reverse operation for the bitwise and operation.
        /// </summary>
        /// <param name="right">Second parameter of the operation</param>
        /// <param name="result">Result of the operation if any</param>
        /// <returns>True if the operation was successful otherwise false</returns>
        public bool ReverseAnd(Byte right, out IncompleteByte result) 
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
        /// Reverse operation for the bitwise and operation.
        /// </summary>
        /// <param name="right">Second parameter of the operation</param>
        /// <param name="result">Result of the operation if any</param>
        /// <returns>True if the operation was successful otherwise false</returns>
        public bool ReverseAnd(IncompleteByte right, out IncompleteByte result)
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
        /// Reverse operation for the bitwise or operation.
        /// </summary>
        /// <param name="right">Second parameter of the operation</param>
        /// <param name="result">Result of the operation if any</param>
        /// <returns>True if the operation was successful otherwise false</returns>
        public bool ReverseOr(Byte right, out IncompleteByte result)
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
        /// Reverse operation for the bitwise or operation.
        /// </summary>
        /// <param name="right">Second parameter of the operation</param>
        /// <param name="result">Result of the operation if any</param>
        /// <returns>True if the operation was successful otherwise false</returns>
        public bool ReverseOr(IncompleteByte right, out IncompleteByte result)
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
        /// The value of the numeral as a binary array.
        /// </summary>
        public bool[] Binary
        {
            get => Value.ToBoolArray();
            set => Value = value.Length >= sizeof(byte) * 8 ? value.Take(sizeof(byte) * 8).ToArray().ToByte() : value.Concat(System.Linq.Enumerable.Repeat(false, sizeof(byte)*8 - value.Length*8)).ToArray().ToByte();
        }
        
        /// <summary>
        /// Get and set the value of the numeral at the given index.
        /// </summary>
        /// <param name="index">Index of the bit</param>
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
        /// The value of the numeral as a byte array.
        /// </summary>
        public byte[] Bytes
        {
            get => new[] { Value };
            set => Value = value.Length == 0 ? (byte)0 : value[0];
        }

        /// <summary>
        /// The length of the numeral type in bits.
        /// </summary>
        public int BitLength => sizeof(byte) * 8;

        /// <summary>
        /// Bitwise not operation.
        /// </summary>
        /// <returns>Bitwise not result</returns>
        public Byte Not() => new()
        {
            Binary = Binary.Not()
        };

        /// <summary>
        /// Bitwise xor operation.
        /// </summary>
        /// <param name="value">Second parameter of the operation</param>
        /// <returns>Bitwise xor on the second parameter</returns>
        public Byte Xor(Byte value) => new()
        {
            Binary = Binary.Xor(value.Binary)
        };

        /// <summary>
        /// Bitwise xor operation.
        /// </summary>
        /// <param name="value">Second parameter of the operation</param>
        /// <returns>Bitwise xor on the second parameter</returns>
        public IncompleteByte Xor(IncompleteByte value) => new()
        {
            Binary = Binary.Xor(value.Binary)
        };

        /// <summary>
        /// Bitwise and operation.
        /// </summary>
        /// <param name="value">Second parameter of the operation</param>
        /// <returns>Bitwise and on the second parameter</returns>
        public Byte And(Byte value) => new()
        {
            Binary = Binary.And(value.Binary)
        };

        /// <summary>
        /// Bitwise and operation.
        /// </summary>
        /// <param name="value">Second parameter of the operation</param>
        /// <returns>Bitwise and on the second parameter</returns>
        public IncompleteByte And(IncompleteByte value) => new ()
        {
            Binary = Binary.And(value.Binary)
        };

        /// <summary>
        /// Bitwise or operation.
        /// </summary>
        /// <param name="value">Second parameter of the operation</param>
        /// <returns>Bitwise or on the second parameter</returns>
        public Byte Or(Byte value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        /// <summary>
        /// Bitwise or operation.
        /// </summary>
        /// <param name="value">Second parameter of the operation</param>
        /// <returns>Bitwise or on the second parameter</returns>
        public IncompleteByte Or(IncompleteByte value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        /// <summary>
        /// Bitwise nand operation.
        /// </summary>
        /// <param name="value">Second parameter of the operation</param>
        /// <returns>Bitwise nand on the second parameter</returns>
        public Byte Nand(Byte value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        /// <summary>
        /// Bitwise nand operation.
        /// </summary>
        /// <param name="value">Second parameter of the operation</param>
        /// <returns>Bitwise nand on the second parameter</returns>
        public IncompleteByte Nand(IncompleteByte value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        /// <summary>
        /// Gets the incomplete representation of the value.
        /// </summary>
        /// <returns>The source value as an incomplete value</returns>
        public IncompleteByte Incomplete() => new ()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        /// <summary>
        /// Gets the value of the numeral as a string of 1s and 0s.
        /// </summary>
        /// <returns>The value of the numeral as a string of 1s and 0s.</returns>
        public override string ToString() => string.Join(string.Empty, Binary.Reverse().Select(x => x ? "1" : "0"));
        
        /// <summary>
        /// Gets the value of the numeral as a string formatted with the given input.
        /// </summary>
        /// <param name="format">Formatter for the value</param>
        /// <returns>The value of the numeral as a string formatted with the given input</returns>
        public string ToString(string format) => Value.ToString(format);

        
    }
}