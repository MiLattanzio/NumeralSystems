using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using Math = NumeralSystems.Net.Utils.Math;


namespace NumeralSystems.Net.Type.Base
{
    public class Char : IRegularOperable<IncompleteChar, Char, char>
    {
        public static Char FromBinary(bool[] binary) => new ()
        {
            Value = Utils.Convert.ToChar(binary)
        };
        
        public virtual char Value { get; set; }

        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value);
            set => Value = BitConverter.ToChar(value, 0);
        }

        public bool[] Binary
        {
            get => Utils.Convert.ToBoolArray(Value);
            set => Value = Utils.Convert.ToChar(value);
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

        public string ToString(string format)
        {
            try
            {
                return ((int)Value).ToString(format);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Value.ToString();
            }
        }


        public bool ReverseAnd(Char right, out IncompleteChar result)
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

        public bool ReverseAnd(IncompleteChar right, out IncompleteChar result)
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

        public bool ReverseOr(Char right, out IncompleteChar result)
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

        public bool ReverseOr(IncompleteChar right, out IncompleteChar result)
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
        
        public override string ToString() => string.Join(string.Empty, Binary.Reverse().Select(x => x ? "1" : "0"));
        
        public Char And(Char value) => new()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        public IncompleteChar And(IncompleteChar value) => new()
        {
            Binary = Math.And(Binary, value.Binary)
        };

        public Char Or(Char value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        public IncompleteChar Or(IncompleteChar value) => new()
        {
            Binary = Math.Or(Binary, value.Binary)
        };

        public IncompleteChar Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };
    }
}