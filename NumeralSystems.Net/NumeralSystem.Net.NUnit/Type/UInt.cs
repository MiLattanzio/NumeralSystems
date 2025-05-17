using System;
using NumeralSystems.Net.Utils;
using NUnit.Framework;
using Polecola.Primitive;

namespace NumeralSystem.Net.NUnit.Type
{
    [TestFixture]
    public class UInt: IBaseTypeTest
    {
        private readonly Random _random = new();
        
        [Test]
        public void Generation()
        {
            var number = (uint)_random.Next();
            var uInt = new NumeralSystems.Net.Type.Base.UInt(){ Value = number };
            Assert.That(number, Is.EqualTo(uInt.Value));
            var fromBinary = new NumeralSystems.Net.Type.Base.UInt()
            {
                Binary = uInt.Binary
            };
            Assert.That(number, Is.EqualTo(fromBinary.Value));
            var fromBytes = new NumeralSystems.Net.Type.Base.UInt()
            {
                Bytes = uInt.Bytes
            };
            Assert.That(number, Is.EqualTo(fromBytes.Value));
            for (var i = 0; i < fromBinary.Binary.Length; i++)
            {
                Assert.That(fromBinary.Binary[i], Is.EqualTo(fromBinary.Value.GetBoolAtIndex((uint)i)));
            }
        }
        
        [Test]
        public void And()
        {
            var uIntA = new NumeralSystems.Net.Type.Base.UInt()
            {
                Value = (uint)_random.Next()
            };
            var uIntB = new NumeralSystems.Net.Type.Base.UInt()
            {
                Value = (uint)_random.Next()
            };
            Assert.That(uIntA.Value & uIntB.Value, Is.EqualTo(uIntA.And(uIntB).Value));
        }
        
        [Test]
        public void Or()
        {
            var uIntA = new NumeralSystems.Net.Type.Base.UInt()
            {
                Value = (uint)_random.Next()
            };
            var uIntB = new NumeralSystems.Net.Type.Base.UInt()
            {
                Value = (uint)_random.Next()
            };
            Assert.That(uIntA.Value | uIntB.Value, Is.EqualTo(uIntA.Or(uIntB).Value));
        }
        
        [Test]
        public void Xor()
        {
            var uIntA = new NumeralSystems.Net.Type.Base.UInt()
            {
                Value = (uint)_random.Next()
            };
            var uIntB = new NumeralSystems.Net.Type.Base.UInt()
            {
                Value = (uint)_random.Next()
            };
            Assert.That(uIntA.Value ^ uIntB.Value, Is.EqualTo(uIntA.Xor(uIntB).Value));
        }
        
        [Test]
        public void Not()
        {
            var uInt = new NumeralSystems.Net.Type.Base.UInt()
            {
                Value = (uint)_random.Next()
            };
            Assert.That(~uInt.Value, Is.EqualTo(uInt.Not().Value));
        }

        [Test]
        public void Nand()
        {
            var int32A = new NumeralSystems.Net.Type.Base.UInt()
            {
                Value = (uint)_random.Next()
            };
            var int32B = new NumeralSystems.Net.Type.Base.UInt()
            {
                Value = (uint)_random.Next()
            };
            Assert.That(~(int32A.Value & int32B.Value), Is.EqualTo(int32A.Nand(int32B).Value));
        }
        
        [Test]
        public void ReverseAnd()
        {
            var uIntA = new NumeralSystems.Net.Type.Base.UInt()
            {
                Value = (uint)_random.Next()
            };
            var uIntB = new NumeralSystems.Net.Type.Base.UInt()
            {
                Value = (uint)_random.Next()
            };
            var result = uIntA.And(uIntB);
            var success = result.ReverseAnd(uIntB, out var uIntAReversed);
            Assert.IsTrue(success);
            Assert.IsTrue(uIntAReversed.Contains(uIntA));
        }
        
        [Test]
        public void ReverseOr()
        {
            var uIntA = new NumeralSystems.Net.Type.Base.UInt()
            {
                Value = (uint)_random.Next()
            };
            var uIntB = new NumeralSystems.Net.Type.Base.UInt()
            {
                Value = (uint)_random.Next()
            };
            var result = uIntA.Or(uIntB);
            var success = result.ReverseOr(uIntB, out var uIntAReversed);
            Assert.IsTrue(success);
            Assert.IsTrue(uIntAReversed.Contains(uIntA));
        }
        
    }
}