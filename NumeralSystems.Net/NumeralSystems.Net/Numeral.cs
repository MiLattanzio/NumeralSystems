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
                if (Base.Contains(value))
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
                if (Base.Contains(value))
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

        public Numeral(NumeralSystem numericSystem, IList<string> integral, IList<string> fractional = null, bool positive = true)
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
            set => Integral = Base[value].Integral;
            get
            {
                var output = Integral.Select((t, i) => Base.Identity.IndexOf(t) * Convert.ToInt32(Math.Pow(Base.Size, (Integral.Count - 1 - i)))).Sum();
                return Positive ? output : - output;
            }
        }

        public double Double
        {
            get
            {
                var integral = Integral.Select((t, i) => Base.Identity.IndexOf(t) * Convert.ToUInt32(Math.Pow(Base.Size, (Integral.Count - 1 - i)))).Sum();
                var fractional = Fractional.Select((t, i) => Base.Identity.IndexOf(t) * Convert.ToUInt32(Math.Pow(Base.Size, (Fractional.Count - 1 - i)))).Sum();
                var frontZeros = 0;
                foreach (var t in Fractional)
                {
                    if (t.Equals(Base.Identity[0])) frontZeros++;
                    else break;
                }
                if (integral == 0 && fractional == 0) Positive = true;
                return ((Positive ? 1 : -1) * (integral + (fractional / Math.Pow(Base.Size, Fractional.Count + frontZeros))));
            }
        }

        public Numeral To(NumeralSystem baseSystem) => baseSystem[Double];
        
        public override string ToString() => Base.StringConverter((Integral.ToList(), Fractional.ToList(), Positive));

        public static class System
        {
            public static class Characters
            {
                
                
                public static IEnumerable<char> Numbers = Enumerable.Range(char.MinValue, char.MaxValue + 1)
                    .Skip(48)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c)).Take(10);
                
                public static IEnumerable<char> UpperLetters = Enumerable.Range(char.MinValue, char.MaxValue + 1).Skip(65)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c)).Take(26);
                
                public static IEnumerable<char> LowerLetters = Enumerable.Range(char.MinValue, char.MaxValue + 1).Skip(97)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c)).Take(26);
                
                public static IEnumerable<char> SymbolsA = Enumerable.Range(char.MinValue, char.MaxValue + 1)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c)).Take(16);
                
                public static IEnumerable<char> SymbolsB = Enumerable.Range(char.MinValue, char.MaxValue + 1).Skip(33)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c)).Take(7);
                
                public static IEnumerable<char> SymbolsC = Enumerable.Range(char.MinValue, char.MaxValue + 1).Skip(58)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c)).Take(6);
                
                public static IEnumerable<char> SymbolsD = Enumerable.Range(char.MinValue, char.MaxValue + 1).Skip(91)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c)).Take(6);
                
                public static IEnumerable<char> Others = Enumerable.Range(char.MinValue, char.MaxValue + 1).Skip(123)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c));
                
                public static IEnumerable<char> Alphanumeric = Numbers.Concat(UpperLetters).Concat(LowerLetters);
                
                public static IEnumerable<char> AlphanumericUpper = Numbers.Concat(UpperLetters);
                
                public static IEnumerable<char> AlphanumericLower = Numbers.Concat(LowerLetters);
                
                public static IEnumerable<char> AlphanumericSymbols = Alphanumeric.Concat(SymbolsA.Concat(SymbolsB).Concat(SymbolsC).Concat(SymbolsD));

                public static IEnumerable<char> Printable = Numbers.Concat(UpperLetters).Concat(LowerLetters).Concat(SymbolsA).Concat(SymbolsB).Concat(SymbolsC).Concat(SymbolsD).Concat(Others);

                public static IEnumerable<char> NotPrintable = Enumerable.Range(char.MinValue, char.MaxValue + 1)
                    .Select(i => (char)i)
                    .Where(char.IsControl);
                
                public static IEnumerable<char> All = Printable.Concat(NotPrintable);

                public static IEnumerable<char> WhiteSpaces = Printable.Where(ch => string.IsNullOrWhiteSpace(Convert.ToString(ch)));

                public const char Point = '.';
                
                public const char Comma = ',';
                
                public const char Minus = '-';

                public const char Semicolon = ';';

            }

            private static List<string> ValueRange(int value, IEnumerable<char> identity, Func<char, bool> allow = null)
            {
                var enumerable = identity.ToList();
                if (value <= enumerable.Count()) return enumerable.Where(c => allow?.Invoke(c) ?? true).Take(value).Select(x => Convert.ToString(x)).ToList();
                return Sequence.IdentityEnumerableOfSize(enumerable.Where(c => allow?.Invoke(c) ?? true).ToList(), value).Select(x => string.Join(string.Empty, x)).ToList();
            }
            
            public static NumeralSystem OfBase(int value, string separator = "", IEnumerable<char> identity = null)
            {
                if (null == identity) identity = Characters.AlphanumericSymbols;
                var enumerable = identity.ToList();
                if (enumerable.Any(separator.Contains)) throw new ArgumentException("Separator cannot be contained in identity");
                var range = ValueRange(value, enumerable, (c) => !separator.Contains(c) && c is not Characters.Minus and not Characters.Comma and not Characters.Point) ;
                var set = new HashSet<string>(range);
                return new NumeralSystem(set, separator);
            }

        }

    }
}