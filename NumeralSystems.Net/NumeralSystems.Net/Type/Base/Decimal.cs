using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using Math = NumeralSystems.Net.Utils.Math;

namespace NumeralSystems.Net.Type.Base
{
    public class Decimal: IRegularOperable<IncompleteDecimal, Decimal, decimal>
    {
        public static Decimal FromBinary(bool[] binary) => new ()
        {
            Value = Utils.Convert.ToDecimal(binary)
        };
        
        public virtual decimal Value { get; set; }
        
        public byte[] Bytes
        {
            get => Utils.Convert.ToByteArray(Value);
            // ReSharper disable once UnusedMember.Local
            private set => Value = Utils.Convert.ToDecimal(value);
        }

        public bool[] Binary
        {
            get => Utils.Convert.ToBoolArray(Value);
            private set => Value = Convert.ToDecimal(value);
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

        public bool ReverseAnd(Decimal right, out IncompleteDecimal result)
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

        public bool ReverseAnd(IncompleteDecimal right, out IncompleteDecimal result)
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

        public bool ReverseOr(Decimal right, out IncompleteDecimal result)
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

        public bool ReverseOr(IncompleteDecimal right, out IncompleteDecimal result)
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

        public Decimal Not() => new()
        {
            Binary = Math.Not(Binary)
        };

        public Decimal Xor(Decimal value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };

        public IncompleteDecimal Xor(IncompleteDecimal value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };
        
        public Decimal And(Decimal value) => new()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        public IncompleteDecimal And(IncompleteDecimal value) => new()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        public Decimal Or(Decimal value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        public IncompleteDecimal Or(IncompleteDecimal value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        public Decimal Nand(Decimal value) => new()
        {
            Binary = Math.Nand(Binary, value.Binary)
        };

        public IncompleteDecimal Nand(IncompleteDecimal value) => new()
        {
            Binary = Math.Nand(Binary, value.Binary)
        };

        public IncompleteDecimal Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        public override string ToString() => string.Join(string.Empty, Bytes.Select(x => new Byte(){ Value = x}.ToString()));

        public string ToString(string format) => Value.ToString(format);
    }
}