using NUnit.Framework;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Type.Incomplete;
using System.Linq;

namespace NumeralSystem.Net.NUnit.Type.Incomplete
{
    [TestFixture]
    public class IncompleteDecimalTests
    {
        private static bool? Or(bool? left, bool right)
        {
            if (left is null)
                return right ? true : (bool?)null;
            return left.Value || right;
        }

        private static bool? Or(bool? left, bool? right)
        {
            return (left, right) switch
            {
                (null, null) => null,
                (false, null) => null,
                (true, null) => true,
                (null, false) => null,
                (null, true) => true,
                (false, false) => false,
                (false, true) => true,
                (true, false) => true,
                (true, true) => true,
            };
        }

        private static bool?[] Or(bool?[] left, bool[] right)
            => left.Select((l, i) => Or(l, right[i])).ToArray();

        private static bool?[] Or(bool?[] left, bool?[] right)
            => left.Select((l, i) => Or(l, right[i])).ToArray();

        [Test]
        public void OrWithDecimal()
        {
            var baseLeft = new Decimal { Value = 1m };
            var baseRight = new Decimal { Value = 2m };
            var left = baseLeft.Incomplete();
            left.Binary[0] = null; // introduce an unknown bit
            var expected = Or(left.Binary.ToArray(), baseRight.Binary);
            var result = left.Or(baseRight);
            Assert.That(result.Binary, Is.EqualTo(expected));
        }

        [Test]
        public void OrWithIncompleteDecimal()
        {
            var baseLeft = new Decimal { Value = 1m };
            var baseRight = new Decimal { Value = 2m };
            var left = baseLeft.Incomplete();
            var right = baseRight.Incomplete();
            right.Binary[1] = null; // introduce an unknown bit
            var expected = Or(left.Binary.ToArray(), right.Binary.ToArray());
            var result = left.Or(right);
            Assert.That(result.Binary, Is.EqualTo(expected));
        }
    }
}
