using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using NumeralSystems.Net;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;
using NUnit.Framework;
using Convert = System.Convert;
using Math = System.Math;

namespace NumeralSystem.Net.NUnit
{
    [MemoryDiagnoser()]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn()]
    public class NumeralTests
    {
        private Random _random;
        private NumeralSystems.Net.NumeralSystem _base2;
        private NumeralSystems.Net.NumeralSystem _base10;
        
        
        [SetUp]
        public void Setup()
        {
            _random = new Random();
            _base2 = Numeral.System.OfBase(2);
            _base10 = Numeral.System.OfBase(10);
        }

        [Test]
        public void IntegerCoherenceTest()
        {
            //Pick a random integer
            var randomInt = _random.Next();
            //Create a Numeral from it
            var randomNumeral = _base10[randomInt];
            //Check that the Numeral is equal to the integer
            Assert.AreEqual(randomInt, randomNumeral.Integer);
            //Check that the Numeral is equal to the integer after conversion
            randomNumeral.Integer = randomInt;
            Assert.AreEqual(randomInt, randomNumeral.Integer);
            //Create a Int32 from randomInt
            var randomInt32 = new Int()
            {
                Value = randomInt
            };
            //Check that the Int32 is equal to the integer
            Assert.AreEqual(randomInt, randomInt32.Value);
            //Check that the Int32 is equal to the integer after conversion
            randomInt32.Value = randomInt;
            Assert.AreEqual(randomInt, randomInt32.Value);
        }
        
        [Test]
        public void ParseTest()
        {
            var identity = Enumerable.Range(0, 9).Select(x => x.ToString()).ToList();
            var separator = ",";
            var negativeSign = "-";
            var numberDecimalSeparator = ".";
            Assert.False(_base10.TryParse(null, identity, separator, negativeSign, numberDecimalSeparator, out var zero));
            Assert.AreEqual(0, zero.Decimal);
            Assert.False(_base10.TryParse(string.Empty, identity, separator, negativeSign, numberDecimalSeparator, out zero));
            Assert.AreEqual(0, zero.Decimal);
            Assert.True(_base10.TryParse("0.0", identity, separator, negativeSign, numberDecimalSeparator, out zero));
            Assert.AreEqual(0, zero.Decimal);
            var zeroBytes = zero.Bytes;
            Assert.AreEqual(zeroBytes, Enumerable.Repeat((byte)0, zeroBytes.Length));
            Assert.True(_base10.TryParse("-1", identity, separator, negativeSign, numberDecimalSeparator, out var minusOne));
            Assert.AreEqual(-1, minusOne.Decimal);
            Assert.Throws(typeof(InvalidOperationException), () => _base10.Parse("a", identity, separator, negativeSign, numberDecimalSeparator));
            Assert.Throws(typeof(InvalidOperationException), () => _base10.Parse("1.a", identity, separator, negativeSign, numberDecimalSeparator));
            Assert.Throws(typeof(Exception), () => new Numeral(_base10, new List<int>(){-1}));
        }

        [Test]
        public void IndexTest()
        {
            var tmp = _base10[0];
            Assert.AreEqual(0, tmp.Decimal);
            tmp = _base10[0.0];
            Assert.AreEqual(0, tmp.Decimal);
            var zeroBytes = tmp.Bytes;
            Assert.AreEqual(zeroBytes, Enumerable.Repeat((byte)0, zeroBytes.Length));
            var zeroLong = _base10[0L];
            Assert.AreEqual(0, zeroLong.Decimal);
            var zeroULong = _base10[0UL];
            Assert.AreEqual(0, zeroULong.Decimal);
            var zeroUInt = _base10[0U];
            Assert.AreEqual(0, zeroUInt.Decimal);
            var zeroUShort = _base10[(ushort)0];
            Assert.AreEqual(0, zeroUShort.Decimal);
            var zeroSByte = _base10[(sbyte)0];
            Assert.AreEqual(0, zeroSByte.Decimal);
            var zeroByte = _base10[(byte)0];
            Assert.AreEqual(0, zeroByte.Decimal);
            var zeroShort = _base10[(short)0];
            Assert.AreEqual(0, zeroShort.Decimal);
        }

        [Test]
        public void CultureInfoTest()
        {
            var random = new Random();
            var value = random.Next();
            var base10 = Numeral.System.OfBase(10);
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
                var numerals = Numeral.System.OfBase(r2);
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
                var decimalValue = _base10[value];
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
                var binaryValue = _base2[value];
                var expected = $"{(value > 0 ? "" : "-" )}{Convert.ToString(System.Math.Abs(value), 2)}";
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
            for (var i = 0; i < 10; i++)
            {
                var value = random.Next();
                value = random.NextDouble() < 0.5 ? value : -value;
                var binaryValue = _base2[value];
                var parse = _base2.Parse(binaryValue.ToString());
                Assert.AreEqual(binaryValue.ToString(), parse.ToString());
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
                var decimalValue = _base10[value];
                // Check with a threshold
                Assert.AreEqual(decimalValue.Double, value, 0.000000000000001);
                var decimalValue2 = _base10.Parse(decimalValue.ToString());
                Assert.AreEqual(decimalValue2.Double, decimalValue.Double, 0.000000000000001);
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
                var decimalValue = _base10[value];
                Assert.AreEqual(decimalValue.Decimal, value);
                var decimalValue2 = _base10.Parse(decimalValue.ToString());
                Assert.AreEqual(decimalValue2.Decimal, decimalValue.Decimal);
            }
            
        }
    }
}