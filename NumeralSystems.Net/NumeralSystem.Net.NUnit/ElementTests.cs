using System.Linq;
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
            Assert.IsTrue(Numeral.System.Characters.Printable.Count() == Numeral.System.Characters.Printable.Distinct().Count());
            Assert.IsTrue(Numeral.System.Characters.NotPrintable.Count() == Numeral.System.Characters.NotPrintable.Distinct().Count());
        }
    }
}