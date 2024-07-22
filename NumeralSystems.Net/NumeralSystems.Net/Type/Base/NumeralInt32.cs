using System;
using System.Linq;

namespace NumeralSystems.Net.Type.Base
{
    public class NumeralInt32: Int32
    {
        public NumeralInt32(Numeral numeral)
        {
            Numeral = numeral;
        }

        public NumeralInt32(int value, NumeralSystem system)
        {
            Numeral = system[value];
        }

        public NumeralInt32(int value, int size = 10): this(value, Numeral.System.OfBase(size))
        {
            
        }

        public NumeralInt32() : this(0)
        {

        }

        public NumeralInt32(Int32 value, int size): this(value.Value, size)
        {
            
        }

        public Numeral Numeral { get; set; }

        public override int Value
        {
            get => BitConverter.ToInt32(Numeral.Bytes, 0);
            set => Numeral.Bytes = BitConverter.GetBytes(value).ToArray();
        }
    }
}