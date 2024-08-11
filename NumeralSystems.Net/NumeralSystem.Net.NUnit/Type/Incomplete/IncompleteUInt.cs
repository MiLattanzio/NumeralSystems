using System.Linq;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit.Type.Incomplete
{
    [TestFixture]
    public class IncompleteUInt
    {
        
        [Test]
        public void Unknown()
        {
            var binary = Enumerable.Range(0, sizeof(uint)*8).Select(x => (bool?)null).ToArray();
            var incomplete = new NumeralSystems.Net.Type.Incomplete.IncompleteUInt() {Binary = binary};
            Assert.IsFalse(incomplete.IsComplete);
            var minVal = incomplete[0];
            Assert.AreEqual(0, minVal.Value);
            Assert.AreEqual(uint.MaxValue, incomplete.Permutations);
            var maxVal = incomplete[uint.MaxValue];
            Assert.AreEqual(uint.MaxValue, maxVal.Value);
        }
    }
}