using System;

namespace NumeralSystems.Net.Type.Base
{
    public class NumeralFloat : Float
    {
        public NumeralFloat(Numeral numeral)
        {
            Numeral = numeral;
        }

        public NumeralFloat(float value, NumeralSystem system)
        {
            Numeral = system[value];
        }

        public NumeralFloat(float value, int size = 10): this(value, Numeral.System.OfBase(size))
        {
            
        }

        public NumeralFloat() : this(0)
        {

        }

        public NumeralFloat(Float value, int size): this(value.Value, size)
        {
            
        }

        public Numeral Numeral { get; set; }

        public override float Value
        {
            get => BitConverter.ToSingle(Numeral.Bytes, 0);
            set => Numeral.Bytes = Utils.Convert.ToByteArray(value);
        }
    }
}