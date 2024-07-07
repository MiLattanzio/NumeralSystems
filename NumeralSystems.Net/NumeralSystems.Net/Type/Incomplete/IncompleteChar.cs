using System;
using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Interface;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;
using Char = NumeralSystems.Net.Type.Base.Char;
using Math = NumeralSystems.Net.Utils.Math;
using Convert = NumeralSystems.Net.Utils.Convert;

namespace NumeralSystems.Net.Type.Incomplete
{
    public class IncompleteChar : IIRregularOperable<IncompleteChar, Char, char>
    {
        private bool?[] _binary;

        public bool?[] Binary
        {
            get => _binary ?? System.Linq.Enumerable.Repeat(false, 8 * sizeof(char)).Select(x => x as bool?).ToArray();
            set
            {
                if (null == value)
                {
                    _binary = System.Linq.Enumerable.Repeat(false, 8 * sizeof(char)).Select(x => x as bool?).ToArray();
                }
                else
                {
                    if (value.Length >= (8 * sizeof(char)))
                    {
                        _binary = value.Take(8 * sizeof(char)).ToArray();
                    }
                    else
                    {
                        _binary = System.Linq.Enumerable.Repeat(false, (8 * sizeof(char)) - value.Length)
                            .Select(x => x as bool?)
                            .Concat(value).ToArray();
                    }
                }
            }
        }
        public bool IsComplete => Binary.All(x => x != null);
        public int Permutations => Sequence.PermutationsCount(2, Binary.Count(x => x is null), true);

        public Char this[int value] => Char.FromBinary(value.ToBoolArray());

        public IEnumerable<Char> Enumerable => System.Linq.Enumerable.Range(0, Permutations).Select(x => this[x]);

        public IncompleteByteArray ByteArray => new()
        {
            Binary = Binary
        };

        public IncompleteByteArray ToByteArray() => new()
        {
            Binary = Binary.Select(x => x).ToArray()
        };

        public string ToString(string missingSeparator = "*") => string.Join(string.Empty, Binary.Group(8).Select(x => x.Reverse().ToArray()).SelectMany(x => x).Select(x => null == x ? missingSeparator : (x.Value ? 1 : 0).ToString()));

        public IncompleteChar Or(Char other) => new()
        {
            Binary = Binary.Or(other.Binary)
        };

        public bool Contains(Char value)
        {
            var bytes = Binary;
            var bytesBinary = value.Binary;
            return !bytes.Where((t, i) => t is not null && t != bytesBinary[i]).Any();
        }

        public bool Contains(IncompleteChar value)
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

        public IncompleteChar Not() => new()
        {
            Binary = Binary.Not()
        };

        public IncompleteChar Xor(IncompleteChar other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteChar Xor(Char other) => new()
        {
            Binary = Binary.Xor(other.Binary)
        };

        public IncompleteChar And(IncompleteChar other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteChar And(Char other) => new()
        {
            Binary = Binary.And(other.Binary)
        };

        public IncompleteChar Or(IncompleteChar other) => new()
        {
            Binary = Binary.Or(other.Binary)
        };

        public bool ReverseAnd(Char right, out IncompleteChar result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteChar()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseAnd(IncompleteChar right, out IncompleteChar result)
        {
            if (!Binary.CanReverseAnd(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteChar()
            {
                Binary = Binary.ReverseAnd(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(Char right, out IncompleteChar result)
        {
            if (!Binary.CanReverseOr(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteChar()
            {
                Binary = Binary.ReverseOr(right.Binary)
            };
            return true;
        }

        public bool ReverseOr(IncompleteChar right, out IncompleteChar result)
        {
            if (!Binary.CanReverseOr(right.Binary))
            {
                result = null;
                return false;
            }
            result = new IncompleteChar()
            {
                Binary = Binary.ReverseOr(right.Binary)
            };
            return true;
        }
    }
}