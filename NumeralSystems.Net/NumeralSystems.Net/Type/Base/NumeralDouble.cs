using System;

namespace NumeralSystems.Net.Type.Base
{
    public class NumeralDouble: Double
    {
        public NumeralDouble(Numeral numeral)
        {
            Numeral = numeral;
        }

        public NumeralDouble(double value, NumeralSystem system)
        {
            Numeral = system[value];
        }

        public NumeralDouble(double value, int size = 10): this(value, Numeral.System.OfBase(size))
        {
            
        }

        public NumeralDouble() : this(0)
        {

        }

        public NumeralDouble(Double value, int size): this(value.Value, size)
        {
            
        }

        public Numeral Numeral { get; set; }

        public override double Value
        {
            get => Numeral.Double;
            set => Numeral.Double = value;
        }
    }
}