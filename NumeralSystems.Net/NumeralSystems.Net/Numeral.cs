using System;
using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net.Utils;

namespace NumeralSystems.Net
{
    public class Numeral
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public bool Positive { get; set; } = true;

        // ReSharper disable once MemberCanBePrivate.Global
        public NumeralSystem Base { get; }

        private IList<string> _fractional;

        public IList<string> Fractional
        {
            get => _fractional ?? new List<string>();
            set
            {
                if (null == value)
                    _fractional = new List<string>();
                else if (Base.Contains(value))
                    _fractional = value;
            }
        }

        private IList<string> _integral;

        // ReSharper disable once MemberCanBePrivate.Global
        public IList<string> Integral
        {
            get => _integral ?? new List<string>();
            // ReSharper disable once MemberCanBePrivate.Global
            set
            {
                if (null == value)
                    _integral = new List<string>();
                else if (Base.Contains(value))
                    _integral = value;
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public Numeral(NumeralSystem numericSystem)
        {
            Base = numericSystem ?? throw new Exception("Cannot build a number without Its numeric system");
            Integral = new List<string>();
            Fractional = new List<string>();
        }

        public Numeral(NumeralSystem numericSystem, IList<string> integral, IList<string> fractional = null,
            bool positive = true)
        {
            Base = numericSystem ?? throw new Exception("Cannot build a number without Its numeric system");
            if (!Base.Contains(integral)) throw new Exception("Cannot build a number without a valid representation");
            if (!Base.Contains(fractional)) fractional = null;
            Integral = integral ?? new List<string>();
            Fractional = fractional ?? new List<string>();
            Positive = positive;
        }

        public bool TrySetValue(IList<string> value)
        {
            if (!Base.Contains(value)) return false;
            Integral = value;
            return true;
        }

        public int Integer
        {
            get
            {
                var output = Integral.Select((t, i) =>
                    Base.Identity.IndexOf(t) * Convert.ToInt32(Math.Pow(Base.Size, (Integral.Count - 1 - i)))).Sum();
                return Positive ? output : -output;
            }
            set => Integral = Base[value].Integral;
        }

        public double Double
        {
            get
            {
                var zero = Base.Identity[0];
                var integralEnumerable = Integral.Select((t, i) =>
                        (ulong) Base.Identity.IndexOf(t) *
                        Convert.ToUInt64(Math.Pow(Base.Size, (Integral.Count - 1 - i))))
                    .ToList();
                var integral = integralEnumerable.Any() ? integralEnumerable.Aggregate((a, c) => a + c) : 0;
                var fractionalEnumerable = Fractional.Select((t, i) =>
                    (ulong) Base.Identity.IndexOf(t) *
                    Convert.ToUInt64(Math.Pow(Base.Size, (Fractional.Count - 1 - i)))).ToList();
                var fractional = fractionalEnumerable.Any() ? fractionalEnumerable.Aggregate((a, c) => a + c) : 0;
                var frontZeros = 0;
                foreach (var t in Fractional)
                {
                    if (t.Equals(zero)) frontZeros++;
                    else break;
                }

                if (integral == 0 && fractional == 0) Positive = true;
                var resultString = string.Concat((Positive ? string.Empty : "-").Concat(integral.ToString()
                    .Concat(Base.NumberDecimalSeparator)
                    .Concat(frontZeros > 0 ? new string(zero[0], frontZeros) : string.Empty)
                    .Concat(fractional.ToString())));
                return double.Parse(resultString, Base.CultureInfo);
            }
            set
            {
                var temp = Base[value];
                Integral = temp.Integral;
                Fractional = temp.Fractional;
                Positive = value >= 0;
            }
        }

        public decimal Decimal
        {
            get
            {
                var integralEnumerable = Integral.Select((t, i) =>
                        (ulong) Base.Identity.IndexOf(t) *
                        Convert.ToUInt64(Math.Pow(Base.Size, (Integral.Count - 1 - i))))
                    .ToList();
                var integral = integralEnumerable.Any() ? integralEnumerable.Aggregate((a, c) => a + c) : 0;
                var fractionalEnumerable = Fractional.Select((t, i) =>
                    (ulong) Base.Identity.IndexOf(t) *
                    Convert.ToUInt64(Math.Pow(Base.Size, (Fractional.Count - 1 - i)))).ToList();
                var fractional = fractionalEnumerable.Any() ? fractionalEnumerable.Aggregate((a, c) => a + c) : 0;
                var frontZeros = 0;
                foreach (var t in Fractional)
                {
                    if (t.Equals(Base.Identity[0])) frontZeros++;
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
                Integral = temp.Integral;
                Fractional = temp.Fractional;
                Positive = value >= 0;
            }
        }

        public byte[] Bytes
        {
            get
            {
                var integralEnumerable = Integral.Select((t, i) =>
                        (ulong) Base.Identity.IndexOf(t) *
                        Convert.ToUInt64(Math.Pow(Base.Size, (Integral.Count - 1 - i))))
                    .ToList();
                var integral = integralEnumerable.Any() ? integralEnumerable.Aggregate((a, c) => a + c) : 0;
                var fractionalEnumerable = Fractional.Select((t, i) =>
                    (ulong) Base.Identity.IndexOf(t) *
                    Convert.ToUInt64(Math.Pow(Base.Size, (Fractional.Count - 1 - i)))).ToList();
                var fractional = fractionalEnumerable.Any() ? fractionalEnumerable.Aggregate((a, c) => a + c) : 0;
                var frontZeros = 0;
                foreach (var t in Fractional)
                {
                    if (t.Equals(Base.Identity[0])) frontZeros++;
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

        public Numeral To(NumeralSystem baseSystem) => baseSystem[Double];

        public override string ToString()
        {
            var integralString = Integral.Count == 0
                ? Base.Identity[0]
                : string.Join(Base.Separator, Integral);
            var fractionalString = Fractional.Count == 0
                ? Base.Identity[0]
                : string.Join(Base.Separator, Fractional);
            if ((Integral.Count == 0 || Integral.All(x => x.Equals(Base.Identity[0]))) &&
                (Fractional.Count == 0 || Fractional.All(x => x.Equals(Base.Identity[0]))))
                return integralString;
            return
                $"{(Positive ? string.Empty : Base.NegativeSign)}{integralString}{(Fractional.Count == 0 ? string.Empty : $"{Base.NumberDecimalSeparator}{fractionalString}")}";
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