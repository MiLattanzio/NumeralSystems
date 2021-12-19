using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NumeralSystems.Net.Utils;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable HeapView.ObjectAllocation.Evident
// ReSharper disable MemberCanBePrivate.Global
namespace NumeralSystems.Net
{
    [SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NumeralSystem<TElement>
    {
        public IList<TElement> Identity { get; }
        private Func<string, Numeral<TElement>> _stringParse;

        public Func<string, Numeral<TElement>> StringParse
        {
            get => _stringParse;
            set
            {
                if (null != value) _stringParse = value;
            }
        }

        private Func<int, bool, List<TElement>, List<TElement>> _intIndexer = (index, positive, suggestedOutput) =>
            suggestedOutput;

        public Func<int, bool, List<TElement>, List<TElement>> IntIndexer
        {
            get => _intIndexer;
            // ReSharper disable once UnusedMember.Global
            set
            {
                if (null != value)
                {
                    _intIndexer = value;
                }
            }
        }

        // ReSharper disable once HeapView.PossibleBoxingAllocation
        private Func<List<TElement>, bool, string> _stringConverter;

        public Func<List<TElement>, bool, string> StringConverter
        {
            get => _stringConverter;
            set
            {
                if (null != value)
                {
                    _stringConverter = value;
                }
            }
        }

        public int Size => Identity.Count;

        private string _negativeSign = "-";

        public string NegativeSign
        {
            get => _negativeSign;
            set
            {
                if (null != value) _negativeSign = value;
            }
        }

        private string _separator;

        public string Separator
        {
            get => _separator;
            set
            {
                // ReSharper disable once HeapView.PossibleBoxingAllocation
                // ReSharper disable once HeapView.DelegateAllocation
                // ReSharper disable once HeapView.ClosureAllocation
                if (null != value && !Identity.Any(x => value.Equals(x.ToString()))) _separator = value;
            }
        }

        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        public NumeralSystem(HashSet<TElement> identity, string separator = ";")
        {
            if (null == identity || identity.Count == 0) throw new Exception("Identity must contain an element");
            Identity = identity.ToList();
            // ReSharper disable once HeapView.DelegateAllocation
            // ReSharper disable once HeapView.PossibleBoxingAllocation
            if (null == separator || Identity.Any(x => separator.Equals(x.ToString())))
                throw new Exception("Invalid separator");
            Separator = separator;
            // ReSharper disable once HeapView.DelegateAllocation
            // ReSharper disable once HeapView.PossibleBoxingAllocation
            StringConverter = (val, pos) =>
                val.Count == 0
                    ? string.Empty
                    : $"{(pos ? "" : NegativeSign)}{string.Join(Separator, val.ConvertAll(x => x.ToString()))}";
            StringParse = (val) =>
            {
                if (null == val) return new Numeral<TElement>(this, new List<TElement>() { Identity[0] });
                var input = val.Clone() as string;
                if (string.IsNullOrEmpty(val)) return this[0];
                var positive = !val.StartsWith(NegativeSign);
                if (!positive)
                {
                    var idx = val.IndexOf(NegativeSign, StringComparison.Ordinal);
                    input = val.Substring(idx + 1);
                }
                // ReSharper disable once HeapView.PossibleBoxingAllocation

                var identities = Identity.ToList().ToDictionary(x => x.ToString(), x => x);
                var outputKeys = input.SplitAndKeep(identities.Keys.ToArray());
                var output = outputKeys.ConvertAll(x =>
                {
                    if (outputKeys.Contains(x)) return identities[x];
                    throw new Exception($"Invalid mapping for value '{x}' of '{val}'");
                });
                var numeric = new Numeral<TElement>(this, output, positive);
                if (!DoubleCheckParsedValue) return numeric;
                if (val != numeric.ToString())
                {
                    throw new Exception($"String '{numeric}' is not equal to input '{val}'");
                }

                var integer = numeric.Integer;
                
                var numericInteger = this[integer];
                if (numericInteger.ToString() != numeric.ToString())
                {
                    throw new Exception($"String '{numeric}' obtained from input '{val}' is not equal to '{numericInteger}' obtained from It's combination count ({integer})");
                }

                return numeric;
            };
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public bool DoubleCheckParsedValue { get; set; }

        // ReSharper disable once HeapView.DelegateAllocation
        public bool Contains(IList<TElement> value) => (null != value && value.ToList().All(Identity.Contains));

        public Numeral<TElement> this[int index]
        {
            get
            {
                var positive = index > 0;
                var value = Math.Abs(index);
                IEnumerable<TElement> result = new List<TElement>();
                while (value != 0)
                {
                    value = Math.DivRem(value, Size, out var remainder);
                    result = result.Prepend(Identity[remainder]);
                }

                var resultList = IntIndexer(index, positive, result.ToList());
                return new Numeral<TElement>(this, resultList, positive);
            }
        }
    }
}