using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Utils;

namespace NumeralSystems.Net
{
    public class Numeral
    {
        // ReSharper disable once MemberCanBePrivate.Global
        // If the number is positive or negative
        public bool Positive { get; set; } = true;

        // ReSharper disable once MemberCanBePrivate.Global
        // The base of the number
        public NumeralSystem Base { get; }
        
        // Fractional indices are the indices of the fractional part of the number
        private readonly List<int> _fractionalIndices = new();

        // ReSharper disable once MemberCanBePrivate.Global
        public List<int> FractionalIndices
        {
            get => _fractionalIndices;
            set {
                if (null == value || value.Count == 0)
                    _fractionalIndices.Clear();
                else if (Base.Contains(value))
                {
                    _fractionalIndices.Clear();
                    value.ForEach(_fractionalIndices.Add);
                }
            }
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public List<string> Fractionals
        {
            get => FractionalIndices.Select(Base.Identity.ElementAt).ToList();
            set
            {
                if (null == value || value.Count == 0)
                    FractionalIndices = null;
                else if (Base.Contains(value))
                    FractionalIndices = value.Select(Base.Identity.IndexOf).ToList();
            }
        }
        
        public string Fractional
        {
            get
            {
                var result = string.Join(Base.Separator, Fractionals);
                return string.IsNullOrEmpty(result) ? Base.Identity[0] : result;
            }
            set
            {
                Base.TrySplitNumberIndices(value, out var result);
                Positive = result.positive;
                FractionalIndices = result.integralIndices;
            }
        }

        
        private readonly List<int> _integralIndices = new ();

        // ReSharper disable once MemberCanBePrivate.Global
        public List<int> IntegralIndices
        {
            get => _integralIndices;
            // ReSharper disable once MemberCanBePrivate.Global
            set
            {
                if (null == value || value.Count == 0)
                    _integralIndices.Clear();
                else if (Base.Contains(value))
                {
                    _integralIndices.Clear();
                    value.ForEach(_integralIndices.Add);
                }
            }
        }

        public List<string> Integrals
        {
            get => IntegralIndices.Select(Base.Identity.ElementAt).ToList();
            set
            {
                if (null == value || value.Count == 0)
                    IntegralIndices = null;
                else if (Base.Contains(value))
                    IntegralIndices = value.Select(Base.Identity.IndexOf).ToList();
            }
        }
        
        public string Integral
        {
            get
            {
                var result = string.Join(Base.Separator, Integrals);
                return string.IsNullOrEmpty(result) ? Base.Identity[0] : result;
            }
            set
            {
                Base.TrySplitNumberIndices(value, out var result);
                Positive = result.positive;
                IntegralIndices = result.integralIndices;
            }
        }

        public Numeral()
        {
            Base = Numeral.System.OfBase(10, string.Empty);
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public Numeral(NumeralSystem numericSystem)
        {
            Base = numericSystem ?? throw new Exception("Cannot build a number without Its numeric system");
        }
        
        public Numeral(NumeralSystem numericSystem, List<string> integral, List<string> fractionals = null, bool positive = true)
        {
            Base = numericSystem ?? throw new Exception("Cannot build a number without Its numeric system");
            if (!Base.Contains(integral)) throw new Exception("Cannot build a number without a valid representation");
            if (!Base.Contains(fractionals)) fractionals = null;
            Integrals = integral;
            Fractionals = fractionals;
            Positive = positive;
        }
        
        public Numeral(NumeralSystem numericSystem, List<int> integral, List<int> fractional = null, bool positive = true)
        {
            Base = numericSystem ?? throw new Exception("Cannot build a number without Its numeric system");
            if (!Base.Contains(integral)) throw new Exception("Cannot build a number without a valid representation");
            if (!Base.Contains(fractional)) fractional = null;
            IntegralIndices = integral;
            FractionalIndices = fractional;
            Positive = positive;
        }

        public bool TrySetValue(List<int> value) {
            if (!Base.Contains(value)) return false;
            IntegralIndices = value;
            return true;
        }

        public int Integer
        {
            get
            {
                var output = IntegralIndices.Select((t, i) =>
                    t * Convert.ToInt32(Math.Pow(Base.Size, (IntegralIndices.Count - 1 - i)))).Sum();
                return Positive ? output : -output;
            }
            set
            {
                IntegralIndices = Base[value].IntegralIndices;
                FractionalIndices = new List<int>();
            }
        }

        public double Double
        {
            get
            {
                var zero = Base.Identity[0];
                var integralEnumerable = IntegralIndices.Select((t, i) =>
                        (ulong) t *
                        Convert.ToUInt64(Math.Pow(Base.Size, (IntegralIndices.Count - 1 - i))))
                    .ToList();
                var integral = integralEnumerable.Any() ? integralEnumerable.Aggregate((a, c) => a + c) : 0;
                var fractionalEnumerable = FractionalIndices.Select((t, i) =>
                    (ulong) t *
                    Convert.ToUInt64(Math.Pow(Base.Size, (Fractionals.Count - 1 - i)))).ToList();
                var fractional = fractionalEnumerable.Any() ? fractionalEnumerable.Aggregate((a, c) => a + c) : 0;
                var frontZeros = 0;
                foreach (var t in FractionalIndices)
                {
                    if (t == 0) frontZeros++;
                    else break;
                }

                if (integral == 0 && fractional == 0) Positive = true;
                var resultString = string.Concat((Positive ? string.Empty : Base.NegativeSign).Concat(integral.ToString()
                    .Concat(Base.NumberDecimalSeparator)
                    .Concat(frontZeros > 0 ? new string(zero[0], frontZeros) : string.Empty)
                    .Concat(fractional.ToString())));
                return double.Parse(resultString, Base.CultureInfo);
            }
            set
            {
                var temp = Base[value];
                IntegralIndices = temp.IntegralIndices;
                Fractionals = temp.Fractionals;
                Positive = value >= 0;
            }
        }

        public decimal Decimal
        {
            get
            {
                var integralEnumerable = IntegralIndices.Select((t, i) =>
                        (ulong) t *
                        Convert.ToUInt64(Math.Pow(Base.Size, (IntegralIndices.Count - 1 - i))))
                    .ToList();
                var integral = integralEnumerable.Any() ? integralEnumerable.Aggregate((a, c) => a + c) : 0;
                var fractionalEnumerable = FractionalIndices.Select((t, i) =>
                    (ulong) t *
                    Convert.ToUInt64(Math.Pow(Base.Size, (Fractionals.Count - 1 - i)))).ToList();
                var fractional = fractionalEnumerable.Any() ? fractionalEnumerable.Aggregate((a, c) => a + c) : 0;
                var frontZeros = 0;
                foreach (var t in FractionalIndices)
                {
                    if (t == 0) frontZeros++;
                    else break;
                }

                if (integral == 0 && fractional == 0) Positive = true;
                var digitsInBase = (int) NumeralSystem.DigitsInBase(fractional, 10) + frontZeros;
                var div = (decimal) Math.Pow(10, digitsInBase);
                return ((Positive ? 1 : -1) * (integral + (decimal.Divide(fractional, div))));
            }
            set
            {
                var temp = Base[value];
                IntegralIndices = temp.IntegralIndices;
                Fractionals = temp.Fractionals;
                Positive = value >= 0;
            }
        }

        public byte[] Bytes
        {
            get
            {
                var integralEnumerable = IntegralIndices.Select((t, i) =>
                        (ulong) t *
                        Convert.ToUInt64(Math.Pow(Base.Size, (IntegralIndices.Count - 1 - i))))
                    .ToList();
                var integral = integralEnumerable.Any() ? integralEnumerable.Aggregate((a, c) => a + c) : 0;
                var fractionalEnumerable = FractionalIndices.Select((t, i) =>
                    (ulong) t *
                    Convert.ToUInt64(Math.Pow(Base.Size, (Fractionals.Count - 1 - i)))).ToList();
                var fractional = fractionalEnumerable.Any() ? fractionalEnumerable.Aggregate((a, c) => a + c) : 0;
                var frontZeros = 0;
                foreach (var t in FractionalIndices)
                {
                    if (t == 0) frontZeros++;
                    else break;
                }

                if (integral == 0 && fractional == 0) Positive = true;
                var digitsInBase = (int) NumeralSystem.DigitsInBase(fractional, 10) + frontZeros;
                var div = (decimal) Math.Pow(10, digitsInBase);
                var result = ((Positive ? 1 : -1) * (integral + (decimal.Divide(fractional, div))));
                return decimal.GetBits(result).SelectMany(BitConverter.GetBytes).ToArray();
            }
            set
            {
                // Byte array to int array
                var intArray = new int[value.Length / 4];
                Buffer.BlockCopy(value, 0, intArray, 0, value.Length);
                var result = new decimal(intArray);
                Decimal = result;
            }
        }

        public Numeral To(NumeralSystem baseSystem) => baseSystem[Decimal];

        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool showFloat = true)
        {
            var integralString = IntegralIndices.Count == 0
                ? Base.Identity[0]
                : string.Join(Base.Separator, IntegralIndices.Select(x => Base.Identity[x]));
            var fractionalString = Fractionals.Count == 0
                ? Base.Identity[0]
                : string.Join(Base.Separator, FractionalIndices.Select(x => Base.Identity[x]));
            if ((IntegralIndices.Count == 0 || IntegralIndices.All(x => x == 0)) &&
                (Fractionals.Count == 0 || FractionalIndices.All(x => x == 0)))
                return integralString;
            if (showFloat)
                return
                    $"{(Positive ? string.Empty : Base.NegativeSign)}{integralString}{(Fractionals.Count == 0 ? string.Empty : $"{Base.NumberDecimalSeparator}{fractionalString}")}";
            return $"{(Positive ? string.Empty : Base.NegativeSign)}{integralString}";
        }

        public static class System
        {
            public static class Characters
            {
                public static readonly IEnumerable<char> Numbers = Enumerable.Range(char.MinValue, char.MaxValue + 1)
                    .Skip(48)
                    .Select(i => (char) i)
                    .Where(c => !char.IsControl(c)).Take(10);

                public static readonly IEnumerable<char> UpperLetters = Enumerable
                    .Range(char.MinValue, char.MaxValue + 1).Skip(65)
                    .Select(i => (char) i)
                    .Where(c => !char.IsControl(c)).Take(26);

                public static readonly IEnumerable<char> LowerLetters = Enumerable
                    .Range(char.MinValue, char.MaxValue + 1).Skip(97)
                    .Select(i => (char) i)
                    .Where(c => !char.IsControl(c)).Take(26);

                public static readonly IEnumerable<char> SymbolsA = Enumerable.Range(char.MinValue, char.MaxValue + 1)
                    .Select(i => (char) i)
                    .Where(c => !char.IsControl(c)).Take(16);

                public static readonly IEnumerable<char> SymbolsB = Enumerable.Range(char.MinValue, char.MaxValue + 1)
                    .Skip(33)
                    .Select(i => (char) i)
                    .Where(c => !char.IsControl(c)).Take(7);

                public static readonly IEnumerable<char> SymbolsC = Enumerable.Range(char.MinValue, char.MaxValue + 1)
                    .Skip(58)
                    .Select(i => (char) i)
                    .Where(c => !char.IsControl(c)).Take(6);

                public static readonly IEnumerable<char> SymbolsD = Enumerable.Range(char.MinValue, char.MaxValue + 1)
                    .Skip(91)
                    .Select(i => (char) i)
                    .Where(c => !char.IsControl(c)).Take(6);

                public static readonly IEnumerable<char> Others = Enumerable.Range(char.MinValue, char.MaxValue + 1)
                    .Skip(123)
                    .Select(i => (char) i)
                    .Where(c => !char.IsControl(c));

                public static readonly IEnumerable<char> Alphanumeric =
                    Numbers.Concat(UpperLetters).Concat(LowerLetters);

                public static IEnumerable<char> AlphanumericUpper = Numbers.Concat(UpperLetters);

                public static IEnumerable<char> AlphanumericLower = Numbers.Concat(LowerLetters);

                public static IEnumerable<char> AlphanumericSymbols =
                    Alphanumeric.Concat(SymbolsA.Concat(SymbolsB).Concat(SymbolsC).Concat(SymbolsD));

                public static readonly IEnumerable<char> Printable = Numbers.Concat(UpperLetters).Concat(LowerLetters)
                    .Concat(SymbolsA).Concat(SymbolsB).Concat(SymbolsC).Concat(SymbolsD).Concat(Others);

                public static readonly IEnumerable<char> NotPrintable = Enumerable
                    .Range(char.MinValue, char.MaxValue + 1)
                    .Select(i => (char) i)
                    .Where(char.IsControl);

                public static readonly IEnumerable<char> All = Printable.Concat(NotPrintable);

                public static IEnumerable<char> WhiteSpaces =
                    Printable.Where(ch => string.IsNullOrWhiteSpace(Convert.ToString(ch)));

                public const char Point = '.';

                public const char Comma = ',';

                public const char Minus = '-';

                public const char Semicolon = ';';
            }

            private static IEnumerable<string> ValueRange(int value, IEnumerable<string> identity)
            {
                var enumerable = identity.ToList();
                return Sequence.IdentityEnumerableOfSize(enumerable, value).Select(x => string.Join(string.Empty, x));
            }

            public static NumeralSystem OfBase(int value, string separator = "", IEnumerable<string> identity = null)
            {
                var enumerable = identity ?? Characters.All.Select(x => x.ToString());
                var exceptions = new[]
                {
                    separator, NumeralSystem.BaseSeparator, NumeralSystem.BaseNegativeSign,
                    NumeralSystem.BaseNumberDecimalSeparator
                };
                var enumerableSet = new HashSet<string>(enumerable);
                var range = ValueRange(value,
                    enumerableSet.Where(c => !string.IsNullOrWhiteSpace(c) && !exceptions.Contains(c)));
                var set = new HashSet<string>(range);
                return new NumeralSystem(set, separator);
            }
        }
    }
}