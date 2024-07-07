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
    public class IncompleteByte : IIRregularOperable<IncompleteByte, NumeralByte, byte>
    {
        private bool?[] _binary;

        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8).Select(x => x as bool?).ToArray();
            set
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

        public NumeralByte this[int value] => Byte.FromBinary(((byte)value).ToBoolArray());

        public IEnumerable<NumeralByte> Bytes => System.Linq.Enumerable.Range(0, Permutations).Select(x => this[x]);

        public IEnumerable<NumeralByte> Enumerable => Bytes;

        public IncompleteByteArray ByteArray => new()
        {
            Binary = Binary
        };

        public IncompleteByteArray ToByteArray() => new()
        {
            Binary = Binary.Select(x => x).ToArray()
        };

        public string ToString(string missingSeparator = "*") => string.Join(string.Empty, Binary.Group(8).Select(x => x.Reverse().ToArray()).SelectMany(x => x).Select(x => null == x ? missingSeparator : (x.Value ? 1 : 0).ToString()));

        public IncompleteByte Or(NumeralByte other)=> new()
        {
            Binary = Binary.Or(other.Binary)
        };

        public bool Contains(NumeralByte value)
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

        public IncompleteByte And(IncompleteByte other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteByte And(NumeralByte other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteByte Or(IncompleteByte other) => new()
        {
            Binary = Binary.Or(other.Binary)
        };

        public bool ReverseAnd(NumeralByte right, out IncompleteByte result)
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

        public bool ReverseOr(NumeralByte right, out IncompleteByte result)
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