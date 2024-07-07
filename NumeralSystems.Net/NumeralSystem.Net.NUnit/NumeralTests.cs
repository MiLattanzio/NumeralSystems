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
using Int32 = NumeralSystems.Net.Type.Base.Int32;
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
        private NumeralSystems.Net.NumeralSystem _base10Separator;
        
        
        [SetUp]
        public void Setup()
        {
            _random = new Random();
            _base2 = Numeral.System.OfBase(2, string.Empty);
            _base10 = Numeral.System.OfBase(10, string.Empty);
            _base10Separator = Numeral.System.OfBase(10, ";");
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
            var randomInt32 = new Int32()
            {
                Value = randomInt
            };
            //Check that the Int32 is equal to the integer
            Assert.AreEqual(randomInt, randomInt32.Value);
            //Check that the Int32 is equal to the integer after conversion
            randomInt32.Value = randomInt;
            Assert.AreEqual(randomInt, randomInt32.Value);
            //Create a NumeralInt32 from randomNumeral
            var randomNumeralInt = new NumeralInt32()
            {
                Numeral = randomNumeral
            };
            //Check that the NumeralInt32 is equal to the integer
            Assert.AreEqual(randomInt, randomNumeralInt.Value);
            Assert.AreEqual(randomInt, randomNumeralInt.Numeral.Integer);
            //Check that the NumeralInt32 is equal to the integer after conversion
            randomNumeralInt.Value = randomInt;
            Assert.AreEqual(randomInt, randomNumeralInt.Value);
            Assert.AreEqual(randomInt, randomNumeralInt.Numeral.Integer);
        }
        
        [Test]
        public void ParseTest()
        {
            Assert.False(_base10Separator.TryParse(null, out var zero));
            Assert.AreEqual(0, zero.Decimal);
            Assert.False(_base10Separator.TryParse(string.Empty, out zero));
            Assert.AreEqual(0, zero.Decimal);
            Assert.True(_base10Separator.TryParse("0.0", out zero));
            Assert.AreEqual(0, zero.Decimal);
            var zeroBytes = zero.Bytes;
            Assert.AreEqual(zeroBytes, Enumerable.Repeat((byte)0, zeroBytes.Length));
            Assert.True(_base10Separator.TryParse("-1", out var minusOne));
            Assert.AreEqual(-1, minusOne.Decimal);
            Assert.Throws(typeof(InvalidOperationException), () => _base10Separator.Parse("a"));
            Assert.Throws(typeof(InvalidOperationException), () => _base10Separator.Parse("1.a"));
            Assert.Throws(typeof(Exception), () => new Numeral(_base10Separator, new List<int>(){-1}));
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
            
            tmp = _base10[100000.00000000500];
            Assert.AreEqual(100000.00000000500, tmp.Decimal);
            tmp = _base10[100000.00000000500d];
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
                var decimalValue = _base10[value];
                Assert.AreEqual(decimalValue.ToString(), value.ToString());
                var numInt = new NumeralInt32(decimalValue);
                var numIntClone = new NumeralInt32(value);
                Assert.AreEqual(value, numIntClone.Value);
                Assert.AreEqual(numInt.Value, numIntClone.Value);
                Assert.AreEqual(numInt.Bytes, numIntClone.Bytes);
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
                Assert.AreEqual(decimalValue.Double, value);
                var decimalValue2 = _base10.Parse(decimalValue.ToString());
                Assert.AreEqual(decimalValue2.Double, decimalValue.Double);
                var numInt = new NumeralDouble(value);
                var numIntClone = new NumeralDouble(decimalValue.Double);
                Assert.AreEqual(value, numIntClone.Value);
                Assert.AreEqual(numInt.Value, numIntClone.Value);
                Assert.AreEqual(numInt.Bytes, numIntClone.Bytes);
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
                var numInt = new NumeralDecimal(value);
                var numIntClone = new NumeralDecimal(decimalValue.Decimal);
                Assert.AreEqual(value, numIntClone.Value);
                Assert.AreEqual(numInt.Value, numIntClone.Value);
                Assert.AreEqual(numInt.Bytes, numIntClone.Bytes);
            }
            
        }
    }
}