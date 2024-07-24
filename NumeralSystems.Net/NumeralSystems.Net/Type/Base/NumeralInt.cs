using System;
using System.Linq;

namespace NumeralSystems.Net.Type.Base
{
    public class NumeralInt: Int
    {
        public NumeralInt(Numeral numeral)
        {
            Numeral = numeral;
        }

        public NumeralInt(int value, NumeralSystem system)
        {
            Numeral = system[value];
        }

        public NumeralInt(int value, int size = 10): this(value, Numeral.System.OfBase(size))
        {
            
        }

        public NumeralInt() : this(0)
        {

        }

        public NumeralInt(Int value, int size): this(value.Value, size)
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