using BenchmarkDotNet.Attributes;

namespace NumeralSystems.Net.Benchmarks
{
    [MemoryDiagnoser]
    public class NumeralBenchmarks
    {
        private const int Value = 123456789;
        private NumeralSystem _binary;
        private NumeralSystem _decimal;
        private NumeralSystem _hexadecimal;
        private string _binaryText;

        [GlobalSetup]
        public void Setup()
        {
            _binary = Numeral.System.OfBase(2);
            _decimal = Numeral.System.OfBase(10);
            _hexadecimal = Numeral.System.OfBase(16);
            _binaryText = _binary[Value].ToString();
        }

        [Benchmark(Baseline = true)]
        public string FormatDecimal() => _decimal[Value].ToString();

        [Benchmark]
        public string FormatBinary() => _binary[Value].ToString();

        [Benchmark]
        public Numeral ParseBinary() => _binary.Parse(_binaryText);

        [Benchmark]
        public Numeral ConvertHexadecimalToBinary() => _hexadecimal[Value].To(_binary);
    }
}
