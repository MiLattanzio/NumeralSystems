using System.Linq;


namespace NumeralSystems.Net.Type.Base
{
    public sealed class NumeralByte : Byte
    {

        public NumeralByte(Numeral numeral)
        {
            Numeral = numeral;
        }

        public NumeralByte(byte value, NumeralSystem system)
        {
            Numeral = system[value];
        }

        public NumeralByte(byte value, int size = 10): this(value, Numeral.System.OfBase(size))
        {
            
        }

        public NumeralByte() : this(0)
        {

        }

        public NumeralByte(Byte value, int size): this(value.Value, size)
        {
            
        }

        public Numeral Numeral { get; set; }

        public override byte Value
        {
            get => Numeral.Bytes.FirstOrDefault();
            set => Numeral.Bytes = new[] { value };
        }
        
    }
}