using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Type.Incomplete;
using NumeralSystems.Net.Utils;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit.Type.Incomplete
{
    [TestFixture]
    public class IncompleteByteOperations
    {
        [Test]
        public void NandNorXnor()
        {
            var left = new IncompleteByte { Binary = new bool?[] { true, null, false, false, false, false, false, false } };
            var right = new Byte { Value = 1 };
            var expectedNand = left.Binary.Nand(right.Binary);
            var expectedNor = left.Binary.Nor(right.Binary);
            var expectedXnor = left.Binary.Xnor(right.Binary);
            Assert.That(left.Nand(right).Binary, Is.EqualTo(expectedNand));
            Assert.That(left.Nor(right).Binary, Is.EqualTo(expectedNor));
            Assert.That(left.Xnor(right).Binary, Is.EqualTo(expectedXnor));
        }

        [Test]
        public void ShiftLeftRight()
        {
            var left = new IncompleteByte { Binary = new bool?[] { true, false, false, false, false, false, false, false } };
            var expectedLeft = left.Binary.ShiftLeft(1);
            var expectedRight = left.Binary.ShiftRight(1);
            Assert.That(left.ShiftLeft(1).Binary, Is.EqualTo(expectedLeft));
            Assert.That(left.ShiftRight(1).Binary, Is.EqualTo(expectedRight));
        }
    }
}
