using System;
using System.Collections.Generic;
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
        
        [Test]
        public void ParseTest()
        {
            var base10 = Numeral.System.OfBase(10, ";");
            Assert.False(base10.TryParse(null, out var zero));
            Assert.AreEqual(0, zero.Decimal);
            Assert.False(base10.TryParse(string.Empty, out zero));
            Assert.AreEqual(0, zero.Decimal);
            Assert.True(base10.TryParse("0.0", out zero));
            Assert.AreEqual(0, zero.Decimal);
            var zeroBytes = zero.Bytes;
            Assert.AreEqual(zeroBytes, Enumerable.Repeat((byte)0, zeroBytes.Length));
            Assert.True(base10.TryParse("-1", out var minusOne));
            Assert.AreEqual(-1, minusOne.Decimal);
            Assert.Throws(typeof(InvalidOperationException), () => base10.Parse("a"));
            Assert.Throws(typeof(InvalidOperationException), () => base10.Parse("1.a"));
            Assert.Throws(typeof(Exception), () => new Numeral(base10, new List<int>(){-1}));
        }

        [Test]
        public void IndexTest()
        {
            var base10 = Numeral.System.OfBase(10, string.Empty);
            var tmp = base10[0];
            Assert.AreEqual(0, tmp.Decimal);
            tmp = base10[0.0];
            Assert.AreEqual(0, tmp.Decimal);
            var zeroBytes = tmp.Bytes;
            Assert.AreEqual(zeroBytes, Enumerable.Repeat((byte)0, zeroBytes.Length));
            var zeroLong = base10[0L];
            Assert.AreEqual(0, zeroLong.Decimal);
            var zeroULong = base10[0UL];
            Assert.AreEqual(0, zeroULong.Decimal);
            var zeroUInt = base10[0U];
            Assert.AreEqual(0, zeroUInt.Decimal);
            var zeroUShort = base10[(ushort)0];
            Assert.AreEqual(0, zeroUShort.Decimal);
            var zeroSByte = base10[(sbyte)0];
            Assert.AreEqual(0, zeroSByte.Decimal);
            var zeroByte = base10[(byte)0];
            Assert.AreEqual(0, zeroByte.Decimal);
            var zeroShort = base10[(short)0];
            Assert.AreEqual(0, zeroShort.Decimal);
            
            tmp = base10[100000.00000000500];
            Assert.AreEqual(100000.00000000500, tmp.Decimal);
            tmp = base10[100000.00000000500d];
            Assert.AreEqual(100000.00000000500d, tmp.Decimal);
        }

        [Test]
        public void CultureInfoTest()
        {
            var random = new Random();
            var value = random.Next();
            var base10 = Numeral.System.OfBase(10, string.Empty);
            base10.CultureInfo = null;
            var decimalValue = base10[value];
            Console.WriteLine($"Generated {decimalValue} should be equal to {value.ToString()}");
            Assert.AreEqual(decimalValue.ToString(), value.ToString());
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