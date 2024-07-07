using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;
using Convert = NumeralSystems.Net.Utils.Convert;

namespace NumeralSystems.Net.Type.Incomplete
{
    public class IncompleteFloat: IIRregularOperable<IncompleteFloat, Float, float>
    {
        private bool?[] _binary;
        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8 * sizeof(float)).Select(x => x as bool?).ToArray();
            set
            {
                if (null == value)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8 * sizeof(float)).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= (8 * sizeof(float)))
                    {
                        _binary = value.Take(8 * sizeof(float)).ToArray();
                    }
                    else
                    {
                        _binary = System.Linq.Enumerable.Repeat(false, (8 * sizeof(float)) - value.Length).Select(x => x as bool?)
                            .Concat(value).ToArray();
                    }
                }
            }
        }
        
        public bool IsComplete => Binary.All(x => x != null);
        public int Permutations => Sequence.PermutationsCount(2, Binary.Count(x => x is null), true);
        public Float this[int value] => Float.FromBinary(value.ToBoolArray());
        
        public IEnumerable<Float> Enumerable => System.Linq.Enumerable.Range(0, Permutations).Select(x => this[x]);

        public IncompleteByteArray ByteArray => new() {
            Binary = Binary
        };
        
        public IncompleteByteArray ToByteArray() => new () {
            Binary = Binary.Select(x => x).ToArray()
        };
        
        public string ToString(string missingSeparator = "*") => string.Join(string.Empty, Binary.Group(8).Select(x => x.Reverse().ToArray()).SelectMany(x => x).Select(x => null == x ? missingSeparator : (x.Value ? 1 : 0).ToString()));
        public IncompleteFloat Or(Float other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public bool Contains(Float value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        public bool Contains(IncompleteFloat value)
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

        public IncompleteFloat Not() => new()
        {
            Binary = Binary.Not()
        };

        public IncompleteFloat Xor(IncompleteFloat other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteFloat Xor(Float other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteFloat And(IncompleteFloat other) => new() {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteFloat And(Float other) => new() {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteFloat Or(IncompleteFloat other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public bool ReverseAnd(Float right, out IncompleteFloat result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteFloat()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseAnd(IncompleteFloat right, out IncompleteFloat result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteFloat()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(Float right, out IncompleteFloat result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteFloat()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(IncompleteFloat right, out IncompleteFloat result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteFloat()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }
    }
}