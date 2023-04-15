using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;

namespace NumeralSystems.Net.Benchmark
{
    class Program
    {
        public static void Main(string[] args)
        {
#if RELEASE
            var summary = BenchmarkRunner.Run<Tests>();
#else
            //var summary = BenchmarkRunner.Run<Tests>();
            
              var test = new Tests();
              var bases = new int[] { 10, 2, 16, 256, 1028, 4096 };
              var sw = new System.Diagnostics.Stopwatch();
              foreach (var b in bases)
              {
                  
                  test.Base = b;
                  Console.WriteLine($"{b} {sw.ElapsedMilliseconds}");
                  for (var i = 0; i < 100; i++)
                  {
                      sw.Restart();
                      test.Test();
                      sw.Stop();
                      Console.WriteLine($"Base {b} test took {sw.ElapsedMilliseconds} ms");
                  }
              }
#endif
              
        }
        
    }
    
    [MemoryDiagnoser()]
    public class Tests
    {
        
        [Params(10, 2, 16, 256, 1028, 4096)]
        public int Base { get; set; }
        
        private NumeralSystem _numeralSystem;

        public NumeralSystem NumeralSystem
        {
            get
            {
                if (null == _numeralSystem || _numeralSystem.Size != Base)
                {
                    Console.WriteLine($"{_numeralSystem?.Size ?? 0} != {Base}");
                    _numeralSystem = Numeral.System.OfBase(Base, ";");
                }
                return _numeralSystem;
            }
        }
        
        [GlobalSetup]
        public void Setup()
        {
            if (null == _numeralSystem || _numeralSystem.Size != Base)
            {
                Console.WriteLine($"{_numeralSystem?.Size ?? 0} != {Base}");
                _numeralSystem = Numeral.System.OfBase(Base, ";");
            }
        }

        [Benchmark]
        public void Test()
        {
            var random = new Random();
            var r3 = (decimal)(random.Next(2, int.MaxValue) + random.NextDouble());
            r3 = random.NextDouble() < 0.5 ? r3 : -r3;
            var numeral = NumeralSystem[r3];
            try
            {
                Console.WriteLine($"Generated {numeral} should be equal to {r3}");
                Assert.AreEqual(r3, numeral.Decimal);
                var stringParse = NumeralSystem.Parse(numeral.ToString()); 
                Assert.AreEqual(stringParse.ToString(), numeral.ToString());
                //numeral.Decimal = r3;
                Assert.AreEqual(r3, numeral.Decimal);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        

            
        }
        
        public static class Assert
        {
            public static void AreEqual(double expected, double actual)
            {
                if (Math.Abs(expected - actual) > 1e-10)
                {
                    throw new Exception($"Expected {expected} but was {actual}");
                }
            }
            
            public static void AreEqual(decimal expected, decimal actual)
            {
                if (Math.Abs(expected - actual) > 1e-10m)
                {
                    throw new Exception($"Expected {expected} but was {actual}");
                }
            }
            
            public static void AreEqual(int expected, int actual)
            {
                if (expected != actual)
                {
                    throw new Exception($"Expected {expected} but was {actual}");
                }
            }
            
            public static void AreEqual(long expected, long actual)
            {
                if (expected != actual)
                {
                    throw new Exception($"Expected {expected} but was {actual}");
                }
            }
            
            public static void AreEqual(ulong expected, ulong actual)
            {
                if (expected != actual)
                {
                    throw new Exception($"Expected {expected} but was {actual}");
                }
            }
            
            public static void AreEqual(uint expected, uint actual)
            {
                if (expected != actual)
                {
                    throw new Exception($"Expected {expected} but was {actual}");
                }
            }
            
            public static void AreEqual(ushort expected, ushort actual)
            {
                if (expected != actual)
                {
                    throw new Exception($"Expected {expected} but was {actual}");
                }
            }
            
            public static void AreEqual(short expected, short actual)
            {
                if (expected != actual)
                {
                    throw new Exception($"Expected {expected} but was {actual}");
                }
            }
            
            public static void AreEqual(sbyte expected, sbyte actual)
            {
                if (expected != actual)
                {
                    throw new Exception($"Expected {expected} but was {actual}");
                }
            }
            
            public static void AreEqual(byte expected, byte actual)
            {
                if (expected != actual)
                {
                    throw new Exception($"Expected {expected} but was {actual}");
                }
            }
            
            public static void AreEqual(bool expected, bool actual)
            {
                if (expected != actual)
                {
                    throw new Exception($"Expected {expected} but was {actual}");
                }
            }
            
            public static void AreEqual(char expected, char actual)
            {
                if (expected != actual)
                {
                    throw new Exception($"Expected {expected} but was {actual}");
                }
            }
            
            public static void AreEqual(string expected, string actual)
            {
                if (expected != actual)
                {
                    throw new Exception($"Expected {expected} but was {actual}");
                }
            }
        }
    }
