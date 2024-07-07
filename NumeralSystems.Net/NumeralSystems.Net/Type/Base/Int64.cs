using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = NumeralSystems.Net.Utils.Convert;

namespace NumeralSystems.Net.Type.Base
{
    public class Int64 : IRegularOperable<IncompleteInt64, Int64, long>
    {
        public static Int64 FromBinary(bool[] binary) => new ()
        {
            Value = Utils.Convert.ToLong(binary)
        };
        public virtual long Value { get; set; }

        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value);
            set => Value = BitConverter.ToInt64(value, 0);
        }

        public bool[] Binary
        {
            get => Convert.ToBoolArray(Value);
            set => Value = Convert.ToLong(value);
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

        public bool ReverseAnd(Int64 right, out IncompleteInt64 result)
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

        public bool ReverseAnd(IncompleteInt64 right, out IncompleteInt64 result)
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

        public bool ReverseOr(Int64 right, out IncompleteInt64 result)
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

        public bool ReverseOr(IncompleteInt64 right, out IncompleteInt64 result)
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

        public Int64 Not() => new()
        {
            Binary = Math.Not(Binary)
        };

        public Int64 Xor(Int64 value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };

        public IncompleteInt64 Xor(IncompleteInt64 value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };

        public Int64 And(Int64 value) => new ()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        public IncompleteInt64 And(IncompleteInt64 value) => new ()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        public Int64 Or(Int64 value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        public IncompleteInt64 Or(IncompleteInt64 value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        public IncompleteInt64 Incomplete() => new ()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        public override string ToString() =>string.Join(string.Empty, Bytes.Select(x => (new Byte() {Value = x})));

        public string ToString(string format) => Value.ToString(format);
        
    }
}