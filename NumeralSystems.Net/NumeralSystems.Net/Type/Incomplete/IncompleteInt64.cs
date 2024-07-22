using System;
using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Utils;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = NumeralSystems.Net.Utils.Convert;
using Int64 = NumeralSystems.Net.Type.Base.Int64;

namespace NumeralSystems.Net.Type.Incomplete
{
    public class IncompleteInt64 : IIRregularOperable<IncompleteInt64, Int64, long>
    {
        private bool?[] _binary;

        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8 * sizeof(long)).Select(x => x as bool?).ToArray();
            set
            {
                if (null == value)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8 * sizeof(long)).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= (8 * sizeof(long)))
                    {
                        _binary = value.Take(8 * sizeof(long)).ToArray();
                    }
                    else
                    {
                        _binary = System.Linq.Enumerable.Repeat(false, (8 * sizeof(long)) - value.Length)
                            .Select(x => x as bool?)
                            .Concat(value).ToArray();
                    }
                }
            }
        }

        public bool IsComplete => Binary.All(x => x != null);
        public int Permutations => Sequence.PermutationsCount(2, Binary.Count(x => x is null), true);

        public Int64 this[int value] => Int64.FromBinary(value.ToBoolArray());

        public IEnumerable<Int64> Enumerable => System.Linq.Enumerable.Range(0, Permutations).Select(x => this[x]);

        public IncompleteByte[] ByteArray => IncompleteByteArray.ArrayOf(Binary);

        public IncompleteByte[] ToByteArray() => IncompleteByteArray.ArrayOf(Binary.Select(x => x).ToArray());
        public string ToString(string missingSeparator = "*") => string.Join(string.Empty, Binary.Group(8).Select(x => x.Reverse().ToArray()).SelectMany(x => x).Select(x => null == x ? missingSeparator : (x.Value ? 1 : 0).ToString()));

        public IncompleteInt64 Or(Int64 other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public bool Contains(Int64 value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        public bool Contains(IncompleteInt64 value)
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


        public IncompleteInt64 Not() => new()
        {
            Binary = Binary.Select(x => !x).ToArray()
        };

        public IncompleteInt64 Xor(IncompleteInt64 other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteInt64 Xor(Int64 other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteInt64 And(IncompleteInt64 other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteInt64 And(Int64 other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteInt64 Or(IncompleteInt64 other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public bool ReverseAnd(Int64 right, out IncompleteInt64 result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteInt64()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseAnd(IncompleteInt64 right, out IncompleteInt64 result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteInt64()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(Int64 right, out IncompleteInt64 result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteInt64()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(IncompleteInt64 right, out IncompleteInt64 result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteInt64()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }
    }
}