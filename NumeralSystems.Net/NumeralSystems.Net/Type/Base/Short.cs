using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using NumeralSystems.Net.Utils;

namespace NumeralSystems.Net.Type.Base
{
    public sealed partial class Short: IRegularOperable<IncompleteShort, Short, short, uint>
    {
        public static Short FromBinary(bool[] binary) => new ()
        {
            Value = binary.ToShort()
        };
        
        public short Value { get; set; }

        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value);
            set => Value = value.Length >= sizeof(short) ? BitConverter.ToInt16(value.Take(sizeof(short)).ToArray(),0) : BitConverter.ToInt16(value.Concat(System.Linq.Enumerable.Repeat((byte)0, sizeof(short) - value.Length)).ToArray(), 0);
        }

        public int BitLength => sizeof(short) * 8;

        public bool[] Binary
        {
            get => Value.ToBoolArray();
            set => Value = value.Length * 8 >= sizeof(short) ? value.Take(sizeof(short)*8).ToArray().ToShort() : value.Concat(System.Linq.Enumerable.Repeat(false, sizeof(short)*8 - value.Length*8)).ToArray().ToShort();
        }
        
        public bool this[int index]
        {
            get => Binary[index];
            set {
                var binary = Binary;
                binary[index] = value;
                Binary = binary;
            }
        }

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
        
        public override string ToString() => string.Join(string.Empty, Binary.Reverse().Select(x => x ? "1" : "0"));

        public Short Not() => new ()
        {
            Binary = Binary.Not()
        };

        public Short Xor(Short value) => new()
        {
            Binary = Binary.Xor(value.Binary)
        };

        public IncompleteShort Xor(IncompleteShort value) => new()
        {
            Binary = Binary.Xor(value.Binary)
        };

        public Short And(Short value) => new()
        {
            Binary = Binary.And(value.Binary)
        };

        public IncompleteShort And(IncompleteShort value) => new()
        {
            Binary = Binary.And(value.Binary)
        };

        public Short Or(Short value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        public IncompleteShort Or(IncompleteShort value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        public Short Nand(Short value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        public IncompleteShort Nand(IncompleteShort value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        public IncompleteShort Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };
    }
}