using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NumeralSystems.Net.Utils;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable HeapView.ObjectAllocation.Evident
// ReSharper disable MemberCanBePrivate.Global
namespace NumeralSystems.Net
{
    [SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NumeralSystem
    {
        public IList<string> Identity { get; }
        private Func<string, Numeral> _stringParse;
        public Func<string, Numeral> StringParse
        {
            get => _stringParse;
            set
            {
                if (null != value) _stringParse = value;
            }
        }

        private Func<ulong, bool, List<string>, List<string>> _uLongIndexer = (index, positive, suggestedOutput) => suggestedOutput;
        public Func<ulong, bool, List<string>, List<string>> LongIndexer
        {
            get => _uLongIndexer;
            // ReSharper disable once UnusedMember.Global
            set
            {
                if (null != value)
                {
                    _uLongIndexer = value;
                }
            }
        }

        // ReSharper disable once HeapView.PossibleBoxingAllocation
        private Func<(List<string> Integral, List<string> Fractional, bool IsPositive), string> _stringConverter;

        public Func<(List<string> Integral, List<string> Fractional, bool IsPositive), string> StringConverter
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

        public static string BaseNegativeSign => BaseCultureInfo.NumberFormat.NegativeSign;

        public string NegativeSign
        {
            get => CultureInfo.NumberFormat.NegativeSign;
            set
            {
                if (null != value && !Identity
                                .Concat(new []{Separator})
                                .Concat(new []{NumberDecimalSeparator})
                                .Any(x => value.Equals(x.ToString()))) CultureInfo.NumberFormat.NegativeSign = value;
            }
        }
        public static CultureInfo BaseCultureInfo => CultureInfo.InvariantCulture;

        private CultureInfo _cultureInfo = BaseCultureInfo;
        public CultureInfo CultureInfo
        {
            get => _cultureInfo;
            set
            {
                if (null != value) _cultureInfo = value;
            }
        }

        public static string BaseNumberDecimalSeparator => BaseCultureInfo.NumberFormat.NumberDecimalSeparator;

        public string NumberDecimalSeparator
        {
            get => CultureInfo.NumberFormat.NumberDecimalSeparator;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && !Identity
                        .Concat(new []{Separator})
                        .Concat(new []{NegativeSign})
                        .Any(x => value.Equals(x.ToString())))
                    CultureInfo.NumberFormat.NumberDecimalSeparator = value;
            }
        }

        public static string BaseSeparator => Convert.ToString(Numeral.System.Characters.Semicolon);
        private string _separator;
        public string Separator
        {
            get => _separator;
            set
            {
                if (null != value && !Identity
                        .Concat(new []{NumberDecimalSeparator})
                        .Concat(new []{NegativeSign})
                        .Any(x => value.Equals(x.ToString()))) _separator = value;
            }
        }

        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        public NumeralSystem(HashSet<string> identity, string separator = ";")
        {
            if (null == identity || identity.Count == 0) throw new Exception("Identity must contain an element");
            Identity = identity.ToList();
            if (null == separator || Identity.Any(x => separator.Equals(x.ToString())))
                throw new Exception("Invalid separator");
            Separator = separator;
            StringConverter = number =>
            {
                var (integral, fractional, isPositive) = number;
                integral ??= new();
                fractional ??= new();
                var integralString = integral.Count == 0 ? Identity[0] : string.Join(Separator, integral.ConvertAll(x => x.ToString()));
                var fractionalString = fractional.Count == 0 ? Identity[0] : string.Join(Separator, fractional.ConvertAll(x => x.ToString()));
                if ((integral.Count == 0 || integral.All(x => x.Equals(Identity[0]))) && (fractional.Count == 0 || fractional.All(x => x.Equals(Identity[0]))))
                    return integralString;
                return $"{(isPositive ? string.Empty : NegativeSign)}{integralString}{(fractional.Count == 0 ? string.Empty :$"{NumberDecimalSeparator}{fractionalString}")}";
            };
            StringParse = val =>
            {
                if (null == val) return new Numeral(this, new List<string> { Identity[0] });
                var input = val.Clone() as string ?? string.Empty;
                if (string.IsNullOrEmpty(val)) return this[0];
                var positive = true;
                if (val.StartsWith(NegativeSign))
                {
                    positive = false;
                    var idx = val.IndexOf(NegativeSign, StringComparison.Ordinal);
                    input = val[(idx + 1)..];
                }
                else if (Numeral.System.Characters.Minus.ToString() == NegativeSign && val.StartsWith(Numeral.System.Characters.Minus)) //Wierd patch
                {
                    positive = false;
                    var idx = val.IndexOf(Numeral.System.Characters.Minus, StringComparison.Ordinal);
                    input = val[(idx + 1)..];
                }
                
                // ReSharper disable once HeapView.PossibleBoxingAllocation

                var floatString = input.Split(NumberDecimalSeparator);
                var integralKeys =  !string.IsNullOrEmpty(Separator) && floatString[0].Contains(Separator) ? floatString[0].Split(Separator).ToList() : floatString[0].SplitAndKeep(Identity.ToArray());
                integralKeys.ForEach(x =>
                {
                    if (!integralKeys.Contains(x))
                        throw new Exception($"Invalid mapping for value '{x}' of '{val}'");
                });
                var fractionalString = (floatString.Length == 1 ? string.Empty : floatString[1]);
                var fractionalKeys = !string.IsNullOrEmpty(Separator) && fractionalString.Contains(Separator) ? fractionalString.Split(Separator).ToList() : string.IsNullOrEmpty(fractionalString) ? new List<string>() : fractionalString.SplitAndKeep(Identity.ToArray()).Select(x => x.Split(Separator)[0]).ToList();;
                fractionalKeys.ForEach(x =>
                {
                    if (!fractionalKeys.Contains(x))
                        throw new Exception($"Invalid mapping for value '{x}' of '{val}'");
                });
                var numeric = new Numeral(this, integralKeys, fractionalKeys, positive: positive);
#if DEBUG
                try
                {
                    if (val != numeric.ToString())
                    {
                        throw new Exception($"String '{numeric}' is not equal to input '{val}'");
                    }

                    var f = numeric.Decimal;
                
                    var numericInteger = this[f];
                    if (numericInteger.ToString() != numeric.ToString())
                    {
                        throw new Exception($"String '{numeric}' obtained from input '{val}' is not equal to '{numericInteger}' obtained from It's combination count ({f})");
                    } 
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                      
#endif
                return numeric;
            };
        }
        
        public static int DigitsInBase(int number, int numeralBase)
        {
            return (int)Math.Ceiling(Math.Log(number + 1, numeralBase));
        }
        
        public static long DigitsInBase(long number, int numeralBase)
        {
            return (long)Math.Ceiling(Math.Log(number + 1, numeralBase));
        }
        
        public static ulong DigitsInBase(ulong number, int numeralBase)
        {
            return (ulong)Math.Ceiling(Math.Log(number + 1, numeralBase));
        }
        
        public bool Contains(IList<string> value) => (null != value && value.ToList().All(Identity.Contains));
        
        private List<string> PositiveIntegralOf(ulong value)
        {
            IEnumerable<string> result = new List<string>();
            while (value != 0)
            {
                var remainder = (int)(value % (ulong)Size);
                value /= (ulong)Size;
                result = result.Prepend(Identity[remainder]);
            }
            return LongIndexer(value, true, result.ToList());
        }

        public Numeral this[int index] => new (this, PositiveIntegralOf((ulong)Math.Abs(index)), positive: index > 0);
        
        public Numeral this[double index]
        {
            get
            {
                var zero = Numeral.System.Characters.Numbers.First();
                var integral = PositiveIntegralOf((ulong)Math.Abs(index));
                var cultureInfo = (CultureInfo)CultureInfo.Clone();
                cultureInfo.NumberFormat.NumberGroupSeparator = string.Empty;
                var dSplitString = index.ToString("N19", cultureInfo)
                                    .Split(NumberDecimalSeparator)
                                    .ToArray();
                
                if (dSplitString.Length <= 1) return new(this, integral, new List<string>(), index > 0);
                if (dSplitString[1].All(x => x == zero)) return new(this, integral, new List<string>(), index > 0);
                var fractionalIntString = dSplitString[1];
                var frontZeros = 0;
                foreach (var t in fractionalIntString)
                {
                    if (t == zero) frontZeros++;
                    else break;
                }
                //reverse the string
                var backZeros = 0;
                foreach (var t in fractionalIntString.Reverse())
                {
                    if (t == zero) backZeros++;
                    else break;
                }
                //remove last backZeros from fractionalIntString 0.0101882201
                if (backZeros > 0)
                    fractionalIntString = fractionalIntString[..^backZeros];
                var fractionalInt = fractionalIntString.Length == 0 ? 0 : ulong.Parse(fractionalIntString);
                var fractional = PositiveIntegralOf(fractionalInt);
                fractional = Enumerable.Repeat(Identity[0], frontZeros).Concat(fractional).ToList();
                return new (this, integral, fractional, index > 0);
            }
        }
        
        public Numeral this[decimal index]
        {
            get
            {
                var zero = Numeral.System.Characters.Numbers.First();
                var integral = PositiveIntegralOf((ulong)Math.Abs(index));
                var cultureInfo = (CultureInfo)CultureInfo.Clone();
                cultureInfo.NumberFormat.NumberGroupSeparator = string.Empty;
                var dSplitString = index.ToString("N19", cultureInfo)
                    .Split(NumberDecimalSeparator)
                    .ToArray();
                
                if (dSplitString.Length <= 1) return new(this, integral, new List<string>(), index > 0);
                if (dSplitString[1].All(x => x == zero)) return new(this, integral, new List<string>(), index > 0);
                var fractionalIntString = dSplitString[1];
                var frontZeros = 0;
                foreach (var t in fractionalIntString)
                {
                    if (t == zero) frontZeros++;
                    else break;
                }
                //reverse the string
                var backZeros = 0;
                foreach (var t in fractionalIntString.Reverse())
                {
                    if (t == zero) backZeros++;
                    else break;
                }
                //remove last backZeros from fractionalIntString 0.0101882201
                if (backZeros > 0)
                    fractionalIntString = fractionalIntString[..^backZeros];
                var fractionalInt = fractionalIntString.Length == 0 ? 0 : ulong.Parse(fractionalIntString);
                var fractional = PositiveIntegralOf(fractionalInt);
                fractional = Enumerable.Repeat(Identity[0], frontZeros).Concat(fractional).ToList();
                return new (this, integral, fractional, index > 0);
            }
        }
    }
}