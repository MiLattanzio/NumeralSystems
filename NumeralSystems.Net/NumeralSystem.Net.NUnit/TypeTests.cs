﻿using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using NumeralSystems.Net;
using NumeralSystems.Net.Type.Base;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit
{
    [MemoryDiagnoser()]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn()]
    public class TypeTests
    {
        [SetUp]
        public void Setup()
        {
            
        }
        
        [Test]
        public void Random()
        {
            var random = new Random();
            var base2 = Numeral.System.OfBase(10, string.Empty);
            for (var i = 0; i < 20; i++)
            {
                var value = (decimal)random.NextDouble();
                var decimalValue = base2[value];
                Console.WriteLine($"Generated {decimalValue} should be equal to {value.ToString(base2.CultureInfo)}");
                Assert.AreEqual(decimalValue.Decimal, value);
                var decimalValue2 = base2.Parse(decimalValue.ToString());
                Assert.AreEqual(decimalValue2.Decimal, decimalValue.Decimal);
                var dec = new NumeralDecimal(decimalValue);
                Console.WriteLine(dec.Value);
                Console.WriteLine(dec);
            }
        }
    }
}