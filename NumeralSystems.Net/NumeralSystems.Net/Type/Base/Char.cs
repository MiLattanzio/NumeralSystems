using System;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Incomplete;
using NumeralSystems.Net.Utils;
using Math = NumeralSystems.Net.Utils.Math;


namespace NumeralSystems.Net.Type.Base
{
    public class Char : IRegularOperable<IncompleteChar, Char, char>
    {
        public static Char FromBinary(bool[] binary) => new ()
        {
            Value = binary.ToChar()
        };
        
        public virtual char Value { get; set; }

        public byte[] Bytes
        {
            get => BitConverter.GetBytes(Value);
            private set => Value = BitConverter.ToChar(value, 0);
        }

        public bool[] Binary
        {
            get => Value.ToBoolArray();
            private set => Value = value.ToChar();
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

        public bool ReverseAnd(IncompleteChar right, out IncompleteChar result)
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

        public bool ReverseOr(Char right, out IncompleteChar result)
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

        public bool ReverseOr(IncompleteChar right, out IncompleteChar result)
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
        
        public override string ToString() => string.Join(string.Empty, Binary.Reverse().Select(x => x ? "1" : "0"));

        public Char Not() => new ()
        {
            Binary = Binary.Not()
        };

        public Char Xor(Char value) => new()
        {
            Binary = Binary.Xor(value.Binary)
        };

        public IncompleteChar Xor(IncompleteChar value) => new()
        {
            Binary = Binary.Xor(value.Binary)
        };

        public Char And(Char value) => new()
        {
            Binary = Binary.And(value.Binary)
        };

        public IncompleteChar And(IncompleteChar value) => new()
        {
            Binary = Binary.And(value.Binary)
        };

        public Char Or(Char value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        public IncompleteChar Or(IncompleteChar value) => new()
        {
            Binary = Binary.Or(value.Binary)
        };

        public IncompleteChar Incomplete() => new()
        {
            Binary = Binary.Select(x => x as bool?).ToArray()
        };
    }
}