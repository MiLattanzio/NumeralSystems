using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;

namespace NumeralSystems.Net.Type.Incomplete
{
    public class IncompleteUShort : IIRregularOperable<IncompleteUShort, UShort, ushort>
    {
        private bool?[] _binary;

        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8 * sizeof(ushort)).Select(x => x as bool?).ToArray();
            internal set
            {
                if (null == value)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8 * sizeof(ushort)).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= (8 * sizeof(ushort)))
                    {
                        _binary = value.Take(8 * sizeof(ushort)).ToArray();
                    }
                    else
                    {
                        _binary = System.Linq.Enumerable.Repeat(false, (8 * sizeof(ushort)) - value.Length)
                            .Select(x => x as bool?)
                            .Concat(value).ToArray();
                    }
                }
            }
        }
        public bool IsComplete => Binary.All(x => x != null);
        public int Permutations => Sequence.PermutationsCount(2, Binary.Count(x => x is null), true);

        public UShort this[int value]
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
                return new UShort
                {
                    Binary = resultBinary
                };
            }
        }

        public IEnumerable<UShort> Enumerable => System.Linq.Enumerable.Range(0, Permutations).Select(x => this[x]);

        public IncompleteByte[] ByteArray => IncompleteByteArray.ArrayOf(Binary);

        public IncompleteByte[] ToByteArray() => IncompleteByteArray.ArrayOf(Binary.Select(x => x).ToArray());

        public string ToString(string missingSeparator = "*") => string.Join(string.Empty, Binary.Group(8).Select(x => x.Reverse().ToArray()).SelectMany(x => x).Select(x => null == x ? missingSeparator : (x.Value ? 1 : 0).ToString()));

        public IncompleteUShort Or(UShort other) => new()
        {
            Binary = Binary.Or(other.Binary)
        };

        public bool Contains(UShort value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        public bool Contains(IncompleteUShort value)
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

        public IncompleteUShort Not() => new()
        {
            Binary = Binary.Not()
        };

        public IncompleteUShort Xor(IncompleteUShort other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteUShort Xor(UShort other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteUShort And(IncompleteUShort other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteUShort And(UShort other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteUShort Or(IncompleteUShort other) => new()
        {
            Binary = Binary.Or(other.Binary)
        };

        public bool ReverseAnd(UShort right, out IncompleteUShort result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteUShort()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseAnd(IncompleteUShort right, out IncompleteUShort result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteUShort()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(UShort right, out IncompleteUShort result)
        {
            if (!Binary.CanReverseOr(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteUShort()
            {
                Binary = Binary.ReverseOr(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(IncompleteUShort right, out IncompleteUShort result)
        {
            if (!Binary.CanReverseOr(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteUShort()
            {
                Binary = Binary.ReverseOr(right.Binary)
            };
            return true;
        }
    }
}