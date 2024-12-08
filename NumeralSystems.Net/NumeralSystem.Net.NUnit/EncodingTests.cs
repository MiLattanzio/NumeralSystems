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

        [Test]
        public void Base64Tests()
        {
            var value = "test";
            var identity = new[]
            {
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                '+', '/'
            }.Select(x => x.ToString()).ToArray();
            var base64 = Numeral.System.OfBase(identity.Length);
            var re = base64.Parse("dGVzdA");
            var result = re.To(Numeral.System.OfBase(char.MaxValue));
            var youtubeId = string.Concat(result.IntegralIndices.Select(x => (char)x).ToList());


        }
        
        
    }
}