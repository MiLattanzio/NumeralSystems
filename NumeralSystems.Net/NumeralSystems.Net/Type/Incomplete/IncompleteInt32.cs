using System;
using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Utils;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = NumeralSystems.Net.Utils.Convert;
using Int32 = NumeralSystems.Net.Type.Base.Int32;

namespace NumeralSystems.Net.Type.Incomplete
{
    public class IncompleteInt32: IIRregularOperable<IncompleteInt32, Int32, int>
    {
        private bool?[] _binary;

        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8 * sizeof(int)).Select(x => x as bool?).ToArray();
            set
            {
                if (null == value)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8 * sizeof(int)).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= (8 * sizeof(int)))
                    {
                        _binary = value.Take(8 * sizeof(int)).ToArray();
                    }
                    else
                    {
                        _binary = System.Linq.Enumerable.Repeat(false, (8 * sizeof(int)) - value.Length).Select(x => x as bool?)
                            .Concat(value).ToArray();
                    }
                }
            }
        }
        public bool IsComplete => Binary.All(x => x != null);
        public int Permutations => Sequence.PermutationsCount(2, Binary.Count(x => x is null), true);

        public Int32 this[int value] => Int32.FromBinary(value.ToBoolArray());
        
        public IEnumerable<Int32> Enumerable => System.Linq.Enumerable.Range(0, Permutations).Select(x => this[x]);

        public IncompleteByteArray ByteArray => new() {
            Binary = Binary
        };
        
        public IncompleteByteArray ToByteArray() => new () {
            Binary = Binary.Select(x => x).ToArray()
        };

        public override string ToString() => ToString("*");

        public string ToString(string missingSeparator) => string.Join(string.Empty, Binary.Group(8).Select(x => x.Reverse().ToArray()).SelectMany(x => x).Select(x => null == x ? missingSeparator : (x.Value ? 1 : 0).ToString()));
        public IncompleteInt32 Or(Int32 other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public bool Contains(Int32 value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        public bool Contains(IncompleteInt32 value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            for (var i = 0; i < bytes.Length; i++)
            {
                var bit = bytes[i];
                if (bit is null) continue;
                var bit2 = bytesBinary[i];
                if (bit2 is null) continue;
                if (bytesBinary[i] != bit) return false;
            }
            return true;
        }

        public IncompleteInt32 Not() => new()
        {
            Binary = Binary.Select(x => !x).ToArray()
        };

        public IncompleteInt32 Xor(IncompleteInt32 other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteInt32 Xor(Int32 other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteInt32 And(IncompleteInt32 other) => new() {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteInt32 And(Int32 other) => new() {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteInt32 Or(IncompleteInt32 other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public bool ReverseAnd(Int32 right, out IncompleteInt32 result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteInt32()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseAnd(IncompleteInt32 right, out IncompleteInt32 result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteInt32()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(Int32 right, out IncompleteInt32 result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteInt32()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(IncompleteInt32 right, out IncompleteInt32 result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteInt32()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }
    }
}