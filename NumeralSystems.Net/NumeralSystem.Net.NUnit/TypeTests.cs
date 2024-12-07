using System;
using System.Globalization;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using NumeralSystems.Net;
using NumeralSystems.Net.Type.Base;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    [MemoryDiagnoser()]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn()]
    public class TypeTests
    {
        [SetUp]
        public void Setup()
        {
            
        }
        
        [Test]
        public void Random()
        {
            
            var random = new Random();
            var base2 = Numeral.System.OfBase(10);
            var serializationInfo = NumeralSystems.Net.NumeralSystem.SerializationInfo.OfBase(10);
            for (var i = 0; i < 20; i++)
            {
                var value = (decimal)random.NextDouble();
                var decimalValue = base2[value];
                Console.WriteLine($"Generated {decimalValue} should be equal to {value.ToString(CultureInfo.CurrentCulture)}");
                Assert.AreEqual(decimalValue.Decimal, value);
                var decimalValue2 = base2.Parse(decimalValue.ToString(), serializationInfo);
                Assert.AreEqual(decimalValue2.Decimal, decimalValue.Decimal);
            }
            
        }


        [Test]
        public void Sizeof()
        {
            Console.WriteLine($"bool sizeof {sizeof(bool)}");
            Console.WriteLine($"byte sizeof {sizeof(byte)}");
            Console.WriteLine($"sbyte sizeof {sizeof(sbyte)}");
            Console.WriteLine($"char sizeof {sizeof(char)}");
            Console.WriteLine($"short sizeof {sizeof(short)}");
            Console.WriteLine($"ushort sizeof {sizeof(ushort)}");
            Console.WriteLine($"int sizeof {sizeof(int)}");
            Console.WriteLine($"uint sizeof {sizeof(uint)}");
            Console.WriteLine($"long sizeof {sizeof(long)}");
            Console.WriteLine($"ulong sizeof {sizeof(ulong)}");
            Console.WriteLine($"float sizeof {sizeof(float)}");
            Console.WriteLine($"double sizeof {sizeof(double)}");
            Console.WriteLine($"decimal sizeof {sizeof(decimal)}");
        }
    }
}