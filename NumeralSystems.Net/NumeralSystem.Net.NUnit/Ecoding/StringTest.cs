using NumeralSystems.Net.Ecoding;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit.Ecoding
{
    [TestFixture]
    [TestOf(typeof(String))]
    public class StringTest
    {

        [Test]
        public void GetIdentity()
        {
            var str = new String();
            var identity = str.GetIdentity("Hello World");
            Assert.AreEqual(identity.Count, 8);
            Assert.AreEqual(identity[0], 'H');
            Assert.AreEqual(identity[1], 'e');
            Assert.AreEqual(identity[2], 'l');
            Assert.AreEqual(identity[3], 'o');
            Assert.AreEqual(identity[4], ' ');
            Assert.AreEqual(identity[5], 'W');
            Assert.AreEqual(identity[6], 'r');
            Assert.AreEqual(identity[7], 'd');
        }
    }
}