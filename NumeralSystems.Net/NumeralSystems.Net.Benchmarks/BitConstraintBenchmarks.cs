using BenchmarkDotNet.Attributes;
using NumeralSystems.Net.Type.Incomplete;

namespace NumeralSystems.Net.Benchmarks
{
    [MemoryDiagnoser]
    public class BitConstraintBenchmarks
    {
        private const string SingleExpression = "x & 10101010 = 10001000";
        private BitConstraintSet _single;
        private BitConstraintSet _composed;

        [GlobalSetup]
        public void Setup()
        {
            _single = BitConstraintSet.Parse(SingleExpression);
            _composed = BitConstraintSet.Parse(
                "x & 10101010 = 10001000; " +
                "x | 00001111 = 10001111; " +
                "x ^ 00000000 = ????????");
        }

        [Benchmark]
        public BitConstraint ParseSingle() => BitConstraint.Parse(SingleExpression);

        [Benchmark(Baseline = true)]
        public BitConstraintSolution SolveSingle() => _single.Solve();

        [Benchmark]
        public BitConstraintSolution SolveComposed() => _composed.Solve();
    }
}
