using System;
using System.Linq;
using System.Text;
using NumeralSystems.Net;
using NUnit.Framework;
using Convert = NumeralSystems.Net.Utils.Convert;

namespace NumeralSystem.Net.NUnit
{
    [TestFixture]
    public class BaseConversionTests
    {
        private readonly Random _random = new ();
        [Test]
        public void BaseConversionTest()
        {
            var randomBase = _random.Next(2, Numeral.System.Characters.All.Count());
            //Create random string
            var testString = "Hello World!";
            var output = Convert.ToStringOfBase(randomBase, Encoding.UTF8.GetBytes(testString));
            var input = Encoding.UTF8.GetString(Convert.FromStringOfBase(randomBase, output));
            Assert.IsTrue(testString.Equals(input));
        }
    }
}