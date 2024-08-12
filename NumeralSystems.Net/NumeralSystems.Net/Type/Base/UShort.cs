using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using NumeralSystems.Net.Utils;

namespace NumeralSystems.Net.Type.Base
{
    public partial class UShort: IRegularOperable<IncompleteUShort, UShort, ushort, uint>
    {
        public static UShort FromBinary(bool[] binary) => new ()
        {
            Value = binary.ToUShort()
        };
        
        public virtual ushort Value { get; set; }

        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value);
            set => Value = value.Length >= sizeof(ushort) ? BitConverter.ToUInt16(value.Take(sizeof(short)).ToArray(),0) : BitConverter.ToUInt16(value.Concat(System.Linq.Enumerable.Repeat((byte)0, sizeof(ushort) - value.Length)).ToArray(), 0);
        }

        public int BitLength => sizeof(ushort) * 8;

        public bool[] Binary
        {
            get => Value.ToBoolArray();
            set => Value = value.Length * 8 >= sizeof(ushort) ? value.Take(sizeof(ushort)*8).ToArray().ToUShort() : value.Concat(System.Linq.Enumerable.Repeat(false, sizeof(ushort)*8 - value.Length*8)).ToArray().ToUShort();
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


        public bool ReverseAnd(UShort right, out IncompleteUShort result)
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

        public bool ReverseAnd(IncompleteUShort right, out IncompleteUShort result)
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

        public bool ReverseOr(UShort right, out IncompleteUShort result)
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

        public bool ReverseOr(IncompleteUShort right, out IncompleteUShort result)
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

        public UShort Not() => new ()
        {
            Binary = Binary.Not()
        };

        public UShort Xor(UShort value) => new()
        {
            Binary = Binary.Xor(value.Binary)
        };

        public IncompleteUShort Xor(IncompleteUShort value) => new()
        {
            Binary = Binary.Xor(value.Binary)
        };

        public UShort And(UShort value) => new()
        {
            Binary = Binary.And(value.Binary)
        };

        public IncompleteUShort And(IncompleteUShort value) => new()
        {
            Binary = Binary.And(value.Binary)
        };

        public UShort Or(UShort value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        public IncompleteUShort Or(IncompleteUShort value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        public UShort Nand(UShort value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        public IncompleteUShort Nand(IncompleteUShort value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        public IncompleteUShort Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };
    }
}