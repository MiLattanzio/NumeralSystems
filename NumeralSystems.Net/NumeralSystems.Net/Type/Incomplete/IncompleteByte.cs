using System;
using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;
using Byte = NumeralSystems.Net.Type.Base.Byte;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = NumeralSystems.Net.Utils.Convert;

namespace NumeralSystems.Net.Type.Incomplete
{
    public class IncompleteByte : IIRregularOperable<IncompleteByte, Byte, byte>
    {
        private bool?[] _binary;

        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8).Select(x => x as bool?).ToArray();
            internal set
            {
                if (null == value)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= 8)
                    {
                        _binary = value.Take(8).ToArray();
                    }
                    else
                    {
                        _binary = System.Linq.Enumerable.Repeat(false, 8 - value.Length).Select(x => x as bool?)
                            .Concat(value).ToArray();
                    }
                }
            }
        }

        public int Permutations => Sequence.PermutationsCount(2, Binary.Count(x => x is null), true);

        public bool IsComplete => Binary.All(x => x != null);

        public Byte this[int value]
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
                return new Byte()
                {
                    Binary = resultBinary
                };
            }
        }

        private IEnumerable<Byte> Bytes => System.Linq.Enumerable.Range(0, Permutations).Select(x => this[x]);

        public IEnumerable<Byte> Enumerable => Bytes;

        public IncompleteByte[] ByteArray => IncompleteByteArray.ArrayOf(Binary);

        public IncompleteByte[] ToByteArray() => IncompleteByteArray.ArrayOf(Binary.Select(x => x).ToArray());

        public string ToString(string missingSeparator = "*") => string.Join(string.Empty, Binary.Group(8).Select(x => x.Reverse().ToArray()).SelectMany(x => x).Select(x => null == x ? missingSeparator : (x.Value ? 1 : 0).ToString()));

        public IncompleteByte Or(Byte other)=> new()
        {
            Binary = Binary.Or(other.Binary)
        };

        public bool Contains(Byte value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        public bool Contains(IncompleteByte value)
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

        public IncompleteByte Not() => new()
        {
            Binary = Binary.Not()
        };

        public IncompleteByte Xor(IncompleteByte other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteByte Xor(Byte other) => new ()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteByte And(IncompleteByte other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteByte And(Byte other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteByte Or(IncompleteByte other) => new()
        {
            Binary = Binary.Or(other.Binary)
        };

        public bool ReverseAnd(Byte right, out IncompleteByte result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteByte()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseAnd(IncompleteByte right, out IncompleteByte result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteByte()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(Byte right, out IncompleteByte result)
        {
            if (!Binary.CanReverseOr(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteByte()
            {
                Binary = Binary.ReverseOr(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(IncompleteByte right, out IncompleteByte result)
        {
            if (!Binary.CanReverseOr(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteByte()
            {
                Binary = Binary.ReverseOr(right.Binary)
            };
            return true;
        }
    }
}