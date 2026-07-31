using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace NumeralSystems.Net.Benchmarks
{
    [MemoryDiagnoser]
    public class NumeralValueArithmeticBenchmarks
    {
        private NumeralValue _binaryFraction;
        private NumeralValue _decimalFraction;
        private NumeralValue _largeHexadecimal;
        private NumeralValue _largeTernary;
        private NumeralValue _seven;

        [GlobalSetup]
        public void Setup()
        {
            _binaryFraction = NumeralValue.FromDigits(
                new[] { 1, 0 },
                new[] { 1, 0, 1 },
                false,
                2);
            _decimalFraction = NumeralValue.FromDecimal(3.75m);
            _seven = NumeralValue.FromInt(7);

            var magnitude = BigInteger.Pow(2, 512) + 1;
            _largeHexadecimal = NumeralValue.FromBigInteger(magnitude, 16);
            _largeTernary = NumeralValue.FromBigInteger(magnitude, 3);
        }

        [Benchmark(Baseline = true)]
        public NumeralValue AddSameBase() => _decimalFraction + _decimalFraction;

        [Benchmark]
        public NumeralValue AddAcrossBases() => _binaryFraction + _decimalFraction;

        [Benchmark]
        public NumeralValue DivideRepeating() => _decimalFraction / _seven;

        [Benchmark]
        public int CompareLargeCrossBaseValues() => _largeHexadecimal.CompareTo(_largeTernary);
    }
}
