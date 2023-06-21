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

        public IList<string> Identity
        {
            get => _identity;
            set
            {
                var val = value?.Distinct().ToList();
                _identity = val ?? throw new InvalidOperationException("Identity cannot be null");
            }
        }

        public int Size => Identity.Count;

        public static string BaseNegativeSign => BaseCultureInfo.NumberFormat.NegativeSign;

        public string NegativeSign => CultureInfo.NumberFormat.NegativeSign;

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

        public string NumberDecimalSeparator => CultureInfo.NumberFormat.NumberDecimalSeparator;

        public static string BaseSeparator => Convert.ToString(Numeral.System.Characters.Semicolon);
        private string _separator;

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
        public NumeralSystem(HashSet<string> identity, string separator = ";")
        {
            if (null == identity || identity.Count == 0) throw new Exception("Identity must contain an element");
            Identity = identity.ToList();
            if (null == separator || Identity.Any(x => separator.Equals(x.ToString())))
                throw new Exception("Invalid separator");
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

        public Numeral Parse(string val)
        {
            if (null == val) return new Numeral(this, new List<int> {0});
            var input = val.Clone() as string ?? string.Empty;
            if (string.IsNullOrEmpty(val)) return this[0];
            var positive = true;
            if (val.StartsWith(NegativeSign))
            {
                positive = false;
                var idx = val.IndexOf(NegativeSign, StringComparison.Ordinal);
                input = val[(idx + 1)..];
            }

            // ReSharper disable once HeapView.PossibleBoxingAllocation

            var floatString = input.Split(NumberDecimalSeparator);
            var integralKeys = !string.IsNullOrEmpty(Separator) && floatString[0].Contains(Separator)
                ? floatString[0].Split(Separator).Select(Identity.IndexOf).ToList()
                : floatString[0].SplitAndKeep(Identity.ToArray()).Select(Identity.IndexOf).ToList();
            integralKeys.ForEach(x =>
            {
                if (x == -1) throw new Exception($"Invalid mapping for value '{x}' of '{val}'");
            });
            var fractionalString = (floatString.Length == 1 ? string.Empty : floatString[1]);
            var fractionalKeys = !string.IsNullOrEmpty(Separator) && fractionalString.Contains(Separator)
                ?
                fractionalString.Split(Separator).Select(Identity.IndexOf).ToList()
                : string.IsNullOrEmpty(fractionalString)
                    ? new List<int>()
                    : fractionalString.SplitAndKeep(Identity.ToArray()).Select(x => x.Split(Separator)[0]).Select(Identity.IndexOf).ToList();
            fractionalKeys.ForEach(x =>
            {
                if (x == -1) throw new Exception($"Invalid mapping for value '{x}' of '{val}'");
            });
            return new Numeral(this, integralKeys, fractionalKeys, positive: positive);
        }

        public bool Contains(IList<string> value) => (null != value && value.ToList().All(Identity.Contains));
        public bool Contains(string value) => (null != value && Identity.Contains(value));
        public bool Contains(IList<int> value) => (null != value && value.ToList().All(x => x >= 0 && x < Size));
        public bool Contains(int value) => (value >= 0 && value < Size);

        private List<int> PositiveIntegralOf(ulong value)
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

        public Numeral this[int index] => new(this, PositiveIntegralOf((ulong) Math.Abs(index)), positive: index > 0);

        public Numeral this[double index]
        {
            get
            {
                var zero = Numeral.System.Characters.Numbers.First();
                var integral = PositiveIntegralOf((ulong) Math.Abs(index));
                var cultureInfo = (CultureInfo) CultureInfo.Clone();
                cultureInfo.NumberFormat.NumberGroupSeparator = string.Empty;
                var dSplitString = index.ToString("N19", cultureInfo)
                    .Split(NumberDecimalSeparator)
                    .ToArray();
                if (dSplitString[1].All(x => x == zero)) return new Numeral(this, integral, new List<int>(), index > 0);
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
                fractional = Enumerable.Repeat(0, frontZeros).Concat(fractional).ToList();
                return new Numeral(this, integral, fractional, index > 0);
            }
        }

        public Numeral this[decimal index]
        {
            get
            {
                var zero = Numeral.System.Characters.Numbers.First();
                var integral = PositiveIntegralOf((ulong) Math.Abs(index));
                var cultureInfo = (CultureInfo) CultureInfo.Clone();
                cultureInfo.NumberFormat.NumberGroupSeparator = string.Empty;
                var dSplitString = index.ToString("N19", cultureInfo)
                    .Split(NumberDecimalSeparator)
                    .ToArray();
                
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
                fractional = Enumerable.Repeat(0, frontZeros).Concat(fractional).ToList();
                return new Numeral(this, integral, fractional, index > 0);
            }
        }

        public Numeral this[long index] => new(this, PositiveIntegralOf((ulong) Math.Abs(index)), positive: index > 0);

        public Numeral this[ulong index] => new(this, PositiveIntegralOf(index), positive: true);

        public Numeral this[uint index] => new(this, PositiveIntegralOf(index), positive: true);

        public Numeral this[short index] => new(this, PositiveIntegralOf((ulong) Math.Abs(index)), positive: index > 0);

        public Numeral this[ushort index] => new(this, PositiveIntegralOf(index), positive: true);

        public Numeral this[sbyte index] => new(this, PositiveIntegralOf((ulong) Math.Abs(index)), positive: index > 0);

        public Numeral this[byte index] => new(this, PositiveIntegralOf(index), positive: true);

        public Numeral this[IEnumerable<byte> index] =>
            new(this, index.Select(x => (int)x).ToList(), positive: true);

        public Numeral this[IEnumerable<char> index] =>
            new(this, index.Select(x => (int)x).ToList(), positive: true);

        public Numeral this[IList<int> index] => new(this, index, positive: true);

        public Numeral this[IList<char> index] => new(this, index.Select(x => (int)x).ToList(), positive: true);

        public Numeral this[IList<byte> index] => new(this, index.Select(x => (int)x).ToList(), positive: true);

        public Numeral this[IEnumerable<int> index] => new(this, index.Select(x => x).ToList(), positive: true);
    }
}