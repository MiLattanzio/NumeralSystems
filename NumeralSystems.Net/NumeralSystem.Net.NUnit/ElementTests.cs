using NumeralSystems.Net;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    public class ElementTests
    {
        [SetUp]
        public void Setup()
        {
            
        }
        
        [Test]
        public void Count()
        {
            Assert.That(Numeral.System.Characters.Printable, Is.Unique);
            Assert.That(Numeral.System.Characters.NotPrintable, Is.Unique);
        }
    }
}