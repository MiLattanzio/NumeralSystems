using System;
using System.Linq;

namespace NumeralSystems.Net.Type.Base
{
    public class NumeralUInt : UInt
    {
        public NumeralUInt(Numeral numeral)
        {
            Numeral = numeral;
        }

        public NumeralUInt(uint value, NumeralSystem system)
        {
            Numeral = system[value];
        }

        public NumeralUInt(uint value, int size = 10): this(value, Numeral.System.OfBase(size))
        {
            
        }

        public NumeralUInt() : this(0)
        {

        }

        public NumeralUInt(UInt value, int size): this(value.Value, size)
        {
            
        }

        public Numeral Numeral { get; set; }

        public override uint Value
        {
            get => BitConverter.ToUInt32(Numeral.Bytes, 0);
            set => Numeral.Bytes = BitConverter.GetBytes(value).ToArray();
        }
    }
}