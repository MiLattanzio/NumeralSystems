namespace NumeralSystems.Net.Type.Base
{
    public class NumeralDecimal : Decimal
    {
        public NumeralDecimal(Numeral numeral)
        {
            Numeral = numeral;
        }

        public NumeralDecimal(decimal value, NumeralSystem system)
        {
            Numeral = system[value];
        }

        public NumeralDecimal(decimal value, int size = 10): this(value, Numeral.System.OfBase(size))
        {
            
        }

        public NumeralDecimal() : this(0)
        {

        }

        public NumeralDecimal(Decimal value, int size): this(value.Value, size)
        {
            
        }

        public Numeral Numeral { get; set; }

        public override decimal Value
        {
            get => Numeral.Decimal;
            set => Numeral.Decimal = value;
        }
    }
}