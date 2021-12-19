using System;
using System.Collections.Generic;
using System.Linq;

namespace NumeralSystems.Net
{
    public class Numeral<TElement>
    {
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public bool Positive { get; set; } = true;
        // ReSharper disable once MemberCanBePrivate.Global
        public NumeralSystem<TElement> Base { get; }
        
        private IList<TElement> _value;
        // ReSharper disable once MemberCanBePrivate.Global
        public IList<TElement> Value
        {
            get => _value;
            // ReSharper disable once MemberCanBePrivate.Global
            set
            {
                if (Base.Contains(value))
                    _value = value;
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public Numeral(NumeralSystem<TElement> numericSystem)
        {
            Base = numericSystem ?? throw new Exception("Cannot build a number without Its numeric system");
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            Value = new List<TElement>();
        }

        public Numeral(NumeralSystem<TElement> numericSystem, IList<TElement> value, bool positive = true)
        {
            Base = numericSystem ?? throw new Exception("Cannot build a number without Its numeric system");
            if (!Base.Contains(value)) throw new Exception("Cannot build a number without a valid representation");
            Value = value;
            Positive = positive;
        }

        /*
        protected static Numeral<TElement> Unsafe(NumeralSystem<TElement> numericSystem, IList<TElement> value, bool positive = true)
        {
            // ReSharper disable once UseObjectOrCollectionInitializer
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            var numeric = new Numeral<TElement>(numericSystem);
            numeric._value = value;
            numeric.Positive = positive;
            return numeric;
        }
        */

        // ReSharper disable once MemberCanBePrivate.Global
        public int Length => Value.Count;

        public bool TrySetValue(IList<TElement> value)
        {
            if (!Base.Contains(value)) return false;
            Value = value;
            return true;
        }

        public int Integer
        {
            get
            {
                var output = 0;
                // ReSharper disable once HeapView.ObjectAllocation
                for (var i = 0; i < Length; i++)
                {
                    output += Base.Identity.IndexOf(Value[i]) *
                              Convert.ToInt32(Math.Pow(Base.Size, (Length - 1 - i)));
                }

                return Positive ? output : -output;
            }
        }

        public Numeral<TDestination> To<TDestination>(NumeralSystem<TDestination> baseSystem) => baseSystem[Integer];
        
        // ReSharper disable once HeapView.PossibleBoxingAllocation
        public override string ToString() => Base.StringConverter(Value.ToList(), Positive);

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