using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
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
        public static string BaseNegativeSign => BaseCultureInfo.NumberFormat.NegativeSign;
        public static CultureInfo BaseCultureInfo => CultureInfo.InvariantCulture;
        public static string BaseNumberDecimalSeparator => BaseCultureInfo.NumberFormat.NumberDecimalSeparator;
        public static string BaseSeparator => Convert.ToString(Numeral.System.Characters.Semicolon);
        public static int DigitsInBase(int number, int numeralBase)
        {
            return (int) Math.Ceiling(Math.Log(number + 1, numeralBase));
        }

        public static long DigitsInBase(long number, int numeralBase)
        {
            return (long) Math.Ceiling(Math.Log(number + 1, numeralBase));
        }

        public static ulong DigitsInBase(ulong number, int numeralBase)
        {
            return (ulong) Math.Ceiling(Math.Log(number + 1, numeralBase));
        }
        
        private IList<string> _identity;
        
        /// <summary>
        /// Identity of the numeral system.
        /// <para>For example, for base 10 it is 0, 1, 2, 3, 4, 5, 6, 7, 8, 9</para>
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public IList<string> Identity
        {
            get => _identity;
            set
            {
                var val = value?.Distinct().ToList();
                _identity = val ?? throw new InvalidOperationException("Identity cannot be null");
            }
        }

        /// <summary>
        /// Size of the numeral system.
        /// <para>For example, for base 10 it is 10</para>
        /// </summary>
        public int Size => Identity.Count;
        
        /// <summary>
        /// Negative sign of the numeral system, inherited from the culture info.
        /// <para>For example, for base 10 it is -</para>
        /// </summary>
        public string NegativeSign => CultureInfo.NumberFormat.NegativeSign;
        
        private CultureInfo _cultureInfo = BaseCultureInfo;

        /// <summary>
        /// Culture info of the numeral system.
        /// Note that it is used only for parsing and formatting.
        /// </summary>
        public CultureInfo CultureInfo
        {
            get => _cultureInfo;
            set
            {
                if (null != value) _cultureInfo = value;
            }
        }
        
        /// <summary>
        /// Floating point separator of the numeral system, inherited from the culture info.
        /// <para>For example, for base 10 it is .</para>
        /// </summary>
        public string NumberDecimalSeparator => CultureInfo.NumberFormat.NumberDecimalSeparator;

        private string _separator;
        
        /// <summary>
        /// Separator of the numeral system.
        /// Used to identify the digits of the numeral system when the length of the digits is not equal.
        /// </summary>
        public string Separator
        {
            get => _separator;
            set
            {
                if (null != value && !Identity
                        .Concat(new[] {NumberDecimalSeparator})
                        .Concat(new[] {NegativeSign})
                        .Any(x => value.Equals(x.ToString()))) _separator = value;
            }
        }
        
        /// <summary>
        /// If true, the parsing will skip the unknown values during the parsing, otherwise they will be replaced with zeros.
        /// Example: for base 10, the value 1;1 will be parsed as 11 if true, otherwhise it will be parsed as 101.
        /// </summary>
        public bool SkipUnknownValues { get; set; }
        
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        public NumeralSystem(HashSet<string> identity, string separator = null)
        {
            if (null == identity || identity.Count == 0) throw new Exception("Identity must contain an element");
            Identity = identity.ToList();
            var shouldHaveSeparator = false;
            if (Identity.Select(x => x.Length).Distinct().Count() > 1 )
            {
                if (null == separator)
                    shouldHaveSeparator = true;
            }
            if (identity.Contains(separator) || shouldHaveSeparator)
            {
                separator = Numeral.System.Characters.All
                    .Select(x => x.ToString())
                    .Where(x => !identity.Contains(x))
                    .Where(x => !x.Equals(NumberDecimalSeparator))
                    .FirstOrDefault(x => !x.Equals(NegativeSign));
                if (null == separator)
                    throw new Exception("Cannot find a valid number separator");
            }
            Separator = separator;
        }

        public bool TrySplitNumberIndices(string val, out (bool positive, List<int> integralIndices, List<int> fractionalIndices, string integral, string fractional) result)
        {
            var sucess = true;
            result = (true, new List<int>(), new List<int>(), Identity[0], Identity[0]);
            if (null == val) return false;
            var input = val.Clone() as string ?? string.Empty;
            if (string.IsNullOrEmpty(val)) return false;
            var positive = true;
            if (val.StartsWith(NegativeSign))
            {
                positive = false;
                var idx = val.IndexOf(NegativeSign, StringComparison.Ordinal);
                input = val[(idx + 1)..];
            }
            var floatString = input.Split(NumberDecimalSeparator);
            var integralString = floatString[0];
            var fractionalString = (floatString.Length == 1 ? string.Empty : floatString[1]);
            var integralKeys = !string.IsNullOrEmpty(Separator) && floatString[0].Contains(Separator)
                ? integralString.Split(Separator).Select(Identity.IndexOf).ToList()
                : integralString.SplitAndKeep(Identity.ToArray()).Select(Identity.IndexOf).ToList();
            if (integralKeys.Any(x => x == -1))
            {
                sucess = false;
            }
            var fractionalKeys = !string.IsNullOrEmpty(Separator) && fractionalString.Contains(Separator)
                ?
                fractionalString.Split(Separator).Select(Identity.IndexOf).ToList()
                : string.IsNullOrEmpty(fractionalString)
                    ? new List<int>()
                    : fractionalString.SplitAndKeep(Identity.ToArray()).Select(x => x.Split(Separator)[0]).Select(Identity.IndexOf).ToList();
            if (fractionalKeys.Any(x => x == -1))
            {
                sucess = false;
            }
            result = (positive, integralKeys.Select(x => !SkipUnknownValues && x == -1? 0 : x).Where(x => x != -1).ToList(), fractionalKeys.Select(x => !SkipUnknownValues && x == -1? 0 : x).Where(x => x != -1).ToList(), integralString, fractionalString);
            return sucess;
        }

        public string Encode(string value, string newline = null)
        {
            var zero = Get(0).integral;
            var ffLength = Get( char.MaxValue).integral.Length;
            var elms = (from char ch in value select Get(ch) into result select result.integral).ToList();
            var maxStringLength = elms.Max(s => s.Length);
            var remainder = maxStringLength % ffLength;
            var prependZeroCount = (ffLength - remainder) % ffLength;
            if (Identity.Contains(newline) || newline == Separator)
            {
                newline = null;
            }

            if (null == newline)
            {
                elms = elms.Select(s =>
                {
                    var targetLength = maxStringLength + prependZeroCount;
                    return string.Concat(Enumerable.Repeat(zero,targetLength - s.Length)) + s;
                }).ToList(); 
            } 
            return string.Join(newline, elms);
        }
        
        public string Decode(string value, string newline = null)
        {
            var buider = new StringBuilder();
            var ffLength = Get( char.MaxValue).integral.Length;
            List<string> lines;
            if (null != newline)
            {
                lines = value.Split(newline).ToList();
            }else
            {
                lines = new List<string>();
                for (var i = 0; i < value.Length; i += ffLength)
                {
                    var substring = new string(value.Skip(i).Take(ffLength).ToArray());
                    var parsed = substring.SplitAndKeep(Identity.ToArray()).SkipWhile(x => x == Identity[0]).ToList();
                    var numeral = string.Join(string.Empty, parsed);
                    lines.Add(numeral);
                }
            }
            foreach (var line in lines)
            {
                var result = TryIntegerOf(line, out var integer);
                if (!result) continue;
                buider.Append((char)integer);
            }
            return buider.ToString();
        }
        
        public bool TryFromIndices(List<int> integralIndices, List<int> fractionalIndices, out string result, bool positive = true)
        {
            var success = true;
            var integralList = (integralIndices ?? new List<int>())
                .Select(x =>
                {
                    if (x >= 0 && x < Identity.Count) return Identity[x];
                    success = false;
                    return Identity[0];
                }).ToList();
            var integral = string.Join(Separator, integralList);
            var fractionalList = (fractionalIndices ?? new List<int>())
                .Select(x =>
                {
                    if (x >= 0 && x < Identity.Count) return Identity[x];
                    success = false;
                    return Identity[0];
                }).ToList();
            var fractional = string.Join(Separator, fractionalList);
            var isFloat = fractionalList.Any(x => x != Identity[0]);
            result = isFloat ? $"{(positive ? string.Empty : NegativeSign)}{integral}{NumberDecimalSeparator}{fractional}" : positive ? integral : $"{NegativeSign}{integral}";
            return success;
        }
        
        public Numeral Parse(string val)
        {
            if (!TrySplitNumberIndices(val, out var result)) throw new InvalidOperationException($"'{val}' is not a valid numeral");
            var (positive, integralIndices, fractionalIndices, _, _) = result;
            return new Numeral(this, integralIndices, fractionalIndices, positive);
        }
        
        public bool TryParse(string value, out Numeral result)
        {
            var success = TrySplitNumberIndices(value, out var r);
            var (positive, integralIndices, fractionalIndices, _, _) = r;
            result = new Numeral(this, integralIndices, fractionalIndices, positive);
            return success;
        }

        public bool Contains(IList<string> value) => (null != value && value.ToList().All(Identity.Contains));
        public bool Contains(IList<int> value) => (null != value && value.ToList().All(x => x >= 0 && x < Size));
        
        private List<int> IntegralIndicesOf(ulong value)
        {
            if (value == 0) return new List<int> {0};
            IEnumerable<int> result = new List<int>();
            while (value != 0)
            {
                var remainder = (int) (value % (ulong) Size);
                value /= (ulong) Size;
                result = result.Prepend(remainder);
            }
            return result.ToList();
        }

        public (bool positive, List<int> integralIndices, List<int> fractionalIndices, string integral, string fractional) Get(int index)
        {
            var integralIndices = IntegralIndicesOf((ulong) Math.Abs(index));
            TryFromIndices(integralIndices, null, out var integral, index >= 0);
            return (index >= 0, integralIndices, new List<int>(), integral, Identity[0]);
        }

        public Numeral this[int index]
        {
            get
            {
                var (positive, integralIndices, fractionalIndices, _, _) = Get(index);
                return new Numeral(this, integralIndices, fractionalIndices, positive);
            }
        }
        
        public (bool positive, List<int> integralIndices, List<int> fractionalIndices, string integral, string fractional) Get(decimal index)
        {
            var zero = Numeral.System.Characters.Numbers.First();
            var integral = IntegralIndicesOf((ulong) Math.Abs(index));
            TryFromIndices(integral, null, out var integralStr, index > 0);
            var cultureInfo = (CultureInfo) CultureInfo.Clone();
            cultureInfo.NumberFormat.NumberGroupSeparator = string.Empty;
            var dSplitString = index.ToString("N19", cultureInfo)
                .Split(NumberDecimalSeparator)
                .ToArray();
            if (dSplitString[1].All(x => x == zero)) return (index >= 0, integral, new List<int>(), integralStr, Identity[0]);
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
            var fractional = IntegralIndicesOf(fractionalInt);
            fractional = Enumerable.Repeat(0, frontZeros).Concat(fractional).ToList();
            var fractionalStr = string.Join(Separator, fractional.Select(x => Identity[x]));
            return (index >= 0, integral, fractional, integralStr, fractionalStr);
        }
        public (bool positive, List<int> integralIndices, List<int> fractionalIndices, string integral, string fractional) Get(double index)
        {
            var zero = Numeral.System.Characters.Numbers.First();
            var integral = IntegralIndicesOf((ulong) Math.Abs(index));
            TryFromIndices(integral, null, out var integralStr, index > 0);
            var cultureInfo = (CultureInfo) CultureInfo.Clone();
            cultureInfo.NumberFormat.NumberGroupSeparator = string.Empty;
            var dSplitString = index.ToString("N19", cultureInfo)
                .Split(NumberDecimalSeparator)
                .ToArray();
            if (dSplitString[1].All(x => x == zero)) return (index >= 0, integral, new List<int>(), integralStr, Identity[0]);
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
            var fractional = IntegralIndicesOf(fractionalInt);
            fractional = Enumerable.Repeat(0, frontZeros).Concat(fractional).ToList();
            var fractionalStr = string.Join(Separator, fractional.Select(x => Identity[x]));
            return (index >= 0, integral, fractional, integralStr, fractionalStr);
        }

        public Numeral this[double index]
        {
            get
            {
                var (positive, integralIndices, fractionalIndices, _, _) = Get(index);
                return new Numeral(this, integralIndices, fractionalIndices, positive);
            }
        }

        public Numeral this[decimal index]
        {
            get
            {
                var (positive, integralIndices, fractionalIndices, _, _) = Get(index);
                return new Numeral(this, integralIndices, fractionalIndices, positive);
            }
        }

        public Numeral this[long index] => new(this, IntegralIndicesOf((ulong) Math.Abs(index)), positive: index > 0);

        public Numeral this[ulong index] => new(this, IntegralIndicesOf(index), positive: true);

        public Numeral this[uint index] => new(this, IntegralIndicesOf(index), positive: true);

        public Numeral this[short index] => new(this, IntegralIndicesOf((ulong) Math.Abs(index)), positive: index > 0);

        public Numeral this[ushort index] => new(this, IntegralIndicesOf(index), positive: true);

        public Numeral this[sbyte index] => new(this, IntegralIndicesOf((ulong) Math.Abs(index)), positive: index > 0);

        public Numeral this[byte index] => new(this, IntegralIndicesOf(index), positive: true);

        public Numeral this[IEnumerable<byte> index] =>
            new(this, index.Select(x => (int)x).ToList(), positive: true);

        public Numeral this[IEnumerable<char> index] =>
            new(this, index.Select(x => (int)x).ToList(), positive: true);

        public Numeral this[List<int> index] => new(this, index, positive: true);

        public Numeral this[IList<char> index] => new(this, index.Select(x => (int)x).ToList(), positive: true);

        public Numeral this[IList<byte> index] => new(this, index.Select(x => (int)x).ToList(), positive: true);

        public Numeral this[IEnumerable<int> index] => new(this, index.ToList(), positive: true);

        /// <summary>
        /// Try to get the integer value of the indices
        /// </summary>
        /// <param name="indices">Indices of the digits of the number</param>
        /// <param name="result">Integer value representing the indices of the digits</param>
        /// <param name="positive"></param>
        /// <returns>True If the conversion succeds, False if it was approssimated to the nearest 0 possible value</returns>
        public bool TryIntegerOf(IList<int> indices, out int result, bool positive = true)
        {
            result = 0; 
            var success = true;
            var ind = indices;
            if (null == indices || indices.Count == 0)
            {
                ind = new List<int> {0};
            } else if (!Contains(indices))
            {
                ind = indices.Select(x => x < Size && x >= 0 ? x : 0).ToList();
                success = false;
            }
            result = ind.Select((t, i) => t * Convert.ToInt32(Math.Pow(Size, (ind.Count() - 1 - i)))).Sum();
            result = positive ? result : -result;
            return success;
        }

        /// <summary>
        /// Try to get the integer value of the indices
        /// </summary>
        /// <param name="indices">Indices of the digits of the number</param>
        /// <param name="result">Integer value representing the indices of the digits</param>
        /// <param name="positive"></param>
        /// <returns>True If the conversion succeds, False if it was approssimated to the nearest 0 possible value</returns>
        public bool TryIntegerOf(IList<string> indices, out int result, bool positive = true)
        {
            result = 0;
            IList<string> ind;
            if (indices.Count == 0)
            {
                ind = new List<string> { Identity[0] };
            }
            else
            {
                if (indices[0] == NegativeSign)
                {
                    positive = false;
                }
                ind = indices
                    .SkipWhile(x => x.Equals(NegativeSign))
                    .TakeWhile(x => !x.Equals(NumberDecimalSeparator))
                    .Where(x => !x.Equals(Separator))
                    .ToList();
            }
            return TryIntegerOf(ind.Select(x => Identity.IndexOf(x)).ToList(), out result, positive);
        }
        
        public bool TryIntegerOf(string value, out int integral, bool positive = true)
        {
            integral = 0;
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            
            //var integral = string.Join(string.Empty, value.Split(NumberDecimalSeparator).First());
            //var test = integral.SplitAndKeep(Identity.ToArray());
            var success = TrySplitNumberIndices(value, out var result);
            var test = TryIntegerOf(result.integralIndices, out integral, result.positive | positive);
            return success && test;
        }
        
        
    }
}