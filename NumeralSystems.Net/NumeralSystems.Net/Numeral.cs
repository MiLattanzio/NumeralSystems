using System;
using BigInt = System.Numerics.BigInteger;
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
    /// Represents a numeral in a specific numeral system.
    public class Numeral
    {
        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Gets or sets a value indicating whether the number is positive or negative.
        /// </summary>
        public bool Positive { get; set; } = true;

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Represents a numeral in a specific numeral system.
        /// </summary>
        public NumeralSystem Base { get; }

        /// <summary>
        /// Fractional indices are the indices of the fractional part of the number
        /// </summary>
        private readonly List<int> _fractionalIndices = new();

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Gets or sets the list of fractional indices for the numeral.
        /// </summary>
        /// <remarks>
        /// The fractional indices represent the positions of the fractional part of the numeral in the identity list.
        /// </remarks>
        public List<int> FractionalIndices
        {
            get => _fractionalIndices;
            set
            {
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
        /// <summary>
        /// Retrieves the fractional part of a number represented as a collection of strings in a specific numeral system.
        /// </summary>
        /// <param name="identity">The collection of strings representing the number's identity in the numeral system.</param>
        /// <returns>A list of strings representing the fractional part of the number.</returns>
        public List<string> GetFractionalStrings(IList<string> identity)
        {
            if (identity.Count < Base.Size)
                throw new ArgumentOutOfRangeException(nameof(identity),
                    "Identity must be at least the size of the base");
            return FractionalIndices.Select(identity.ElementAt).ToList();
        }

        /// <summary>
        /// Gets the fractional part of the identity as a string, using the provided separator.
        /// </summary>
        /// <param name="identity">The list of strings representing the identity.</param>
        /// <param name="separator">The string used to separate each fractional part.</param>
        /// <returns>A string representing the fractional part of the identity, separated by the provided separator. If there is no fractional part, returns the first element of the identity list.</returns>
        public string GetFractionalString(IList<string> identity, string separator)
        {
            var result = string.Join(separator, GetFractionalStrings(identity));
            return string.IsNullOrEmpty(result) ? identity[0] : result;
        }


        /// <summary>
        /// Represents the list of integral indices of a Numeral object.
        /// </summary>
        private readonly List<int> _integralIndices = new();

        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Gets or sets the list of integral indices representing a numeral system's number.
        /// </summary>
        /// <remarks>
        /// The integral indices represent the positions of the digits in a number within a specific numeral system.
        /// The indices are stored as a list of integers.
        /// </remarks>
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

        /// <summary>
        /// Gets the integral digits of a numeral as a list of string representations.
        /// </summary>
        /// <param name="identity">The list of string representations of the identity of the numeral system (e.g., "0", "1", "2", ...) must have at least the same size as the base.</param>
        /// <returns>A list of string representations of the integral digits of the numeral.</returns>
        public List<string> GetIntegralStrings(IList<string> identity)
        {
            if (identity.Count < Base.Size)
                throw new ArgumentOutOfRangeException(nameof(identity),
                    "Identity must be at least the size of the base");
            return IntegralIndices.Select(identity.ElementAt).ToList();
        }

        /// Returns the integral part of a number represented in a given numeral system as a string.
        /// If the integral part is empty, it returns the first element of the identity.
        /// @param identity The identity of the numeral system represented as a list of strings.
        /// @param separator The separator to be used between the integral digits.
        /// @return The integral part of the number as a string.
        /// @throws ArgumentOutOfRangeException If the size of the identity is less than the size of the numeral system's base.
        /// /
        public string GetIntegralString(IList<string> identity, string separator)
        {
            var result = string.Join(separator, GetIntegralStrings(identity));
            return string.IsNullOrEmpty(result) ? identity[0] : result;
        }

        /// The `Numeral` class represents a numeral in a specific numeral system.
        /// It provides methods to get and set the integral and fractional parts of the numeral, as well as converting the numeral to different types.
        /// @constructor Numeral
        /// @param numericSystem - The numeral system that the numeral belongs to.
        /// @param integral - The list of indices representing the integral part of the numeral.
        /// @param fractional - The list of indices representing the fractional part of the numeral. (Optional)
        /// @param positive - Whether the numeral is positive or negative. (Default: true)
        /// /
        public Numeral()
        {
            Base = Numeral.System.OfBase(10);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        /// The `Numeral` class represents a numeral in a specific numeral system.
        /// @namespace NumeralSystems.Net
        /// @see NumeralSystem
        /// /
        public Numeral(NumeralSystem numericSystem)
        {
            Base = numericSystem ?? throw new Exception("Cannot build a number without Its numeric system");
        }

        /// <summary>
        /// Represents a numerical value in a specific numeral system.
        /// </summary>
        public Numeral(NumeralSystem numericSystem, List<int> integral, List<int> fractional = null,
            bool positive = true)
        {
            Base = numericSystem ?? throw new Exception("Cannot build a number without Its numeric system");
            if (!Base.Contains(integral)) throw new Exception("Cannot build a number without a valid representation");
            if (!Base.Contains(fractional)) fractional = null;
            IntegralIndices = integral;
            FractionalIndices = fractional;
            Positive = positive;
            if (!Base.AdjustToFitIntegralLength) return;
            var difference = Base.Length - IntegralIndices.Count;
            if (difference > 0)
            {
                IntegralIndices = Enumerable.Repeat(0, difference).Concat(IntegralIndices).ToList();
            }
        }

        /// <summary>
        /// Tries to set the value of the Numeral object using the provided list of integers as indices.
        /// </summary>
        /// <param name="value">The list of integer indices representing a value in the NumeralSystem</param>
        /// <returns>True if the value was set successfully, false otherwise</returns>
        public bool TrySetValue(List<int> value)
        {
            if (!Base.Contains(value)) return false;
            IntegralIndices = value;
            return true;
        }

        /// <summary>
        /// Represents a numeral value in a specific numeral system.
        /// </summary>
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

        /// <summary>
        /// Represents a numeral in a specific numeral system.
        /// </summary>
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

        /// <summary>
        /// Represents a double-precision floating-point number.
        /// </summary>
        /// <remarks>
        /// The Double property is used to get or set the value of the Numeral in the form of a double-precision floating-point number.
        /// It converts the Numeral to a double value and vice versa.
        /// </remarks>
        public double Double
        {
            get => Type.Base.Double.FromIndicesOfBase(IntegralIndices.Select(x => (ulong)x).ToArray(),
                FractionalIndices.Select(x => (ulong)x).ToArray(), Positive, Base.Size);
            set
            {
                var temp = Base[value];
                IntegralIndices = temp.IntegralIndices;
                FractionalIndices = temp.FractionalIndices;
                Positive = value >= 0;
            }
        }

        /// <summary>
        /// Represents a numeral in a number system.
        /// </summary>
        public decimal Decimal
        {
            get
            {
                var integralEnumerable = IntegralIndices.Select((t, i) =>
                        (ulong)t *
                        BigInt.Pow(Base.Size, (IntegralIndices.Count - 1 - i)))
                    .ToList();
                var integral = integralEnumerable.Any() ? integralEnumerable.Aggregate((a, c) => a + c) : 0;
                var fractionalEnumerable = FractionalIndices.Select((t, i) =>
                    (ulong)t *
                    Convert.ToUInt64(Math.Pow(Base.Size, (FractionalIndices.Count - 1 - i)))).ToList();
                var fractional = fractionalEnumerable.Any() ? fractionalEnumerable.Aggregate((a, c) => a + c) : 0;
                var frontZeros = 0;
                foreach (var t in FractionalIndices)
                {
                    if (t == 0) frontZeros++;
                    else break;
                }

                if (integral == 0 && fractional == 0) Positive = true;
                var digitsInBase = (int)Utils.Math.DigitsInBase(fractional, 10) + frontZeros;
                var div = (decimal)Math.Pow(10, digitsInBase);
                return ((Positive ? 1 : -1) * ((decimal)integral + (decimal.Divide(fractional, div))));
            }
            set
            {
                var temp = Base[value];
                IntegralIndices = temp.IntegralIndices;
                FractionalIndices = temp.FractionalIndices;
                Positive = value >= 0;
            }
        }

        /// <summary>
        /// Represents a numeral object that can store and manipulate numbers in different numeral systems.
        /// </summary>
        public float Float
        {
            get => decimal.ToSingle(Decimal);
            set => Decimal = Convert.ToDecimal(value);
        }

        /// <summary>
        /// Gets or sets the byte array representation of the Numeral value.
        /// </summary>
        public byte[] Bytes
        {
            get
            {
                var integralEnumerable = IntegralIndices.Select((t, i) =>
                        (ulong)t *
                        Convert.ToUInt64(Math.Pow(Base.Size, (IntegralIndices.Count - 1 - i))))
                    .ToList();
                var integral = integralEnumerable.Any() ? integralEnumerable.Aggregate((a, c) => a + c) : 0;
                var fractionalEnumerable = FractionalIndices.Select((t, i) =>
                    (ulong)t *
                    Convert.ToUInt64(Math.Pow(Base.Size, (FractionalIndices.Count - 1 - i)))).ToList();
                var fractional = fractionalEnumerable.Any() ? fractionalEnumerable.Aggregate((a, c) => a + c) : 0;
                var frontZeros = 0;
                foreach (var t in FractionalIndices)
                {
                    if (t == 0) frontZeros++;
                    else break;
                }

                if (integral == 0 && fractional == 0) Positive = true;
                var digitsInBase = (int)Utils.Math.DigitsInBase(fractional, 10) + frontZeros;
                var div = (decimal)Math.Pow(10, digitsInBase);
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
                        Enumerable.Range(0, 4 - intArray.Length).ToList()
                            .ForEach(i => intArray = intArray.Append(0).ToArray());
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

        /// <summary>
        /// Converts the numerical value of the Numeral object to the specified numeral system.
        /// </summary>
        /// <param name="baseSystem">The target numeral system to convert the value to.</param>
        /// <returns>A new Numeral object representing the converted value in the specified numeral system.</returns>
        public Numeral To(NumeralSystem baseSystem) => baseSystem[Decimal];

        /// <summary>
        /// Represents a numeral in a specific numeral system.
        /// </summary>
        public string ToString(IList<string> identity, string separator, string negativeSign,
            string numberDecimalSeparator)
        {
            Base.TryFromIndices(IntegralIndices, FractionalIndices, identity, separator, negativeSign,
                numberDecimalSeparator, out var result, Positive);
            return result;
        }

        /// <summary>
        /// Returns a string that represents the current object in a specific format using the default identity, separator, negative sign, and number decimal separator.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var serializationInfo = NumeralSystem.SerializationInfo.OfBase(Base.Size);
            return ToString(serializationInfo.Identity, serializationInfo.Separator, serializationInfo.NegativeSign,
                serializationInfo.NumberDecimalSeparator);
        }

        /// The `System` class is a collection of static nested classes that provides various properties related to numeral systems.
        /// The `System` class is part of the `Numeral` namespace.
        /// This class contains the following nested classes:
        /// - `Characters`: Provides collections of characters representing different types of characters used in numeral systems, such as numbers, letters, symbols, etc.
        /// - `OfBase`: Provides a method to create a new instance of the `NumeralSystem` class with a specified base.
        /// Example usage:
        /// ```csharp
        /// var base10 = Numeral.System.OfBase(10);
        /// ```
        /// /
        public static class System
        {
            /// <summary>
            /// The Characters class provides static properties that define various sets of characters.
            /// These sets include numbers, upper and lower case letters, symbols, and other printable and non-printable characters.
            /// The class is part of the NumeralSystems.Net namespace.
            /// </summary>
            /// <seealso cref="NumeralSystems.Net.Numeral.System"/>
            public static class Characters
            {
                /// Collection of numeric characters.
                /// </summary>
                public static readonly IEnumerable<char> Numbers = Enumerable.Range(char.MinValue, char.MaxValue + 1)
                    .Skip(48)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c)).Take(10);

                /// <summary>
                /// A collection of upper case letters.
                /// </summary>
                public static readonly IEnumerable<char> UpperLetters = Enumerable
                    .Range(char.MinValue, char.MaxValue + 1).Skip(65)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c)).Take(26);

                /// <summary>
                /// Provides a collection of lower case letters in the Unicode character set.
                /// </summary>
                public static readonly IEnumerable<char> LowerLetters = Enumerable
                    .Range(char.MinValue, char.MaxValue + 1).Skip(97)
                    .Select(i => (char)i)
                    .Where(c => !char.IsControl(c)).Take(26);

                /// The collection of symbols used in the Numeral System.
                /// The collection includes alphanumeric symbols, as well as special characters.
                /// <remarks>
                /// The SymbolsA collection is designed to include a wide range of characters,
                /// such as letters, numbers, and other symbols, that can be used in various numeral systems.
                /// The collection is created by combining the Numbers, UpperLetters, LowerLetters,
                /// and other specified collections of characters.
                /// The resulting collection is then filtered to remove duplicate characters.
                /// </remarks>
                public static IEnumerable<char> Symbols
                {
                    get
                    {
                        var others = Alphanumeric.ToList();
                        return Enumerable.Range(0, char.MaxValue + 1)
                            .Select(i => (char)i)
                            .Where(c => !char.IsControl(c))
                            .Where(c => !others.Contains(c));
                    }
                }


                /// <summary>
                /// Represents a collection of alphanumeric characters, which includes numbers, uppercase letters, and lowercase letters.
                /// </summary>
                public static readonly IEnumerable<char> Alphanumeric =
                    Numbers.Concat(UpperLetters).Concat(LowerLetters);

                /// <summary>
                /// The collection of uppercase alphanumeric characters.
                /// </summary>
                /// <remarks>
                /// This collection includes uppercase letters (A-Z) and numbers (0-9).
                /// </remarks>
                public static IEnumerable<char> AlphanumericUpper = Numbers.Concat(UpperLetters);

                /// <summary>
                /// The collection of lowercase alphanumeric characters.
                /// </summary>
                public static IEnumerable<char> AlphanumericLower = Numbers.Concat(LowerLetters);

                /// <summary>
                /// Represents a collection of alphanumeric symbols.
                /// </summary>
                public static IEnumerable<char> AlphanumericSymbols =
                    Numbers.Concat(UpperLetters).Concat(LowerLetters)
                        .Concat(Symbols);

                //TODO: Remove duplicates
                /// <summary>
                /// The set of printable characters.
                /// </summary>
                public static readonly IEnumerable<char> Printable = Numbers.Concat(UpperLetters)
                    .Concat(LowerLetters)
                    .Concat(Symbols)
                    .Distinct();

                /// <summary>
                /// Characters that can't be printed
                /// </summary>
                public static readonly IEnumerable<char> NotPrintable = Enumerable
                    .Range(char.MinValue, char.MaxValue + 1)
                    .Select(i => (char)i)
                    .Where(char.IsControl);

                /// <summary>
                /// Contains all printable and non-printable characters in the Numeral Systems library.
                /// </summary>
                public static readonly IEnumerable<char> All = Printable.Concat(NotPrintable);

                /// <summary>
                /// Represents a collection of white space characters.
                /// </summary>
                public static IEnumerable<char> WhiteSpaces =
                    Printable.Where(ch => string.IsNullOrWhiteSpace(Convert.ToString(ch)));

                /// <summary>
                /// Represents a point character.
                /// </summary>
                public const char Point = '.';

                /// <summary>
                /// Represents the Comma character (,).
                /// </summary>
                public const char Comma = ',';

                /// <summary>
                /// The minus symbol character (-) used in numeral systems.
                /// </summary>
                public const char Minus = '-';

                /// <summary>
                /// Represents the semicolon character (;).
                /// </summary>
                /// <remarks>
                /// Used to separate items in numeral formats.
                /// </remarks>
                public const char Semicolon = ';';
            }

            /// <summary>
            /// Returns a range of values based on the given value and identity sequence.
            /// </summary>
            /// <param name="value">The value to determine the range.</param>
            /// <param name="identity">The sequence of identity values.</param>
            /// <returns>An enumerable range of values.</returns>
            private static IEnumerable<string> ValueRange(int value, IEnumerable<string> identity)
            {
                var enumerable = identity.ToList();
                return Sequence.IdentityEnumerableOfSize(enumerable, value).Select(x => string.Join(string.Empty, x));
            }

            /// <summary>
            /// Creates a NumeralSystem object of a specified base.
            /// </summary>
            /// <param name="value">The base value of the NumeralSystem.</param>
            /// <returns>A NumeralSystem object of the specified base.</returns>
            public static NumeralSystem OfBase(int value) => new(value);
        }
    }
}