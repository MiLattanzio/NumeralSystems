using System;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit.Type
{
    [TestFixture]
    public class Double: IBaseTypeTest
    {
        private readonly Random _random = new();
        
        [Test]
        public void Generation()
        {
            var number = _random.NextDouble();
            var @double = new NumeralSystems.Net.Type.Base.Double(){ Value = number };
            Assert.That(number, Is.EqualTo(@double.Value));
            var fromBinary = new NumeralSystems.Net.Type.Base.Double()
            {
                Binary = @double.Binary
            };
            Assert.That(number, Is.EqualTo(fromBinary.Value));
            var fromBytes = new NumeralSystems.Net.Type.Base.Double()
            {
                Bytes = @double.Bytes
            };
            Assert.That(number, Is.EqualTo(fromBytes.Value));
        }

        [Test]
        public void And()
        {
            var doubleA = new NumeralSystems.Net.Type.Base.Double()
            {
                Value = _random.NextDouble()
            };
            var doubleB = new NumeralSystems.Net.Type.Base.Double()
            {
                Value = _random.NextDouble()
            };
            var aInt = BitConverter.ToInt64(BitConverter.GetBytes(doubleA.Value), 0);
            var bInt = BitConverter.ToInt64(BitConverter.GetBytes(doubleB.Value), 0);
            var resultInt = aInt & bInt;
            var result = BitConverter.ToDouble(BitConverter.GetBytes(resultInt), 0);
            Assert.That(result, Is.EqualTo(doubleA.And(doubleB).Value));
        }

        [Test]
        public void Or()
        {
            var doubleA = new NumeralSystems.Net.Type.Base.Double()
            {
                Value = _random.NextDouble()
            };
            var doubleB = new NumeralSystems.Net.Type.Base.Double()
            {
                Value = _random.NextDouble()
            };
            var aInt = BitConverter.ToInt64(BitConverter.GetBytes(doubleA.Value), 0);
            var bInt = BitConverter.ToInt64(BitConverter.GetBytes(doubleB.Value), 0);
            var resultInt = aInt | bInt;
            var result = BitConverter.ToDouble(BitConverter.GetBytes(resultInt), 0);
            Assert.That(result, Is.EqualTo(doubleA.Or(doubleB).Value));
        }

        [Test]
        public void Xor()
        {
            var doubleA = new NumeralSystems.Net.Type.Base.Double()
            {
                Value = _random.NextDouble()
            };
            var doubleB = new NumeralSystems.Net.Type.Base.Double()
            {
                Value = _random.NextDouble()
            };
            var aInt = BitConverter.ToInt64(BitConverter.GetBytes(doubleA.Value), 0);
            var bInt = BitConverter.ToInt64(BitConverter.GetBytes(doubleB.Value), 0);
            var resultInt = aInt ^ bInt;
            var result = BitConverter.ToDouble(BitConverter.GetBytes(resultInt), 0);
            Assert.That(result, Is.EqualTo(doubleA.Xor(doubleB).Value));
        }

        [Test]
        public void Not()
        {
            var doubleA = new NumeralSystems.Net.Type.Base.Double()
            {
                Value = _random.NextDouble()
            };
            var aInt = BitConverter.ToInt64(BitConverter.GetBytes(doubleA.Value), 0);
            var resultInt = ~aInt;
            var result = BitConverter.ToDouble(BitConverter.GetBytes(resultInt), 0);
            Assert.That(result, Is.EqualTo(doubleA.Not().Value));
        }

        [Test]
        public void Nand()
        {
            var doubleA = new NumeralSystems.Net.Type.Base.Double()
            {
                Value = _random.NextDouble()
            };
            var doubleB = new NumeralSystems.Net.Type.Base.Double()
            {
                Value = _random.NextDouble()
            };
            var aInt = BitConverter.ToInt64(BitConverter.GetBytes(doubleA.Value), 0);
            var bInt = BitConverter.ToInt64(BitConverter.GetBytes(doubleB.Value), 0);
            var resultInt = ~(aInt & bInt);
            var result = BitConverter.ToDouble(BitConverter.GetBytes(resultInt), 0);
            Assert.That(result, Is.EqualTo(doubleA.Nand(doubleB).Value));
        }

        [Test]
        public void ReverseAnd()
        {
            var doubleA = new NumeralSystems.Net.Type.Base.Double()
            {
                Value = _random.NextDouble()
            };
            var doubleB = new NumeralSystems.Net.Type.Base.Double()
            {
                Value = _random.NextDouble()
            };
            var floatC = doubleA.And(doubleB);
            var success = floatC.ReverseAnd(doubleB, out var reversedA);
            Assert.IsTrue(success);
            Assert.IsTrue(reversedA.Contains(doubleA));
        }

        [Test]
        public void ReverseOr()
        {
            var doubleA = new NumeralSystems.Net.Type.Base.Double()
            {
                Value = _random.NextDouble()
            };
            var doubleB = new NumeralSystems.Net.Type.Base.Double()
            {
                Value = _random.NextDouble()
            };
            var floatC = doubleA.Or(doubleB);
            var success = floatC.ReverseOr(doubleB, out var reversedA);
            Assert.IsTrue(success);
            Assert.IsTrue(reversedA.Contains(doubleA));
        }
    }
}