using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = NumeralSystems.Net.Utils.Convert;

namespace NumeralSystems.Net.Type.Base
{
    public class Int32: IRegularOperable<IncompleteInt32, Int32, int>
    {
        public static Int32 FromBinary(bool[] binary) => new ()
        {
            Value = Utils.Convert.ToInt(binary)
        };
        public virtual int Value { get; set; }

        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value).ToArray();
            set => Value = BitConverter.ToInt32(value, 0);
        }

        public bool[] Binary
        {
            get => Convert.ToBoolArray(Value);
            set => Value = Convert.ToInt(value);
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

        public bool ReverseAnd(Int32 right, out IncompleteInt32 result)
        {
            if (Math.CanReverseAnd(Binary, right.Binary))
            {
                result = new ()
                {
                    Binary = Math.ReverseAnd(Binary, right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        public bool ReverseAnd(IncompleteInt32 right, out IncompleteInt32 result)
        {
            if (Math.CanReverseAnd(Binary, right.Binary))
            {
                result = new ()
                {
                    Binary = Math.ReverseAnd(Binary, right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        public bool ReverseOr(Int32 right, out IncompleteInt32 result)
        {
            if (Math.CanReverseOr(Binary, right.Binary))
            {
                result = new ()
                {
                    Binary = Math.ReverseOr(Binary, right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        public bool ReverseOr(IncompleteInt32 right, out IncompleteInt32 result)
        {
            if (Math.CanReverseOr(Binary, right.Binary))
            {
                result = new ()
                {
                    Binary = Math.ReverseOr(Binary, right.Binary)
                };
                return true;
            }
            result = null;
            return false;
        }

        public Int32 Not() => new()
        {
            Binary = Math.Not(Binary)
        };

        public Int32 Xor(Int32 value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };

        public IncompleteInt32 Xor(IncompleteInt32 value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };

        public Int32 And(Int32 value) => new()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        public IncompleteInt32 And(IncompleteInt32 value) => new()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        public Int32 Or(Int32 value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        public IncompleteInt32 Or(IncompleteInt32 value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        public IncompleteInt32 Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        public override string ToString() => string.Join(string.Empty, Bytes.Select(x => new Byte(){ Value =  x }.ToString()));

        public string ToString(string format) => Value.ToString(format);

    }
}