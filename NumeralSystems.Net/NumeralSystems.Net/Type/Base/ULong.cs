using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Type.Base
{
    public partial class ULong: IRegularOperable<IncompleteULong, ULong, ulong>
    {
         public static ULong FromBinary(bool[] binary) => new ()
        {
            Value = Utils.Convert.ToULong(binary)
        };
        public virtual ulong Value { get; set; }

        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value).ToArray();
            // ReSharper disable once UnusedMember.Local
            private set => Value = BitConverter.ToUInt64(value, 0);
        }

        public bool[] Binary
        {
            get => Utils.Convert.ToBoolArray(Value);
            private set => Value = Utils.Convert.ToULong(value);
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

        public bool ReverseAnd(ULong right, out IncompleteULong result)
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

        public bool ReverseAnd(IncompleteULong right, out IncompleteULong result)
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

        public bool ReverseOr(ULong right, out IncompleteULong result)
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

        public bool ReverseOr(IncompleteULong right, out IncompleteULong result)
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

        public ULong Not() => new()
        {
            Binary = Utils.Math.Not(Binary)
        };

        public ULong Xor(ULong value) => new()
        {
            Binary = Utils.Math.Xor(Binary, value.Binary)
        };

        public IncompleteULong Xor(IncompleteULong value) => new()
        {
            Binary = Utils.Math.Xor(Binary, value.Binary)
        };

        public ULong And(ULong value) => new()
        {
            Binary = Utils.Math.And(Binary, value.Binary)
        };

        public IncompleteULong And(IncompleteULong value) => new()
        {
            Binary = Utils.Math.And(Binary, value.Binary)
        };

        public ULong Or(ULong value) => new()
        {
            Binary = Utils.Math.Or(Binary, value.Binary)
        };

        public IncompleteULong Or(IncompleteULong value) => new()
        {
            Binary = Utils.Math.Or(Binary, value.Binary)
        };

        public ULong Nand(ULong value) => new()
        {
            Binary = Utils.Math.Nand(Binary, value.Binary)
        };

        public IncompleteULong Nand(IncompleteULong value) => new()
        {
            Binary = Utils.Math.Nand(Binary, value.Binary)
        };

        public IncompleteULong Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        public override string ToString() => string.Join(string.Empty, Bytes.Select(x => new Byte(){ Value =  x }.ToString()));

        public string ToString(string format) => Value.ToString(format);
    }
}