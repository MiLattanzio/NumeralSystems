﻿using System;
using System.Linq;
using NumeralSystems.Net.Type.Base;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit.Math
{
    [TestFixture]
    public class ReverseBitwise
    {
        private readonly Random _random = new();
        
        [Test]
        public void ReverseAnd()
        {
            var int32A = new Int()
            {
                Value = _random.Next()
            };
            var int32B = new Int()
            {
                Value = _random.Next()
            };
            var result = int32A.And(int32B);
            var success = result.ReverseAnd(int32B, out var int32AReversed);
            Assert.IsTrue(success);
            Assert.IsTrue(int32AReversed.Contains(int32A));
        }
        
        [Test]
        public void ReverseOr()
        {
            var int32A = new Int()
            {
                Value = _random.Next()
            };
            var int32B = new Int()
            {
                Value = _random.Next()
            };
            var result = int32A.Or(int32B);
            var success = result.ReverseOr(int32B, out var int32AReversed);
            Assert.IsTrue(success);
            Assert.IsTrue(int32AReversed.Contains(int32A));
            Assert.Multiple(() =>
            {
                foreach (var i in int32AReversed.Enumerable)
                {
                    Assert.IsTrue(int32AReversed.Contains(i));
                }
            });
        }
    }
}