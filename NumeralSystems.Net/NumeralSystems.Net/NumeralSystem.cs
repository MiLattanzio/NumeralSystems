using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
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

        public static string BaseNegativeSign => BaseCultureInfo.NumberFormat.NegativeSign;

        /// <summary>
        /// Negative sign of the numeral system, inherited from the culture info.
        /// <para>For example, for base 10 it is -</para>
        /// </summary>
        public string NegativeSign => CultureInfo.NumberFormat.NegativeSign;

        public static CultureInfo BaseCultureInfo => CultureInfo.InvariantCulture;

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

        public static string BaseNumberDecimalSeparator => BaseCultureInfo.NumberFormat.NumberDecimalSeparator;

        /// <summary>
        /// Floating point separator of the numeral system, inherited from the culture info.
        /// <para>For example, for base 10 it is .</para>
        /// </summary>
        public string NumberDecimalSeparator => CultureInfo.NumberFormat.NumberDecimalSeparator;

        public static string BaseSeparator => Convert.ToString(Numeral.System.Characters.Semicolon);
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
        
        public bool TrySplitNumberIndices(string val, out (bool positive, List<int> integralIndices, List<int> fractionalIndices, string integral, string fractional) result)
        {
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
            var fractionalKeys = !string.IsNullOrEmpty(Separator) && fractionalString.Contains(Separator)
                ?
                fractionalString.Split(Separator).Select(Identity.IndexOf).ToList()
                : string.IsNullOrEmpty(fractionalString)
                    ? new List<int>()
                    : fractionalString.SplitAndKeep(Identity.ToArray()).Select(x => x.Split(Separator)[0]).Select(Identity.IndexOf).ToList();
            result = (positive, integralKeys, fractionalKeys, integralString, fractionalString);
            return integralKeys.All(x => x != -1) && fractionalKeys.All(x => x != -1);
        }
        
        // ReSharper disable once UnusedMethodReturnValue.Global
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
            TryFromIndices(integralIndices, null, out var integral, index > 0);
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
    }
}