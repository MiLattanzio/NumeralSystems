using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using NumeralSystems.Net.Utils;

namespace NumeralSystems.Net.Type.Base
{
    public class Byte : IRegularOperable<IncompleteByte, Byte, byte>
    {
        public virtual byte Value { get; set; }

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

        public bool[] Binary
        {
            get => Value.ToBoolArray();
            set => Value = value.Length >= sizeof(byte) * 8 ? value.Take(sizeof(byte) * 8).ToArray().ToByte() : value.Concat(Enumerable.Repeat(false, sizeof(byte)*8 - value.Length*8)).ToArray().ToByte();
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

        public byte[] Bytes
        {
            get => new[] { Value };
            set => Value = value.Length == 0 ? (byte)0 : value[0];
        }


        public Byte Not() => new()
        {
            Binary = Binary.Not()
        };

        public Byte Xor(Byte value) => new()
        {
            Binary = Binary.Xor(value.Binary)
        };

        public IncompleteByte Xor(IncompleteByte value) => new()
        {
            Binary = Binary.Xor(value.Binary)
        };

        public Byte And(Byte value) => new()
        {
            Binary = Binary.And(value.Binary)
        };

        public IncompleteByte And(IncompleteByte value) => new ()
        {
            Binary = Binary.And(value.Binary)
        };

        public Byte Or(Byte value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        public IncompleteByte Or(IncompleteByte value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        public Byte Nand(Byte value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        public IncompleteByte Nand(IncompleteByte value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        public IncompleteByte Incomplete() => new ()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        public override string ToString() => string.Join(string.Empty, Binary.Reverse().Select(x => x ? "1" : "0"));

        public string ToString(string format) => Value.ToString(format);

        
    }
}