using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NumeralSystems.Net
{
    public class Numeral<TElement>
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public bool Positive { get; set; } = true;
        // ReSharper disable once MemberCanBePrivate.Global
        public NumeralSystem<TElement> Base { get; }

        private IList<TElement> _fractional;

        public IList<TElement> Fractional
        {
            get => _fractional ?? new List<TElement>();
            set
            {
                if (Base.Contains(value))
                    _fractional = value;
            }
        }

        private IList<TElement> _integral;
        // ReSharper disable once MemberCanBePrivate.Global
        public IList<TElement> Integral
        {
            get => _integral ?? new List<TElement>();
            // ReSharper disable once MemberCanBePrivate.Global
            set
            {
                if (Base.Contains(value))
                    _integral = value;
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public Numeral(NumeralSystem<TElement> numericSystem)
        {
            Base = numericSystem ?? throw new Exception("Cannot build a number without Its numeric system");
            Integral = new List<TElement>();
            Fractional = new List<TElement>();
        }

        public Numeral(NumeralSystem<TElement> numericSystem, IList<TElement> integral, IList<TElement> fractional = null, bool positive = true)
        {
            Base = numericSystem ?? throw new Exception("Cannot build a number without Its numeric system");
            if (!Base.Contains(integral)) throw new Exception("Cannot build a number without a valid representation");
            if (!Base.Contains(fractional)) fractional = null;
            Integral = integral ?? new List<TElement>();
            Fractional = fractional ?? new List<TElement>();
            Positive = positive;
        }

        public bool TrySetValue(IList<TElement> value)
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

        public float Float
        {
            get
            {
                var integral = Integral.Select((t, i) => Base.Identity.IndexOf(t) * Convert.ToInt32(Math.Pow(Base.Size, (Integral.Count - 1 - i)))).Sum();
                var fractional = Fractional.Select((t, i) => Base.Identity.IndexOf(t) * Convert.ToInt32(Math.Pow(Base.Size, (Fractional.Count - 1 - i)))).Sum();
                var frontZeros = 0;
                foreach (var t in Fractional)
                {
                    if (t.Equals(Base.Identity[0])) frontZeros++;
                    else break;
                }
                if (integral == 0 && fractional == 0) Positive = true;
                return float.Parse($"{(Positive ? string.Empty : "-")}{integral}{Base.FloatSign}{new string('0', frontZeros)}{fractional}", Base.CultureInfo);
            }
        }

        public Numeral<TDestination> To<TDestination>(NumeralSystem<TDestination> baseSystem) => baseSystem[Float];
        
        public override string ToString() => Base.StringConverter((Integral.ToList(), Fractional.ToList(), Positive));

    }

    public static class Numeral
    {
        public static class System
        {
            private static List<string> ValueRange(int value) =>
                Enumerable.Range(0, value)
                    .ToList()
                    .ConvertAll(x => x switch {
                        < 10 => x.ToString(),
                        >= 10 and <= 35 => ((char)(55 + x)).ToString(),
                        > 35 and <= 61 => ((char)(61 + x)).ToString(),
                        // > 61 and <= 157 => ((char)(98 + x)).ToString(),
                        _ => (x - 148).ToString() 
                    }).ToList();
            public static NumeralSystem<string> OfBase(int value, string separator = "") => value <= 0 ? throw new Exception("Invalid base") : new NumeralSystem<string>(new HashSet<string>(ValueRange(value)), value > 157 ? string.IsNullOrEmpty(separator) ? ";" : separator : separator);

        }
    }
}