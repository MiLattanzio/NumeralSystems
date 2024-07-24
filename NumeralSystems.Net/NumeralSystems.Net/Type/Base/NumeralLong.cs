using System;
using System.Linq;

namespace NumeralSystems.Net.Type.Base
{
    public class NumeralLong: Long
    {
        public NumeralLong(Numeral numeral)
        {
            Numeral = numeral;
        }

        public NumeralLong(long value, NumeralSystem system)
        {
            Numeral = system[value];
        }

        public NumeralLong(long value, int size = 10): this(value, Numeral.System.OfBase(size))
        {
            
        }

        public NumeralLong() : this(0)
        {

        }

        public NumeralLong(Long value, int size): this(value.Value, size)
        {
            
        }

        public Numeral Numeral { get; set; }

        public override long Value
        {
            get => BitConverter.ToInt64(Numeral.Bytes, 0);
            set => Numeral.Bytes = BitConverter.GetBytes(value).ToArray();
        }
    }
}