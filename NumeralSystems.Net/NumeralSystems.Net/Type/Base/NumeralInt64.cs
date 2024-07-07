using System;
using System.Linq;

namespace NumeralSystems.Net.Type.Base
{
    public class NumeralInt64: Int64
    {
        public NumeralInt64(Numeral numeral)
        {
            Numeral = numeral;
        }

        public NumeralInt64(long value, NumeralSystem system)
        {
            Numeral = system[value];
        }

        public NumeralInt64(long value, int size = 10): this(value, Numeral.System.OfBase(size))
        {
            
        }

        public NumeralInt64() : this(0)
        {

        }

        public NumeralInt64(Int64 value, int size): this(value.Value, size)
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