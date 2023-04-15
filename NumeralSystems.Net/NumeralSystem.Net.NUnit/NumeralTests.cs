using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using NumeralSystems.Net;
using NumeralSystems.Net.Utils;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    [MemoryDiagnoser()]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn()]
    public class NumeralTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Benchmark]
        [Test]
        public void RandomAlphanumericTest()
        {
            var random = new Random();
            var r = random.Next(2, 2000);
            var difficulty = Numeral.System.Characters.Alphanumeric.Count();
            for (int i = 0; i < r; i++)
            {
                var r2 = random.Next(2, difficulty);
                var r3 = (decimal)(random.Next(2, int.MaxValue) + random.NextDouble());
                var numerals = Numeral.System.OfBase(r2, Convert.ToString(Numeral.System.Characters.Semicolon), Numeral.System.Characters.Alphanumeric.Select(x => x.ToString()));
                var numeral = numerals[r3];
                Assert.AreEqual(r3, numeral.Decimal);
                Assert.AreEqual(numerals.Parse(numeral.ToString()).ToString(), numeral.ToString());
                var r3Bytes = decimal.GetBits(r3).SelectMany(BitConverter.GetBytes).ToArray();
                Assert.AreEqual(numeral.Bytes, r3Bytes);
                numeral.Bytes = r3Bytes;
                Assert.AreEqual(r3, numeral.Decimal);
            }
        }
        
        

        [Benchmark]
        [Test]
        public void Base10Test()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var random = new Random();
            for (var i = 0; i < 10; i++)
            {
                var value = random.Next();
                var base10 = Numeral.System.OfBase(10, string.Empty);
                var decimalValue = base10[value];
                Console.WriteLine($"Generated {decimalValue} should be equal to {value.ToString()}");
                Assert.AreEqual(decimalValue.ToString(), value.ToString());
            }
        }

        [Benchmark]
        [Test]
        public void BinaryTest()
        {
            var random = new Random();
            for (var i = 0; i < 10; i++)
            {
                var value = random.Next();
                value = random.NextDouble() < 0.5 ? value : -value;
                var base2 = Numeral.System.OfBase(2, string.Empty);
                var binaryValue = base2[value];
                var expected = $"{(value > 0 ? "" : "-" )}{Convert.ToString(Math.Abs(value), 2)}";
                Console.WriteLine($"Generated {binaryValue} should be equal to {expected}");
                Assert.AreEqual(binaryValue.ToString(), expected);
            }
        }
        
        [Benchmark]
        [Test]
        public void BinaryParseTest()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var random = new Random();
            var base2 = Numeral.System.OfBase(2, string.Empty);
            for (var i = 0; i < 10; i++)
            {
                var value = random.Next();
                value = random.NextDouble() < 0.5 ? value : -value;
                var binaryValue = base2[value];
                var Parse = base2.Parse(binaryValue.ToString());
                Assert.AreEqual(binaryValue.ToString(), Parse.ToString());
            }
        }

        [Benchmark]
        [Test]
        public void DoubleTest()
        {
            var random = new Random();
            for (var i = 0; i < 20; i++)
            {
                var value = random.NextDouble();
                var base10 = Numeral.System.OfBase(10, string.Empty);
                var decimalValue = base10[value];
                Console.WriteLine($"Generated {decimalValue} should be equal to {value.ToString(base10.CultureInfo)}");
                Assert.AreEqual(decimalValue.Double, value);
                var decimalValue2 = base10.Parse(decimalValue.ToString());
                Assert.AreEqual(decimalValue2.Double, decimalValue.Double);
            }
            
        }

        [Benchmark]
        [Test]
        public void DecimalTest()
        {
            var random = new Random();
            for (var i = 0; i < 20; i++)
            {
                var value = (decimal)random.NextDouble();
                var base10 = Numeral.System.OfBase(10, string.Empty);
                var decimalValue = base10[value];
                Console.WriteLine($"Generated {decimalValue} should be equal to {value.ToString(base10.CultureInfo)}");
                Assert.AreEqual(decimalValue.Decimal, value);
                var decimalValue2 = base10.Parse(decimalValue.ToString());
                Assert.AreEqual(decimalValue2.Decimal, decimalValue.Decimal);
            }
            
        }
    }
}