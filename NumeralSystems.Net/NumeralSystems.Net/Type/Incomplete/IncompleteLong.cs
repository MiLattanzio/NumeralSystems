using System;
using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = NumeralSystems.Net.Utils.Convert;

namespace NumeralSystems.Net.Type.Incomplete
{
    public class IncompleteLong : IIRregularOperable<IncompleteLong, Long, long, ulong>
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
        public ulong Permutations => Sequence.PermutationsCount(2, Sequence.CountToULong(Binary.Where(x => x is null)), true);

        public Long this[ulong value]
        {
            get
            {
                var binary = Binary;
                var valueBinary = value.ToBoolArray();
                var resultBinary = new bool[binary.Length];
                var lastValueBinaryIndex = 0;
                for (var i = 0; i < binary.Length; i++)
                {
                    for (var i1 = lastValueBinaryIndex; i1 < valueBinary.Length; i1++)
                    {
                        if (binary[i] is null)
                        {
                            resultBinary[i] = valueBinary[i1];
                            lastValueBinaryIndex = i1 + 1;
                            break;
                        }
                        resultBinary[i] = binary[i] ?? false;
                    }
                }
                return new Long()
                {
                    Binary = resultBinary
                };
            }
        }

        public IEnumerable<Long> Enumerable => Sequence.Range(0, Permutations).Select(x => this[x]);

        public IncompleteByte[] ByteArray => IncompleteByteArray.ArrayOf(Binary);

        public IncompleteByte[] ToByteArray() => IncompleteByteArray.ArrayOf(Binary.Select(x => x).ToArray());
        public string ToString(string missingSeparator = "*") => string.Join(string.Empty, Binary.Group(8).Select(x => x.Reverse().ToArray()).SelectMany(x => x).Select(x => null == x ? missingSeparator : (x.Value ? 1 : 0).ToString()));

        public IncompleteLong Or(Long other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public bool Contains(Long value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        public bool Contains(IncompleteLong value)
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


        public IncompleteLong Not() => new()
        {
            Binary = Binary.Select(x => !x).ToArray()
        };

        public IncompleteLong Xor(IncompleteLong other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteLong Xor(Long other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteLong And(IncompleteLong other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteLong And(Long other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteLong Or(IncompleteLong other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public bool ReverseAnd(Long right, out IncompleteLong result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteLong()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseAnd(IncompleteLong right, out IncompleteLong result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteLong()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(Long right, out IncompleteLong result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteLong()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(IncompleteLong right, out IncompleteLong result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteLong()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }
    }
}