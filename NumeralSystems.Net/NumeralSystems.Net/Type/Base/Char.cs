using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using NumeralSystems.Net.Utils;
using Polecola.Primitive;
using Convert = Polecola.Primitive.Convert;
using Math = NumeralSystems.Net.Utils.Math;

namespace NumeralSystems.Net.Type.Base
{
    /// <summary>
    /// Represents a character with various binary operations.
    /// </summary>
    public sealed class Char : IRegularOperable<IncompleteChar, Char, char, uint>
    {
        /// <summary>
        /// Creates a Char instance from a binary array.
        /// </summary>
        /// <param name="binary">The binary array representing the character.</param>
        /// <returns>A new Char instance.</returns>
        public static Char FromBinary(bool[] binary) => new ()
        {
            Value = binary.ToChar()
        };
        
        /// <summary>
        /// Gets or sets the character value.
        /// </summary>
        public char Value { get; set; }

        /// <summary>
        /// Gets or sets the byte representation of the character.
        /// </summary>
        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value);
            set => Value = value.Length >= sizeof(char) ? BitConverter.ToChar(value.Take(sizeof(char)).ToArray(),0) : BitConverter.ToChar(value.Concat(System.Linq.Enumerable.Repeat((byte)0, sizeof(char) - value.Length)).ToArray(), 0);
        }

        /// <summary>
        /// Gets the bit length of the character.
        /// </summary>
        public int BitLength => sizeof(char) * 8;

        /// <summary>
        /// Gets or sets the binary representation of the character.
        /// </summary>
        public bool[] Binary
        {
            get => Value.ToBoolArray();
            set => Value = value.Length * 8 >= sizeof(char) ? value.Take(sizeof(char)*8).ToArray().ToChar() : value.Concat(System.Linq.Enumerable.Repeat(false, sizeof(char)*8 - value.Length*8)).ToArray().ToChar();
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
        /// Converts the character to a string using the specified format.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <returns>The formatted string representation of the character.</returns>
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
        /// Performs a reverse AND operation with another Char and returns the result as an IncompleteChar.
        /// </summary>
        /// <param name="right">The Char to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(Char right, out IncompleteChar result)
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
        /// Performs a reverse AND operation with an IncompleteChar and returns the result as an IncompleteChar.
        /// </summary>
        /// <param name="right">The IncompleteChar to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseAnd(IncompleteChar right, out IncompleteChar result)
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
        /// Performs a reverse OR operation with another Char and returns the result as an IncompleteChar.
        /// </summary>
        /// <param name="right">The Char to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(Char right, out IncompleteChar result)
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
        /// Performs a reverse OR operation with an IncompleteChar and returns the result as an IncompleteChar.
        /// </summary>
        /// <param name="right">The IncompleteChar to perform the operation with.</param>
        /// <param name="result">The result of the operation.</param>
        /// <returns>True if the operation was successful, otherwise false.</returns>
        public bool ReverseOr(IncompleteChar right, out IncompleteChar result)
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
        /// Returns the string representation of the binary value.
        /// </summary>
        /// <returns>The string representation of the binary value.</returns>
        public override string ToString() => string.Join(string.Empty, Binary.Reverse().Select(x => x ? "1" : "0"));

        /// <summary>
        /// Performs a NOT operation on the Char and returns the result.
        /// </summary>
        /// <returns>The result of the NOT operation.</returns>
        public Char Not() => new ()
        {
            Binary = Binary.Not()
        };

        /// <summary>
        /// Performs an XOR operation with another Char and returns the result.
        /// </summary>
        /// <param name="value">The Char to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public Char Xor(Char value) => new()
        {
            Binary = Binary.Xor(value.Binary)
        };

        /// <summary>
        /// Performs an XOR operation with an IncompleteChar and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteChar to perform the operation with.</param>
        /// <returns>The result of the XOR operation.</returns>
        public IncompleteChar Xor(IncompleteChar value) => new()
        {
            Binary = Binary.Xor(value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with another Char and returns the result.
        /// </summary>
        /// <param name="value">The Char to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public Char And(Char value) => new()
        {
            Binary = Binary.And(value.Binary)
        };

        /// <summary>
        /// Performs an AND operation with an IncompleteChar and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteChar to perform the operation with.</param>
        /// <returns>The result of the AND operation.</returns>
        public IncompleteChar And(IncompleteChar value) => new()
        {
            Binary = Binary.And(value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with another Char and returns the result.
        /// </summary>
        /// <param name="value">The Char to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public Char Or(Char value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        /// <summary>
        /// Performs an OR operation with an IncompleteChar and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteChar to perform the operation with.</param>
        /// <returns>The result of the OR operation.</returns>
        public IncompleteChar Or(IncompleteChar value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with another Char and returns the result.
        /// </summary>
        /// <param name="value">The Char to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public Char Nand(Char value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        /// <summary>
        /// Performs a NAND operation with an IncompleteChar and returns the result.
        /// </summary>
        /// <param name="value">The IncompleteChar to perform the operation with.</param>
        /// <returns>The result of the NAND operation.</returns>
        public IncompleteChar Nand(IncompleteChar value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        /// <summary>
        /// Converts the Char to an IncompleteChar.
        /// </summary>
        /// <returns>The IncompleteChar representation of the Char.</returns>
        public IncompleteChar Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };
    }
}