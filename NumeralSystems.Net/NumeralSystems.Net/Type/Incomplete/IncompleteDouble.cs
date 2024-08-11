using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Utils;
using Convert = NumeralSystems.Net.Utils.Convert;
using Math = NumeralSystems.Net.Utils.Math;
using Double = NumeralSystems.Net.Type.Base.Double;

namespace NumeralSystems.Net.Type.Incomplete
{
    public class IncompleteDouble: IIRregularOperable<IncompleteDouble, Double, double, int>
    {
        private bool?[] _binary;
        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8 * sizeof(double)).Select(x => x as bool?).ToArray();
            set
            {
                if (null == value)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8 * sizeof(double)).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= (8 * sizeof(double)))
                    {
                        _binary = value.Take(8 * sizeof(double)).ToArray();
                    }
                    else
                    {
                        _binary = System.Linq.Enumerable.Repeat(false, (8 * sizeof(double)) - value.Length).Select(x => x as bool?)
                            .Concat(value).ToArray();
                    }
                }
            }
        }
        
        public bool IsComplete => Binary.All(x => x != null);
        public int Permutations => Sequence.PermutationsCount(2, Binary.Count(x => x is null), true);

        public Double this[int value]
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
                return new Double()
                {
                    Binary = resultBinary
                };
            }
        }
        
        public IEnumerable<Double> Enumerable => System.Linq.Enumerable.Range(0, Permutations).Select(x => this[x]);

        public IncompleteByte[] ByteArray => IncompleteByteArray.ArrayOf(Binary);
        
        public IncompleteByte[] ToByteArray() => IncompleteByteArray.ArrayOf(Binary.Select(x => x).ToArray());
        
        public string ToString(string missingSeparator = "*") => string.Join(string.Empty, Binary.Group(8).Select(x => x.Reverse().ToArray()).SelectMany(x => x).Select(x => null == x ? missingSeparator : (x.Value ? 1 : 0).ToString()));
        public IncompleteDouble Or(Double other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public bool Contains(Double value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        public bool Contains(IncompleteDouble value)
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

        public IncompleteDouble Not() => new()
        {
            Binary = Binary.Not()
        };

        public IncompleteDouble Xor(IncompleteDouble other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteDouble Xor(Double other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteDouble And(IncompleteDouble other) => new() {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteDouble And(Double other) => new() {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteDouble Or(IncompleteDouble other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public bool ReverseAnd(Double right, out IncompleteDouble result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteDouble()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseAnd(IncompleteDouble right, out IncompleteDouble result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteDouble()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(Double right, out IncompleteDouble result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteDouble()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(IncompleteDouble right, out IncompleteDouble result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteDouble()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }
    }
}