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
        public Random Random = new ();
        public List<string> _fromCharset = new ();
        public List<string> _toCharset = new ();

        [SetUp]
        public void Setup()
        {
            _fromCharset.Clear();
            for (var i = 0; i < Random.Next(0, char.MaxValue); i++)
            {
                var randomChar = ((char) Random.Next(char.MinValue, char.MaxValue)).ToString();
                if (!_fromCharset.Contains(randomChar)) _fromCharset.Add(randomChar);
            }
            _toCharset.Clear();
            for (var i = 0; i < Random.Next(0, char.MaxValue); i++)
            {
                var randomChar = ((char) Random.Next(char.MinValue, char.MaxValue)).ToString();
                if (!_toCharset.Contains(randomChar)) _toCharset.Add(randomChar);
            }
        }

        private string GenerateRandomString(int length)
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var randomChar = _fromCharset[Random.Next(0, _fromCharset.Count-1)];
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
            var sourceString = GenerateRandomString(Random.Next(0, 255));
            var value = Value.FromString(sourceString, new HashSet<string>(_fromCharset));
            var valueString = ValueToString(value, _fromCharset);
            Assert.That(sourceString, Is.EqualTo(valueString));
            var valueChanged = value.ToBase(_toCharset.Count);
            var valueChangedString = ValueToString(valueChanged, _toCharset);
            Assert.That(sourceString, Is.Not.EqualTo(valueChangedString));
            var valueSource = value.ToBase(_fromCharset.Count);
            var valueSourceString = ValueToString(valueSource, _fromCharset);
            Assert.That(sourceString, Is.EqualTo(valueSourceString));
        }
        
        [Test]
        public void RandomStringFitFalseTest()
        {
            var sourceString = GenerateRandomString(Random.Next(0, 255));
            var value = Value.FromString(sourceString);
            var valueString = ValueToString(value);
            Assert.That(sourceString, Is.EqualTo(valueString));
        }
        
        [Test]
        public void RandomStringFitTrueTest()
        {
            var sourceString = GenerateRandomString(Random.Next(0, 255));
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
            var sourceNumber = Random.Next() + Random.NextDouble();
            var number = NumeralValue.FromDouble(sourceNumber);
            var binary = number.ToBase(2);
            Assert.That(number.ToDecimal(), Is.EqualTo(binary.ToDecimal()));
            var original = binary.ToBase(number.Base);
            Assert.That(original.ToDecimal(), Is.EqualTo(number.ToDecimal()));
            Assert.That(sourceNumber, Is.EqualTo(original.ToDouble()));
        }
    }
}