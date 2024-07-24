﻿using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = NumeralSystems.Net.Utils.Convert;

namespace NumeralSystems.Net.Type.Base
{
    public class Int: IRegularOperable<IncompleteInt, Int, int>
    {
        public static Int FromBinary(bool[] binary) => new ()
        {
            Value = Utils.Convert.ToInt(binary)
        };
        public virtual int Value { get; set; }

        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value).ToArray();
            // ReSharper disable once UnusedMember.Local
            private set => Value = BitConverter.ToInt32(value, 0);
        }

        public bool[] Binary
        {
            get => Convert.ToBoolArray(Value);
            private set => Value = Convert.ToInt(value);
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

        public bool ReverseAnd(Int right, out IncompleteInt result)
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

        public bool ReverseAnd(IncompleteInt right, out IncompleteInt result)
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

        public bool ReverseOr(Int right, out IncompleteInt result)
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

        public bool ReverseOr(IncompleteInt right, out IncompleteInt result)
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

        public Int Not() => new()
        {
            Binary = Math.Not(Binary)
        };

        public Int Xor(Int value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };

        public IncompleteInt Xor(IncompleteInt value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };

        public Int And(Int value) => new()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        public IncompleteInt And(IncompleteInt value) => new()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        public Int Or(Int value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        public IncompleteInt Or(IncompleteInt value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        public Int Nand(Int value) => new()
        {
            Binary = Math.Nand(Binary, value.Binary)
        };

        public IncompleteInt Nand(IncompleteInt value) => new()
        {
            Binary = Math.Nand(Binary, value.Binary)
        };

        public IncompleteInt Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };

        public override string ToString() => string.Join(string.Empty, Bytes.Select(x => new Byte(){ Value =  x }.ToString()));

        public string ToString(string format) => Value.ToString(format);

    }
}