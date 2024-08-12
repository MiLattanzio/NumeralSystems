using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Type.Base
{
    public partial class UInt: IRegularOperable<IncompleteUInt, UInt, uint, uint>
    {
        public virtual uint Value { get; set; }

        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value).ToArray();
            // ReSharper disable once UnusedMember.Local
            set => Value = value.Length >= sizeof(uint) ? BitConverter.ToUInt32(value.Take(sizeof(uint)).ToArray(), 0) : BitConverter.ToUInt32(value.Concat(System.Linq.Enumerable.Repeat((byte)0, sizeof(uint) - value.Length)).ToArray(), 0);
        }

        public int BitLength => sizeof(uint) * 8;

        public bool[] Binary
        {
            get => Utils.Convert.ToBoolArray(Value);
            set => Value = value.Length * 8 >= sizeof(uint) ? Utils.Convert.ToUInt(value.Take(sizeof(uint)*8).ToArray()) : Utils.Convert.ToUInt(value.Concat(System.Linq.Enumerable.Repeat(false, sizeof(uint)*8 - value.Length*8)).ToArray());
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