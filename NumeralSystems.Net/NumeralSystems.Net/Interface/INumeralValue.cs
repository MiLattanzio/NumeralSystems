
using NumeralSystems.Net.Utils;

namespace NumeralSystems.Net.Interface
{
    public interface INumeralValue<TValue>
    {
        public TValue Value { get; set; }
        public byte[] Bytes { get; }
        public bool[] Binary { get; }
        public int BitLength { get; }
        public string ToString(string format);
        public bool this[int index]
        {
            get;
            set;
        }
    }
    
}