using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = NumeralSystems.Net.Utils.Convert;


namespace NumeralSystems.Net.Type.Base
{
    public class Float : IRegularOperable<IncompleteFloat, Float, float>
    {
        public static Float FromBinary(bool[] binary) => new ()
        {
            Value = Utils.Convert.ToFloat(binary)
        };
        public virtual float Value { get; set; }

        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value).ToArray();
            set => Value = BitConverter.ToSingle(value, 0);
        }

        public bool[] Binary
        {
            get => Convert.ToBoolArray(Value);
            set => Value = Convert.ToFloat(value);
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

        public bool ReverseAnd(IncompleteFloat right, out IncompleteFloat result)
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

        public bool ReverseOr(Float right, out IncompleteFloat result)
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

        public bool ReverseOr(IncompleteFloat right, out IncompleteFloat result)
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

        public Float And(Float value) => new ()
        {
            Binary = Utils.Math.And(Binary, value.Binary)
        };

        public IncompleteFloat And(IncompleteFloat value) => new ()
        {
            Binary = Utils.Math.And(Binary, value.Binary)
        };

        public Float Or(Float value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        public IncompleteFloat Or(IncompleteFloat value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };
        public IncompleteFloat Incomplete() => new ()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        public override string ToString() => string.Join(string.Empty, Bytes.Select(x => (new Byte() { Value = x})));

        public string ToString(string format) => Value.ToString(format);
    }
}