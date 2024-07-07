using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using NumeralSystems.Net.Utils;

namespace NumeralSystems.Net.Type.Base
{
    public  class Byte : IRegularOperable<IncompleteByte, NumeralByte, byte>
    {

        public virtual byte Value { get; set; }

        public bool ReverseAnd(NumeralByte right, out IncompleteByte result) 
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

        public bool ReverseOr(NumeralByte right, out IncompleteByte result)
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
            set => Value = value.ToByte();
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

        public static NumeralByte FromBinary(bool[] binary) => new(binary.ToByte());

        public NumeralByte And(NumeralByte value) => FromBinary(Binary.And(value.Binary));

        public IncompleteByte And(IncompleteByte value) => new ()
        {
            Binary = Binary.And(value.Binary)
        };

        public NumeralByte Or(NumeralByte value) => FromBinary(Binary.Or(value.Binary));

        public IncompleteByte Or(IncompleteByte value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        public IncompleteByte Incomplete() => new ()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        public override string ToString() => string.Join(string.Empty, Binary.Reverse().Select(x => x ? "1" : "0"));

        public string ToString(string format) => Value.ToString(format);

        
    }
}