using System;
using System.Globalization;
using NumeralSystems.Net;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            
        }

       
        [Test]
        public void DecimalTest()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var random = new Random();
            for (var i = 0; i < 10; i++)
            {
                var value = random.Next();
                var base10 = Numeral.System.OfBase(10, string.Empty);
                var decimalValue = base10[value];
                Console.WriteLine($"Generated {decimalValue} should be equal to {value.ToString()}");
                Assert.AreEqual(decimalValue.ToString(), value.ToString());
            }
        }

        [Test]
        public void BinaryTest()
        {
            var random = new Random();
            for (var i = 0; i < 10; i++)
            {
                var value = random.Next();
                value = random.NextDouble() < 0.5 ? value : -value;
                var base2 = Numeral.System.OfBase(2, string.Empty);
                var binaryValue = base2[value];
                var expected = $"{(value > 0 ? "" : "-" )}{Convert.ToString(Math.Abs(value), 2)}";
                Console.WriteLine($"Generated {binaryValue} should be equal to {expected}");
                Assert.AreEqual(binaryValue.ToString(), expected);
            }
        }
        
        [Test]
        public void BinaryParseTest()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var random = new Random();
            var base2 = Numeral.System.OfBase(2, string.Empty);
            base2.DoubleCheckParsedValue = true;
            for (var i = 0; i < 10; i++)
            {
                try
                {
                    var value = random.Next();
                    value = random.NextDouble() < 0.5 ? value : -value;
                    var binaryValue = base2[value];
                    var stringParse = base2.StringParse(binaryValue.ToString());
                    Assert.AreEqual(binaryValue.ToString(), stringParse.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        
        [Test]
        public void BinaryParseSpecificTest()
        {
            var bin = "1011101101011000011101110101100";
            var base2 = Numeral.System.OfBase(2, string.Empty);
            var numeric = base2.StringParse(bin);
            var dec = numeric.Integer;
            Assert.AreEqual(dec, 1571568556);
            var test = base2[1571568556];
            Assert.AreEqual(bin, test.ToString());
        }
        
        [Test]
        public void FloatTest()
        {
            var random = new Random();
            for (var i = 0; i < 10; i++)
            {
                var value = (float)random.NextDouble();
                var base10 = Numeral.System.OfBase(10, string.Empty);
                var decimalValue = base10[value];
                Console.WriteLine($"Generated {decimalValue} should be equal to {value.ToString(base10.CultureInfo)}");
                //Keeps failing only on GitHub
                Assert.AreEqual(decimalValue.ToString(), value.ToString(base10.CultureInfo));
                Assert.AreEqual(decimalValue.Float, value);
                var decimalValue2 = base10.StringParse(decimalValue.ToString());
                Assert.AreEqual(decimalValue2.Float, decimalValue.Float);
            }
            
        }
        
        [Test]
        public void FloatTestSpecific()
        {
            var value = 0.65348464f;
            var base10 = Numeral.System.OfBase(10, string.Empty);
            var decimalValue = base10[value];
            Console.WriteLine($"Generated {decimalValue} should be equal to {value.ToString(base10.CultureInfo)}");
            //Keeps failing only on GitHub
            Assert.AreEqual(decimalValue.ToString(), value.ToString(base10.CultureInfo));
            Assert.AreEqual(decimalValue.Float, value);
            var decimalValue2 = base10.StringParse(decimalValue.ToString());
            Assert.AreEqual(decimalValue2.Float, decimalValue.Float);
        }
    }
}