using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Type.Base
{
    public partial class UInt: IRegularOperable<IncompleteUInt, UInt, uint>
    {
        public static UInt FromBinary(bool[] binary) => new ()
        {
            Value = Utils.Convert.ToUInt(binary)
        };
        public virtual uint Value { get; set; }

        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value).ToArray();
            // ReSharper disable once UnusedMember.Local
            private set => Value = BitConverter.ToUInt32(value, 0);
        }

        public bool[] Binary
        {
            get => Utils.Convert.ToBoolArray(Value);
            private set => Value = Utils.Convert.ToUInt(value);
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

        public bool ReverseAnd(UInt right, out IncompleteUInt result)
        {
            if (Utils.Math.CanReverseAnd(Binary, right.Binary))
            {
                result = new ()
                {
                    Binary = Utils.Math.ReverseAnd(Binary, right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        public bool ReverseAnd(IncompleteUInt right, out IncompleteUInt result)
        {
            if (Utils.Math.CanReverseAnd(Binary, right.Binary))
            {
                result = new ()
                {
                    Binary = Utils.Math.ReverseAnd(Binary, right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        public bool ReverseOr(UInt right, out IncompleteUInt result)
        {
            if (Utils.Math.CanReverseOr(Binary, right.Binary))
            {
                result = new ()
                {
                    Binary = Utils.Math.ReverseOr(Binary, right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        public bool ReverseOr(IncompleteUInt right, out IncompleteUInt result)
        {
            if (Utils.Math.CanReverseOr(Binary, right.Binary))
            {
                result = new ()
                {
                    Binary = Utils.Math.ReverseOr(Binary, right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        public UInt Not() => new()
        {
            Binary = Utils.Math.Not(Binary)
        };

        public UInt Xor(UInt value) => new()
        {
            Binary = Utils.Math.Xor(Binary, value.Binary)
        };

        public IncompleteUInt Xor(IncompleteUInt value) => new()
        {
            Binary = Utils.Math.Xor(Binary, value.Binary)
        };

        public UInt And(UInt value) => new()
        {
            Binary = Utils.Math.And(Binary, value.Binary)
        };

        public IncompleteUInt And(IncompleteUInt value) => new()
        {
            Binary = Utils.Math.And(Binary, value.Binary)
        };

        public UInt Or(UInt value) => new()
        {
            Binary = Utils.Math.Or(Binary, value.Binary)
        };

        public IncompleteUInt Or(IncompleteUInt value) => new()
        {
            Binary = Utils.Math.Or(Binary, value.Binary)
        };

        public UInt Nand(UInt value) => new()
        {
            Binary = Utils.Math.Nand(Binary, value.Binary)
        };

        public IncompleteUInt Nand(IncompleteUInt value) => new()
        {
            Binary = Utils.Math.Nand(Binary, value.Binary)
        };

        public IncompleteUInt Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        public override string ToString() => string.Join(string.Empty, Bytes.Select(x => new Byte(){ Value =  x }.ToString()));

        public string ToString(string format) => Value.ToString(format);
    }
}