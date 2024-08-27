using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using Math = NumeralSystems.Net.Utils.Math;


namespace NumeralSystems.Net.Type.Base
{
    public sealed partial class Double : IRegularOperable<IncompleteDouble, Double, double, ulong>
    {
        public static Double FromBinary(bool[] binary) => new ()
        {
            Value = Utils.Convert.ToDouble(binary)
        };
        
        public double Value { get; set; }

        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value).ToArray();
            // ReSharper disable once UnusedMember.Local
            set => Value = value.Length >= sizeof(double) ? BitConverter.ToDouble(value, 0) : BitConverter.ToDouble(value.Concat(System.Linq.Enumerable.Repeat((byte)0, sizeof(double) - value.Length)).ToArray(), 0);
        }

        public int BitLength => sizeof(double) * 8;

        public bool[] Binary
        {
            get => Utils.Convert.ToBoolArray(Value);
            set => Value = value.Length * 8 >= sizeof(double) ? Utils.Convert.ToDouble(value) : Utils.Convert.ToDouble(value.Concat(System.Linq.Enumerable.Repeat(false, sizeof(double)*8 - value.Length*8)).ToArray());
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

        public bool ReverseAnd(Double right, out IncompleteDouble result)
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

        public bool ReverseAnd(IncompleteDouble right, out IncompleteDouble result)
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

        public bool ReverseOr(Double right, out IncompleteDouble result)
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

        public bool ReverseOr(IncompleteDouble right, out IncompleteDouble result)
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

        public Double Not() => new()
        {
            Binary = Math.Not(Binary)
        };

        public Double Xor(Double value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };

        public IncompleteDouble Xor(IncompleteDouble value) => new()
        {
            Binary = Math.Xor(Binary, value.Binary)
        };
        
        public Double And(Double value) => new()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        public IncompleteDouble And(IncompleteDouble value) => new()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        public Double Or(Double value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        public IncompleteDouble Or(IncompleteDouble value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        public Double Nand(Double value) => new()
        {
            Binary = Math.Nand(Binary, value.Binary)
        };

        public IncompleteDouble Nand(IncompleteDouble value) => new()
        {
            Binary = Math.Nand(Binary, value.Binary)
        };

        public IncompleteDouble Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };
        public override string ToString() => string.Join(string.Empty, Bytes.Select(x => new Byte(){ Value = x}.ToString()));
        
        public string ToString(string format) =>  Value.ToString(format);
    }
}