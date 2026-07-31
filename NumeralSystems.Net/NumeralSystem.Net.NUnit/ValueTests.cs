using System;
using System.Collections.Generic;
using System.Text;
using NumeralSystems.Net;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class ValueTests
    {
        private Random _random;
        private readonly List<string> _fromCharset = new();
        private readonly List<string> _toCharset = new();

        [SetUp]
        public void Setup()
        {
            _random = new Random(42);
            _fromCharset.Clear();
            while (_fromCharset.Count < 128)
            {
                var randomChar = ((char)_random.Next(char.MinValue, char.MaxValue)).ToString();
                if (!_fromCharset.Contains(randomChar)) _fromCharset.Add(randomChar);
            }
            _toCharset.Clear();
            while (_toCharset.Count < 96)
            {
                var randomChar = ((char)_random.Next(char.MinValue, char.MaxValue)).ToString();
                if (!_toCharset.Contains(randomChar)) _toCharset.Add(randomChar);
            }
        }

        private string GenerateRandomString(int length)
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var randomChar = _fromCharset[_random.Next(_fromCharset.Count)];
                stringBuilder.Append(randomChar);
            }
            return stringBuilder.ToString();
        }

        private string ValueToString(Value value, List<string> source)
        {
            var stringBuilder = new StringBuilder();
            foreach (var index in value.Indices)
            {
                stringBuilder.Append(source[index]);
            }
            return stringBuilder.ToString();
        }
        
        private string ValueToString(Value value)
        {
            var stringBuilder = new StringBuilder();
            foreach (var index in value.Indices)
            {
                stringBuilder.Append((char)index);
            }
            return stringBuilder.ToString();
        }

        [Test]
        public void RandomStringTest()
        {
            var sourceString = GenerateRandomString(_random.Next(1, 255));
            var alphabet = new NumeralAlphabet(_fromCharset);
            var sourceIdentity = new List<string>(alphabet.Symbols);
            var value = Value.FromString(sourceString, alphabet);
            var valueString = ValueToString(value, sourceIdentity);
            Assert.That(sourceString, Is.EqualTo(valueString));
            var valueChanged = value.ToBase(_toCharset.Count);
            var valueSource = value.ToBase(_fromCharset.Count);
            var valueSourceString = ValueToString(valueSource, sourceIdentity);
            Assert.That(sourceString, Is.EqualTo(valueSourceString));
            Assert.That(valueChanged.Base, Is.EqualTo(_toCharset.Count));
        }
        
        [Test]
        public void RandomStringFitFalseTest()
        {
            var sourceString = GenerateRandomString(_random.Next(1, 255));
            var value = Value.FromString(sourceString);
            var valueString = ValueToString(value);
            Assert.That(sourceString, Is.EqualTo(valueString));
        }
        
        [Test]
        public void RandomStringFitTrueTest()
        {
            var sourceString = GenerateRandomString(_random.Next(1, 255));
            var value = Value.FromString(sourceString, true);
            var valueString = ValueToString(value);
            Assert.That(sourceString, Is.EqualTo(valueString));
        }

        [Test]
        public void SubZeroNumeralValueSpecificTest1()
        {
            var zerozeroone = NumeralValue.FromDecimal(0.01m);
            var binary = zerozeroone.ToBase(2);
            Assert.That(zerozeroone.ToDecimal(), Is.EqualTo(binary.ToDecimal()));
            var original = binary.ToBase(zerozeroone.Base);
            Assert.That(original.ToDecimal(), Is.EqualTo(zerozeroone.ToDecimal()));
        }
        
        [Test]
        public void NumeralValueGeneralTest()
        {
            var sourceNumber = _random.Next() + _random.NextDouble();
            var number = NumeralValue.FromDouble(sourceNumber);
            var binary = number.ToBase(2);
            Assert.That(number.ToDecimal(), Is.EqualTo(binary.ToDecimal()));
            var original = binary.ToBase(number.Base);
            Assert.That(original.ToDecimal(), Is.EqualTo(number.ToDecimal()));
            //Assert.That(sourceNumber, Is.EqualTo(original.ToDouble()));
        }
    }
}
