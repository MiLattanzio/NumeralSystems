using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;
using Math = System.Math;
using Convert = System.Convert;

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

        public List<string> GetFractionalStrings(IList<string> identity)
        {
            if (identity.Count < Base.Size) throw new ArgumentOutOfRangeException(nameof(identity), "Identity must be at least the size of the base");
            return FractionalIndices.Select(identity.ElementAt).ToList();
        }

        public string GetFractionalString(IList<string> identity, string separator)
        {
            var result = string.Join(separator, GetFractionalStrings(identity));
            return string.IsNullOrEmpty(result) ? identity[0] : result;
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
        
        public List<string> GetIntegralStrings(IList<string> identity)
        {
            if (identity.Count < Base.Size) throw new ArgumentOutOfRangeException(nameof(identity), "Identity must be at least the size of the base");
            return IntegralIndices.Select(identity.ElementAt).ToList();
        }

        public string GetIntegralString(IList<string> identity, string separator)
        {
            var result = string.Join(separator, GetIntegralStrings(identity));
            return string.IsNullOrEmpty(result) ? identity[0] : result;
        }

        public Numeral()
        {
            Base = Numeral.System.OfBase(10);
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public Numeral(NumeralSystem numericSystem)
        {
            Base = numericSystem ?? throw new Exception("Cannot build a number without Its numeric system");
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
                Base.TryIntegerOf(IntegralIndices, out var result, Positive);
                return result;
            }
            set
            {
                IntegralIndices = Base[value].IntegralIndices;
                FractionalIndices = new List<int>();
            }
        }
        
        public char Char
        {
            get
            {
                Base.TryCharOf(IntegralIndices, out var result, Positive);
                return result;
            }
            set
            {
                IntegralIndices = Base[value].IntegralIndices;
                FractionalIndices = new List<int>();
            }
        }

        public double Double
        {
            get => Utils.Encode.Double.FromIndicesOfBase(IntegralIndices.Select(x => (ulong) x).ToArray(), FractionalIndices.Select(x => (ulong) x).ToArray(), Positive, Base.Size);
            set
            {
                var temp = Base[value];
                IntegralIndices = temp.IntegralIndices;
                FractionalIndices = temp.FractionalIndices;
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
                    Convert.ToUInt64(Math.Pow(Base.Size, (FractionalIndices.Count - 1 - i)))).ToList();
                var fractional = fractionalEnumerable.Any() ? fractionalEnumerable.Aggregate((a, c) => a + c) : 0;
                var frontZeros = 0;
                foreach (var t in FractionalIndices)
                {
                    if (t == 0) frontZeros++;
                    else break;
                }

                if (integral == 0 && fractional == 0) Positive = true;
                var digitsInBase = (int) Utils.Math.DigitsInBase(fractional, 10) + frontZeros;
                var div = (decimal) Math.Pow(10, digitsInBase);
                return ((Positive ? 1 : -1) * (integral + (decimal.Divide(fractional, div))));
            }
            set
            {
                var temp = Base[value];
                IntegralIndices = temp.IntegralIndices;
                FractionalIndices = temp.FractionalIndices;
                Positive = value >= 0;
            }
        }

        public float Float
        {
            get => decimal.ToSingle(Decimal);
            set => Decimal = Convert.ToDecimal(value);
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
                    Convert.ToUInt64(Math.Pow(Base.Size, (FractionalIndices.Count - 1 - i)))).ToList();
                var fractional = fractionalEnumerable.Any() ? fractionalEnumerable.Aggregate((a, c) => a + c) : 0;
                var frontZeros = 0;
                foreach (var t in FractionalIndices)
                {
                    if (t == 0) frontZeros++;
                    else break;
                }

                if (integral == 0 && fractional == 0) Positive = true;
                var digitsInBase = (int) Utils.Math.DigitsInBase(fractional, 10) + frontZeros;
                var div = (decimal) Math.Pow(10, digitsInBase);
                var result = ((Positive ? 1 : -1) * (integral + (decimal.Divide(fractional, div))));
                return decimal.GetBits(result).SelectMany(BitConverter.GetBytes).ToArray();
            }
            set
            {
                // Byte array to int array
                var intArray = new int[value.Length / 4];
                Buffer.BlockCopy(value, 0, intArray, 0, value.Length);
                switch (intArray.Length)
                {
                    case < 4:
                        // Pad so it's 4 int long
                        Enumerable.Range(0, 4 - intArray.Length).ToList().ForEach(i => intArray = intArray.Append(0).ToArray());
                        break;
                    case 4:
                        break;
                    case > 4:
                        // Truncate to 4 int long
                        //intArray = intArray.Take(4).ToArray();
                        throw new ArgumentOutOfRangeException(nameof(value), "Byte array is too long");
                        break;
                }
                var result = new decimal(intArray);
                Decimal = result;
            }
        }

        public Numeral To(NumeralSystem baseSystem) => baseSystem[Decimal];

        public string ToString(IList<string> identity, string separator, string negativeSign, string numberDecimalSeparator)
        {
            Base.TryFromIndices(IntegralIndices, FractionalIndices, identity, separator, negativeSign, numberDecimalSeparator, out var result, Positive);
            return result;
        }

        public override string ToString()
        {
            var serializationInfo = NumeralSystem.SerializationInfo.OfBase(Base.Size);
            return ToString(serializationInfo.Identity, serializationInfo.Separator, serializationInfo.NegativeSign, serializationInfo.NumberDecimalSeparator);
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

                public static readonly IEnumerable<char> SymbolsA = Enumerable.Range(0, char.MaxValue + 1)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c))
                    .Take(16);

                public static readonly IEnumerable<char> SymbolsB = Enumerable.Range(33, char.MaxValue + 1 - 33)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c))
                    .Take(7);

                public static readonly IEnumerable<char> SymbolsC = Enumerable.Range(58, char.MaxValue + 1 - 58)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c))
                    .Take(6);

                public static readonly IEnumerable<char> SymbolsD = Enumerable.Range(91, char.MaxValue + 1 - 91)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c))
                    .Take(6);

                public static readonly IEnumerable<char> Others = Enumerable.Range(123, char.MaxValue + 1 - 123)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c));


                public static readonly IEnumerable<char> Alphanumeric =
                    Numbers.Concat(UpperLetters).Concat(LowerLetters);

                public static IEnumerable<char> AlphanumericUpper = Numbers.Concat(UpperLetters);

                public static IEnumerable<char> AlphanumericLower = Numbers.Concat(LowerLetters);

                public static IEnumerable<char> AlphanumericSymbols =
                    Alphanumeric.Concat(SymbolsA.Concat(SymbolsB).Concat(SymbolsC).Concat(SymbolsD));

                public static readonly IEnumerable<char> Printable = Numbers.Concat(UpperLetters).Concat(LowerLetters)
                    .Concat(SymbolsA).Concat(SymbolsB).Concat(SymbolsC).Concat(SymbolsD).Concat(Others).Distinct();

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

            public static NumeralSystem OfBase(int value) => new (value);
        }
    }
}