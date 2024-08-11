using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;

namespace NumeralSystems.Net.Type.Incomplete
{
    public class IncompleteUInt: IIRregularOperable<IncompleteUInt, UInt, uint, uint>
    {
        private bool?[] _binary;

        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8 * sizeof(uint)).Select(x => x as bool?).ToArray();
            set
            {
                if (null == value)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8 * sizeof(uint)).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= (8 * sizeof(uint)))
                    {
                        _binary = value.Take(8 * sizeof(uint)).ToArray();
                    }
                    else
                    {
                        _binary = System.Linq.Enumerable.Repeat(false, (8 * sizeof(uint)) - value.Length).Select(x => x as bool?)
                            .Concat(value).ToArray();
                    }
                }
            }
        }
        public bool IsComplete => Binary.All(x => x != null);
        public uint Permutations => Sequence.PermutationsCount(2, Sequence.CountToUInt(Binary.Select(x => x is null)), true);
        
        public UInt this[uint value]
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
                return new UInt()
                {
                    Binary = resultBinary
                };

            }
        }
        
        public IEnumerable<UInt> Enumerable => Sequence.Range(0, Permutations).Select(x => this[x]);

        public IncompleteByte[] ByteArray => IncompleteByteArray.ArrayOf(Binary);
        
        public IncompleteByte[] ToByteArray() => IncompleteByteArray.ArrayOf(Binary.Select(x => x).ToArray());

        public override string ToString() => ToString("*");

        public string ToString(string missingSeparator) => string.Join(string.Empty, Binary.Group(8).Select(x => x.Reverse().ToArray()).SelectMany(x => x).Select(x => null == x ? missingSeparator : (x.Value ? 1 : 0).ToString()));
        public IncompleteUInt Or(UInt other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public bool Contains(UInt value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        public bool Contains(IncompleteUInt value)
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

        public IncompleteUInt Not() => new()
        {
            Binary = Binary.Select(x => !x).ToArray()
        };

        public IncompleteUInt Xor(IncompleteUInt other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteUInt Xor(UInt other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteUInt And(IncompleteUInt other) => new() {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteUInt And(UInt other) => new() {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteUInt Or(IncompleteUInt other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public bool ReverseAnd(UInt right, out IncompleteUInt result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteUInt()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseAnd(IncompleteUInt right, out IncompleteUInt result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteUInt()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(UInt right, out IncompleteUInt result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteUInt()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(IncompleteUInt right, out IncompleteUInt result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteUInt()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }
    }
}