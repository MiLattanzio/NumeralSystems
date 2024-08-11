using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using NumeralSystems.Net.Utils;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = NumeralSystems.Net.Utils.Convert;


namespace NumeralSystems.Net.Type.Base
{
    public partial class Float : IRegularOperable<IncompleteFloat, Float, float, uint>
    {
        public static Float FromBinary(bool[] binary) => new ()
        {
            Value = binary.ToFloat()
        };
        public virtual float Value { get; set; }

        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value).ToArray();
            // ReSharper disable once UnusedMember.Local
            set => Value = value.Length >= sizeof(float) ? BitConverter.ToSingle(value.Take(sizeof(float)).ToArray(),0) : BitConverter.ToSingle(value.Concat(System.Linq.Enumerable.Repeat((byte)0, sizeof(float) - value.Length)).ToArray(), 0);
        }

        public int BitLength => sizeof(float) * 8;

        public bool[] Binary
        {
            get => Value.ToBoolArray();
            set => Value = value.Length * 8 >= sizeof(float) ? value.Take(sizeof(float)*8).ToArray().ToFloat() : value.Concat(System.Linq.Enumerable.Repeat(false, sizeof(float)*8 - value.Length*8)).ToArray().ToFloat();
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

        public bool ReverseAnd(Float right, out IncompleteFloat result)
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

        public bool ReverseAnd(IncompleteFloat right, out IncompleteFloat result)
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

        public bool ReverseOr(Float right, out IncompleteFloat result)
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

        public bool ReverseOr(IncompleteFloat right, out IncompleteFloat result)
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

        public Float Not() => new ()
        {
            Binary = Binary.Not()
        };

        public Float Xor(Float value) => new ()
        {
            Binary = Binary.Xor(value.Binary)
        };

        public IncompleteFloat Xor(IncompleteFloat value) => new ()
        {
            Binary = Binary.Xor(value.Binary)
        };
        
        public Float And(Float value) => new ()
        {
            Binary = Binary.And(value.Binary)
        };

        public IncompleteFloat And(IncompleteFloat value) => new ()
        {
            Binary = Binary.And(value.Binary)
        };

        public Float Or(Float value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        public IncompleteFloat Or(IncompleteFloat value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        public Float Nand(Float value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        public IncompleteFloat Nand(IncompleteFloat value) => new()
        {
            Binary = Binary.Nand(value.Binary)
        };

        public IncompleteFloat Incomplete() => new ()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        public override string ToString() => string.Join(string.Empty, Bytes.Select(x => (new Byte() { Value = x})));

        public string ToString(string format) => Value.ToString(format);
    }
}