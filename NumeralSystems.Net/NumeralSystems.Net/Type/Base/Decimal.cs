﻿using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = Polecola.Primitive.Convert;

namespace NumeralSystems.Net.Type.Base
{
    public partial class Decimal: IRegularOperable<IncompleteDecimal, Decimal, decimal, ulong>
    {
        public static Decimal FromBinary(bool[] binary) => new ()
        {
            Value = Convert.ToDecimal(binary)
        };
        
        public virtual decimal Value { get; set; }
        
        public byte[] Bytes
        {
            get => Convert.ToByteArray(Value);
            // ReSharper disable once UnusedMember.Local
            set => Value = value.Length >= sizeof(decimal) ? Convert.ToDecimal(value) : Convert.ToDecimal(value.Concat(System.Linq.Enumerable.Repeat((byte)0, sizeof(decimal) - value.Length)).ToArray());
        }

        public int BitLength => sizeof(decimal) * 8;

        public bool[] Binary
        {
            get => Convert.ToBoolArray(Value);
            set => Value = value.Length * 8 >= sizeof(decimal) ?  Convert.ToDecimal(value) : Convert.ToDecimal(value.Concat(System.Linq.Enumerable.Repeat(false, sizeof(decimal)*8 - value.Length*8)).ToArray());
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