using System;
using System.Linq;
using NumeralSystems.Net;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class EncodingTests
    {
        private readonly Random _random = new ();
        
        [Test]
        public void Identities()
        {
            Assert.That(Numeral.System.Characters.Alphanumeric.Count(), Is.EqualTo(Numeral.System.Characters.Alphanumeric.Distinct().Count()));
            Assert.That(Numeral.System.Characters.Numbers.Count(), Is.EqualTo(Numeral.System.Characters.Numbers.Distinct().Count()));
            Assert.That(Numeral.System.Characters.AlphanumericLower.Count(), Is.EqualTo(Numeral.System.Characters.AlphanumericLower.Distinct().Count()));
            Assert.That(Numeral.System.Characters.AlphanumericUpper.Count(), Is.EqualTo(Numeral.System.Characters.AlphanumericUpper.Distinct().Count()));
            Assert.That(Numeral.System.Characters.AlphanumericSymbols.Count(), Is.EqualTo(Numeral.System.Characters.AlphanumericSymbols.Distinct().Count()));
            Assert.That(Numeral.System.Characters.LowerLetters.Count(), Is.EqualTo(Numeral.System.Characters.LowerLetters.Distinct().Count()));
            Assert.That(Numeral.System.Characters.UpperLetters.Count(), Is.EqualTo(Numeral.System.Characters.UpperLetters.Distinct().Count()));
            Assert.That(Numeral.System.Characters.Symbols.Count(), Is.EqualTo(Numeral.System.Characters.Symbols.Distinct().Count()));
            Assert.That(Numeral.System.Characters.WhiteSpaces.Count(), Is.EqualTo(Numeral.System.Characters.WhiteSpaces.Distinct().Count()));
            Assert.That(Numeral.System.Characters.Printable.Count(), Is.EqualTo(Numeral.System.Characters.Printable.Distinct().Count()));
            Assert.That(Numeral.System.Characters.NotPrintable.Count(), Is.EqualTo(Numeral.System.Characters.NotPrintable.Distinct().Count()));
            Assert.That(Numeral.System.Characters.All.Count(), Is.EqualTo(Numeral.System.Characters.All.Distinct().Count()));
        }
        
        
    }
}