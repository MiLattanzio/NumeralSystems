using NUnit.Framework;
using NumeralSystems.Net.Utils;

namespace NumeralSystem.Net.NUnit.Utils
{
    [TestFixture]
    public class SequenceTests
    {
        [Test]
        public void CombinationsWithoutRepetition()
        {
            Assert.That(Sequence.CombinationsCount(5, 3), Is.EqualTo(10));
            Assert.That(Sequence.CombinationsCount(10, 2), Is.EqualTo(45));
        }

        [Test]
        public void CombinationsWithRepetition()
        {
            Assert.That(Sequence.CombinationsCount(5, 3, true), Is.EqualTo(35));
            Assert.That(Sequence.CombinationsCount(10, 2, true), Is.EqualTo(55));
        }
    }
}
