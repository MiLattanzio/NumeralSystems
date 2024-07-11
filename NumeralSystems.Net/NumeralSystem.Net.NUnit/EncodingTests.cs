using System;
using NumeralSystems.Net;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class EncodingTests
    {
        private readonly Random _random = new ();
        
        [Test]
        public void Encode()
        {
            var ciao = NumeralSystems.Net.Utils.Encode.Float.ToIndicesOfBase(18.04399f, 10);
            var nBase = _random.Next(2, int.MaxValue);
            var testString = "Hello World!";
            var output = Numeral.System.OfBase(nBase);
            var encoded = output.Encode(testString);
            var decoded = output.Decode(encoded);
            Assert.That(testString, Is.EqualTo(decoded));
        }
        
        
    }
}