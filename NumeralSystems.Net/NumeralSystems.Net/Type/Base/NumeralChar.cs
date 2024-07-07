using System.Linq;
using NumeralSystems.Net.Type.Incomplete;
using NumeralSystems.Net.Utils;

namespace NumeralSystems.Net.Type.Base
{
    public class NumeralChar: Char
    {
        
        public NumeralChar(Numeral numeral)
        {
            Numeral = numeral;
        }

        public NumeralChar(char value, NumeralSystem system)
        {
            Numeral = system[value];
        }

        public NumeralChar(char value, int size = 10): this(value, Numeral.System.OfBase(size))
        {
            
        }

        public NumeralChar() : this((char)0)
        {

        }

        public NumeralChar(Char value, int size): this(value.Value, size)
        {
            
        }

        public Numeral Numeral { get; set; }

        public override char Value
        {
            get => Numeral.Integer.ToBoolArray().ToChar();
            set => Numeral.Bytes = value.ToByteArray();
        }
    }
}