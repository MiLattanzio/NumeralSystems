﻿using System;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit.Type
{
    [TestFixture]
    public class Float: IBaseTypeTest
    {
        private readonly Random _random = new();
        
        [Test]
        public void Generation()
        {
            var number = (float)_random.NextDouble();
            var float32 = new NumeralSystems.Net.Type.Base.Float(){ Value = number };
            Assert.That(number, Is.EqualTo(float32.Value));
            var fromBinary = new NumeralSystems.Net.Type.Base.Float()
            {
                Binary = float32.Binary
            };
            Assert.That(number, Is.EqualTo(fromBinary.Value));
            var fromBytes = new NumeralSystems.Net.Type.Base.Float()
            {
                Bytes = float32.Bytes
            };
            Assert.That(number, Is.EqualTo(fromBytes.Value));
        }

        [Test]
        public void And()
        {
            var a = (float)_random.NextDouble();
            var b = (float)_random.NextDouble();
            var floatA = new NumeralSystems.Net.Type.Base.Float()
            {
                Value = a
            };
            var floatB = new NumeralSystems.Net.Type.Base.Float()
            {
                Value = b
            };
            var aInt = BitConverter.ToInt32(BitConverter.GetBytes(a), 0);
            var bInt = BitConverter.ToInt32(BitConverter.GetBytes(b), 0);
            var resultInt = aInt & bInt;
            var result = BitConverter.ToSingle(BitConverter.GetBytes(resultInt), 0);
            Assert.That(result, Is.EqualTo(floatA.And(floatB).Value));
        }

        [Test]
        public void Or()
        {
            var a = (float)_random.NextDouble();
            var b = (float)_random.NextDouble();
            var floatA = new NumeralSystems.Net.Type.Base.Float()
            {
                Value = a
            };
            var floatB = new NumeralSystems.Net.Type.Base.Float()
            {
                Value = b
            };
            var aInt = BitConverter.ToInt32(BitConverter.GetBytes(a), 0);
            var bInt = BitConverter.ToInt32(BitConverter.GetBytes(b), 0);
            var resultInt = aInt | bInt;
            var result = BitConverter.ToSingle(BitConverter.GetBytes(resultInt), 0);
            Assert.That(result, Is.EqualTo(floatA.Or(floatB).Value));
        }

        [Test]
        public void Xor()
        {
            var a = (float)_random.NextDouble();
            var b = (float)_random.NextDouble();
            var floatA = new NumeralSystems.Net.Type.Base.Float()
            {
                Value = a
            };
            var floatB = new NumeralSystems.Net.Type.Base.Float()
            {
                Value = b
            };
            var aInt = BitConverter.ToInt32(BitConverter.GetBytes(a), 0);
            var bInt = BitConverter.ToInt32(BitConverter.GetBytes(b), 0);
            var resultInt = aInt ^ bInt;
            var result = BitConverter.ToSingle(BitConverter.GetBytes(resultInt), 0);
            Assert.That(result, Is.EqualTo(floatA.Xor(floatB).Value));
        }
        
        [Test]
        public void Not()
        {
            var a = (float)_random.NextDouble();
            var floatA = new NumeralSystems.Net.Type.Base.Float()
            {
                Value = a
            };
            var aInt = BitConverter.ToInt32(BitConverter.GetBytes(a), 0);
            var resultInt = ~aInt;
            var result = BitConverter.ToSingle(BitConverter.GetBytes(resultInt), 0);
            Assert.That(result, Is.EqualTo(floatA.Not().Value));
        }
        
        [Test]
        public void Nand()
        {
            var a = (float)_random.NextDouble();
            var b = (float)_random.NextDouble();
            var floatA = new NumeralSystems.Net.Type.Base.Float()
            {
                Value = a
            };
            var floatB = new NumeralSystems.Net.Type.Base.Float()
            {
                Value = b
            };
            var aInt = BitConverter.ToInt32(BitConverter.GetBytes(a), 0);
            var bInt = BitConverter.ToInt32(BitConverter.GetBytes(b), 0);
            var resultInt = ~(aInt & bInt);
            var result = BitConverter.ToSingle(BitConverter.GetBytes(resultInt), 0);
            Assert.That(result, Is.EqualTo(floatA.Nand(floatB).Value));
        }

        public void ReverseAnd()
        {
            var a = (float)_random.NextDouble();
            var b = (float)_random.NextDouble();
            var floatA = new NumeralSystems.Net.Type.Base.Float()
            {
                Value = a
            };
            var floatB = new NumeralSystems.Net.Type.Base.Float()
            {
                Value = b
            };
            var aInt = BitConverter.ToInt32(BitConverter.GetBytes(a), 0);
            var bInt = BitConverter.ToInt32(BitConverter.GetBytes(b), 0);
            var resultInt = aInt & bInt;
            var result = BitConverter.ToSingle(BitConverter.GetBytes(resultInt), 0);
            var floatC = floatA.And(floatB);
        }

        public void ReverseOr()
        {
            throw new System.NotImplementedException();
        }
    }
}