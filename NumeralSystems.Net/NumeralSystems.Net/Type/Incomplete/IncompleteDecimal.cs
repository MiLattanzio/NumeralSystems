using System;
using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Utils;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = NumeralSystems.Net.Utils.Convert;
using Decimal = NumeralSystems.Net.Type.Base.Decimal;

namespace NumeralSystems.Net.Type.Incomplete
{
    public class IncompleteDecimal: IIRregularOperable<IncompleteDecimal, Decimal, decimal, ulong>
    {
        private bool?[] _binary;
        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8 * sizeof(decimal)).Select(x => x as bool?).ToArray();
            internal set
            {
                if (null == value)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8 * sizeof(decimal)).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= (8 * sizeof(decimal)))
                    {
                        _binary = value.Take(8 * sizeof(decimal)).ToArray();
                    }
                    else
                    {
                        _binary = System.Linq.Enumerable.Repeat(false, (8 * sizeof(decimal)) - value.Length).Select(x => x as bool?)
                            .Concat(value).ToArray();
                    }
                }
            }
        }
        
        public bool IsComplete => Binary.All(x => x != null);
        public ulong Permutations => Sequence.PermutationsCount(2, Sequence.CountToULong(Binary.Where(x => x is null)), true);

        public Decimal this[ulong value]
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
                        resultBinary[i] = binary[i].Value;
                        break;
                    }
                }
                return new Decimal()
                {
                    Binary = resultBinary
                };
            }
        }
        
        public IEnumerable<Decimal> Enumerable => Sequence.Range(0, Permutations).Select(x => this[x]);

        public IncompleteByte[] ByteArray => IncompleteByteArray.ArrayOf(Binary);
        
        public IncompleteByte[] ToByteArray() => IncompleteByteArray.ArrayOf(Binary.Select(x => x).ToArray());
        
        public string ToString(string missingSeparator = "*") => string.Join(string.Empty, Binary.Group(8).Select(x => x.Reverse().ToArray()).SelectMany(x => x).Select(x => null == x ? missingSeparator : (x.Value ? 1 : 0).ToString()));
        public IncompleteDecimal Or(Decimal other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public bool Contains(Decimal value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        public bool Contains(IncompleteDecimal value)
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

        public IncompleteDecimal Not() => new()
        {
            Binary = Binary.Not()
        };

        public IncompleteDecimal Xor(IncompleteDecimal other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteDecimal Xor(Decimal other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteDecimal And(IncompleteDecimal other) => new() {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteDecimal And(Decimal other) => new() {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteDecimal Or(IncompleteDecimal other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public bool ReverseAnd(Decimal right, out IncompleteDecimal result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteDecimal()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseAnd(IncompleteDecimal right, out IncompleteDecimal result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteDecimal()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(Decimal right, out IncompleteDecimal result)
        {
            if (!Binary.CanReverseOr(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteDecimal()
            {
                Binary = Binary.ReverseOr(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(IncompleteDecimal right, out IncompleteDecimal result)
        {
            if (!Binary.CanReverseOr(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteDecimal()
            {
                Binary = Binary.ReverseOr(right.Binary)
            };
            return true;
        }
    }
}