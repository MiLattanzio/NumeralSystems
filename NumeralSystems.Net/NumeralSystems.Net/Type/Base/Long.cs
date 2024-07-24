using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = NumeralSystems.Net.Utils.Convert;

namespace NumeralSystems.Net.Type.Base
{
    public class Long : IRegularOperable<IncompleteLong, Long, long>
    {
        public static Long FromBinary(bool[] binary) => new ()
        {
            Value = Utils.Convert.ToLong(binary)
        };
        public virtual long Value { get; set; }

        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value);
            // ReSharper disable once UnusedMember.Local
            private set => Value = BitConverter.ToInt64(value, 0);
        }

        public bool[] Binary
        {
            get => Convert.ToBoolArray(Value);
            private set => Value = Convert.ToLong(value);
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

        public bool ReverseAnd(Long right, out IncompleteLong result)
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

        public bool ReverseAnd(IncompleteLong right, out IncompleteLong result)
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

        public bool ReverseOr(Long right, out IncompleteLong result)
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

        public bool ReverseOr(IncompleteLong right, out IncompleteLong result)
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

        public Long Not() => new()
        {
            Binary = Math.Not(Binary)
        };

        public Long Xor(Long value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };

        public IncompleteLong Xor(IncompleteLong value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };

        public Long And(Long value) => new ()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        public IncompleteLong And(IncompleteLong value) => new ()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        public Long Or(Long value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        public IncompleteLong Or(IncompleteLong value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        public Long Nand(Long value) => new()
        {
            Binary = Math.Nand(Binary, value.Binary)
        };

        public IncompleteLong Nand(IncompleteLong value) => new()
        {
            Binary = Math.Nand(Binary, value.Binary)
        };

        public IncompleteLong Incomplete() => new ()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        public override string ToString() =>string.Join(string.Empty, Bytes.Select(x => (new Byte() {Value = x})));

        public string ToString(string format) => Value.ToString(format);
        
    }
}