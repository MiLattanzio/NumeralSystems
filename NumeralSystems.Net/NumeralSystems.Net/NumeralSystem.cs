using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using NumeralSystems.Net.Type.Base;
using NumeralSystems.Net.Utils;
using Math = System.Math;
using Convert = System.Convert;
using Decimal = System.Decimal;
using Double = System.Double;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable HeapView.ObjectAllocation.Evident
// ReSharper disable MemberCanBePrivate.Global
namespace NumeralSystems.Net
{
    /// The `NumeralSystem` class defines a numeral system with a specified base size.
    /// It offers functionalities to parse, convert, and manipulate numerals in various formats and representations.
    /// Properties:
    /// - `Size`: Represents the size or base of the numeral system.
    /// - `SkipUnknownValues`: Determines whether unknown values should be skipped during processing.
    /// Methods:
    /// - `TrySplitNumberIndices`: Attempts to split a number into integral and fractional parts and returns indices corresponding to these parts.
    /// - `TryFromIndices`: Attempts to construct a string representation from given integral and fractional indices.
    /// - `Parse`: Converts a string representation of a numeral into a `Numeral` object based on the system's specifications.
    /// - `TryParse`: Attempts to parse a numeral from a string, outputting whether it was successful.
    /// - `Contains`: Checks if a given list of indices are valid within the numeral system.
    /// - Indexers: Provides access to numerals using various index types (e.g., int, double, IEnumerable) supported by the numeral system.
    /// - `TryIntegerOf`: Attempts to convert a list of numeral indices into an integer value.
    /// - `TryCharOf`: Attempts to convert a list of numeral indices into a character.
    /// Nested Types:
    /// - `SerializationInfo`: Accompanies numeral parsing by providing additional configuration such as identity, separators, and signs for conversion.
    [SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NumeralSystem
    {
        /// <summary>
        /// Gets the size of the numeral system.
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Gets the length of a numeral representation, calculated as the ceiling of the logarithm of
        /// the maximum byte value plus one, with the base equal to the size of the numeral system.
        /// </summary>
        public int Length => (int)Math.Ceiling(Math.Log(byte.MaxValue + 1, Size));

        /// Determines whether unknown values in the numeral system should be skipped or handled.
        /// When set to true, unknown values encountered are ignored, otherwise they are processed.
        /// This property modifies the behavior of numeral parsing and representation.
        public bool SkipUnknownValues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the numeral system should adjust the number representation
        /// to fit the integral length specified by the system's settings.
        /// </summary>
        public bool AdjustToFitIntegralLength { get; set; } = true;

        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        /// <summary>
        /// The <c>NumeralSystem</c> class represents a numeral system with a specified size and provides functionality to convert numbers to and from the indices of the digits in this numeral system.
        /// </summary>
        public NumeralSystem(int size)
        {
            if (size <= 0) throw new Exception("Size cannot be less than 1");
            Size = size;
        }

        /// <summary>
        /// Attempts to split a given number string into integral and fractional parts and their respective indices within a specified numeral system identity list.
        /// </summary>
        /// <param name="val">The string representation of the number to be split.</param>
        /// <param name="identity">A list of strings representing the numeral system identity to map each character in the number string.</param>
        /// <param name="separator">The string separator used to split digit characters in the integral and fractional parts, if any.</param>
        /// <param name="negativeSign">The string representing the negative sign to identify if the number is negative.</param>
        /// <param name="numberDecimalSeparator">The string used as a decimal separator in the number string.</param>
        /// <param name="result">A tuple containing the split results: a boolean indicating if the number is positive, lists of indices for integral and fractional parts, and the integral and fractional strings.</param>
        /// <returns>Returns true if the number string was successfully split into indices; otherwise, false.</returns>
        public bool TrySplitNumberIndices(string val, IList<string> identity, string separator, string negativeSign,
            string numberDecimalSeparator,
            out (bool positive, List<int> integralIndices, List<int> fractionalIndices, string integral, string
                fractional) result)
        {
            var sucess = true;
            result = (true, new List<int>(), new List<int>(), identity[0], identity[0]);
            if (null == val) return false;
            var input = val.Clone() as string ?? string.Empty;
            if (string.IsNullOrEmpty(val)) return false;
            var positive = true;
            if (val.StartsWith(negativeSign))
            {
                positive = false;
                var idx = val.IndexOf(negativeSign, StringComparison.Ordinal);
                input = val[(idx + 1)..];
            }

            var floatString = input.Split(numberDecimalSeparator);
            var integralString = floatString[0];
            var fractionalString = (floatString.Length == 1 ? string.Empty : floatString[1]);
            var integralKeys = !string.IsNullOrEmpty(separator) && floatString[0].Contains(separator)
                ? integralString.Split(separator).Select(identity.IndexOf).ToList()
                : integralString.SplitAndKeep(identity.ToArray()).Select(identity.IndexOf).ToList();
            if (integralKeys.Any(x => x == -1))
            {
                sucess = false;
            }
            var fractionalKeys = !string.IsNullOrEmpty(separator) && fractionalString.Contains(separator)
                ?
                fractionalString.Split(separator).Select(identity.IndexOf).ToList()
                : string.IsNullOrEmpty(fractionalString)
                    ? new List<int>()
                    : fractionalString.SplitAndKeep(identity.ToArray()).Select(x => x.Split(separator)[0]).Select(identity.IndexOf).ToList();
            if (fractionalKeys.Any(x => x == -1))
            {
                sucess = false;
            }
            result = (positive, integralKeys.Select(x => !SkipUnknownValues && x == -1? 0 : x).Where(x => x != -1).ToList(), fractionalKeys.Select(x => !SkipUnknownValues && x == -1? 0 : x).Where(x => x != -1).ToList(), integralString, fractionalString);
            return sucess;
        }

        /// <summary>
        /// Tries to convert a list of integral and fractional indices to a string representation in a given numeral system.
        /// </summary>
        /// <param name="integralIndices">A list of integral indices representing the digits of the number.</param>
        /// <param name="fractionalIndices">A list of fractional indices representing the digits after the decimal separator.</param>
        /// <param name="identity">A list of strings representing the symbols or digits in the numeral system.</param>
        /// <param name="separator">The separator used between integral and fractional parts of the number.</param>
        /// <param name="negativeSign">The string representing the negative sign.</param>
        /// <param name="numberDecimalSeparator">The string representing the decimal separator.</param>
        /// <param name="result">The string representation of the number in the given numeral system.</param>
        /// <param name="positive">A boolean value indicating whether the number is positive or negative (default is true).</param>
        /// <returns>True if the conversion succeeds, False if the conversion was approximate.</returns>
        public bool TryFromIndices(List<int> integralIndices, List<int> fractionalIndices, IList<string> identity,
            string separator, string negativeSign, string numberDecimalSeparator, out string result,
            bool positive = true)
        {
            var success = true;
            var integralList = (integralIndices ?? new List<int>())
                .Select(x =>
                {
                    if (x >= 0 && x < identity.Count) return identity[x];
                    success = false;
                    return identity[0];
                }).ToList();
            var integral = string.Join(separator, integralList);
            var fractionalList = (fractionalIndices ?? new List<int>())
                .Select(x =>
                {
                    if (x >= 0 && x < identity.Count) return identity[x];
                    success = false;
                    return identity[0];
                }).ToList();
            var fractional = string.Join(separator, fractionalList);
            var isFloat = fractionalList.Any(x => x != identity[0]);
            result = isFloat ? $"{(positive ? string.Empty : negativeSign)}{integral}{numberDecimalSeparator}{fractional}" : positive ? integral : $"{negativeSign}{integral}";
            return success;
        }


        /// <summary>
        /// Parses a string representation of a numeral into a Numeral object using the specified identity, separator, negative sign, and number decimal separator.
        /// </summary>
        /// <param name="val">The string representation of the numeral</param>
        /// <param name="identity">The list of characters that represent the digits of the numeral system</param>
        /// <param name="separator">The separator used to separate integral and fractional parts of the numeral</param>
        /// <param name="negativeSign">The character that represents negative sign in the numeral</param>
        /// <param name="numberDecimalSeparator">The character that represents the decimal point in the numeral</param>
        /// <returns>A Numeral object representing the parsed numeral</returns>
        public Numeral Parse(string val, IList<string> identity, string separator, string negativeSign,
            string numberDecimalSeparator)
        {
            if (!TrySplitNumberIndices(val, identity, separator, negativeSign, numberDecimalSeparator, out var result))
                throw new InvalidOperationException($"'{val}' is not a valid numeral");
            var (positive, integralIndices, fractionalIndices, _, _) = result;
            return new Numeral(this, integralIndices, fractionalIndices, positive);
        }

        // <summary>
        /// <summary>
        /// Tries to parse a string representation of a number using the given numeral system.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <param name="identity">The list of identities of the digits in the numeral system.</param>
        /// <param name="separator">The separator used in the string value.</param>
        /// <param name="negativeSign">The negative sign used in the string value.</param>
        /// <param name="numberDecimalSeparator">The decimal separator used in the string value.</param>
        /// <param name="result">The parsed Numeral object if successful, otherwise null.</param>
        /// <returns>True if the parsing was successful, otherwise false.</returns>
        public bool TryParse(string value, IList<string> identity, string separator, string negativeSign,
            string numberDecimalSeparator, out Numeral result)
        {
            var success = TrySplitNumberIndices(value, identity, separator, negativeSign, numberDecimalSeparator,
                out var r);
            var (positive, integralIndices, fractionalIndices, _, _) = r;
            result = new Numeral(this, integralIndices, fractionalIndices, positive);
            return success;
        }

        // <summary>
        /// <summary>
        /// Determines whether the given list of indices is a valid representation in the numeral system.
        /// </summary>
        /// <param name="value">The list of indices to be validated.</param>
        /// <returns>True if the list of indices represents a valid value in the numeral system, false otherwise.</returns>
        public bool Contains(IList<int> value) => (null != value && value.ToList().All(x => x >= 0 && x < Size));

        /// Indexer for accessing a `Numeral` object based on an integer index.
        /// Converts the given integer index into a numeral representation using the size
        /// of the numeral system. The index is converted into a list of integral indices
        /// corresponding to the base of the numeral system.
        /// Parameters:
        /// index: An integer index to be converted into a `Numeral`.
        /// Returns:
        /// A `Numeral` object that represents the given index within this numeral
        /// system.
        public Numeral this[int index]
        {
            get
            {
                var integral = UInt.ToIndicesOfBase(index, Size, out var positive);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), new List<int>(), index >= 0);
            }
        }

        /// <summary>
        /// Indexer that allows accessing the <see cref="Numeral"/> representation of a double-precision floating-point number.
        /// </summary>
        /// <param name="index">A double-precision floating-point number to be converted into a <see cref="Numeral"/>.</param>
        /// <returns>A <see cref="Numeral"/> representation of the given double-precision floating-point number.</returns>
        public Numeral this[double index]
        {
            get
            {
                var (integral, fractional, positive) = Type.Base.Double.ToIndicesOfBase(index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), fractional.Select(x => (int)x).ToList(),
                    positive);
            }
        }

        /// Provides indexing capabilities for accessing numeral representations within a numeral system.
        /// Allows access by various numeric index types. The indexer retrieves the numeral representation
        /// of a given index in the specific numeral system. The supported numeric types for indexing
        /// include decimal, int, double, long, ulong, uint, short, ushort, sbyte, and byte.
        /// - Use this indexer to obtain the numeral from a decimal index. The index is converted
        /// to the numeral system using the base size of the system.
        /// Note:
        /// This feature is particularly useful when needing to interact with numeral systems of
        /// arbitrary bases and perform conversions accordingly.
        /// param name="index"
        /// The decimal index value to convert and retrieve as a Numeral.
        public Numeral this[decimal index]
        {
            get
            {
                var (integral, fractional, positive) = Type.Base.Decimal.ToIndicesOfBase(index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), fractional.Select(x => (int)x).ToList(), positive);
            }
        }

        /// Provides methods and properties for working with numeral systems and their representation.
        /// The class supports different types of indices to access numerals based on the specified numeral system.
        /// It includes functionality to handle numeral system size and skipping unknown values.
        public Numeral this[long index]
        {
            get
            {
                var integral = ULong.ToIndicesOfBase((ulong)index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), new List<int>(), index >= 0);
            }
        }

        /// Represents a numeral system and provides functionality to work with various numeral bases.
        /// This class supports indexing operations to access numerals based on different types of indices.
        public Numeral this[ulong index]
        {
            get
            {
                var integral = ULong.ToIndicesOfBase(index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), new List<int>(), true);
            }
        }

        /// <summary>
        /// Provides access to various numeral representations within a numeral system using
        /// an indexer based on varying types of indices including integers, floating points,
        /// and collections of numeric types.
        /// </summary>
        /// <param name="index">The index used to select the numeral, which can be of multiple types
        /// such as int, double, decimal, long, ulong, uint, short, ushort, sbyte, byte,
        /// IEnumerable<int>, IEnumerable<byte>, IEnumerable<char>, IList<int>, IList<byte>, and IList<char>.</param>
        /// <returns>A <see cref="Numeral"/> instance corresponding to the specified index within the numeral system.</returns>
        public Numeral this[uint index]
        {
            get
            {
                var integral = ULong.ToIndicesOfBase(index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), new List<int>(), true);
            }
        }

        /// Represents a numeral system with a defined size and behavior for handling unknown values.
        public Numeral this[short index]
        {
            get
            {
                var integral = ULong.ToIndicesOfBase((ulong)index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), new List<int>(), index >= 0);
            }
        }

        /// Provides access to numerals at the specified index within a numeral system.
        /// This property allows for retrieving a `Numeral` object using various index types,
        /// such as `int`, `double`, `decimal`, `long`, `ulong`, `uint`, `short`, `ushort`,
        /// `sbyte`, `byte`, and various collection types containing `byte`, `char`, and `int`.
        /// Each index type corresponds to an overload of the indexer, enabling flexible access
        /// to numeral representations within the system.
        /// The indexer utilizes the specified index to calculate and return the corresponding
        /// `Numeral` object, which represents a particular value within the numeral system's size
        /// and rules of conversion.
        public Numeral this[ushort index]
        {
            get
            {
                var integral = ULong.ToIndicesOfBase((ulong)index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), new List<int>(), true);
            }
        }

        /// Represents a numeral system with specified properties and methods for handling numerals.
        /// Provides access to numerals using various index types including integers, decimals, and collections.
        public Numeral this[sbyte index]
        {
            get
            {
                var integral = ULong.ToIndicesOfBase((ulong)index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), new List<int>(), index >= 0);
            }
        }

        /// <summary>
        /// Numeral indexer that accepts an input of type <c>byte</c>.
        /// </summary>
        /// <param name="index">The byte value used to index into the numeral system.</param>
        /// <returns>The <see cref="Numeral"/> representation corresponding to the specified byte index.</returns>
        /// <remarks>
        /// Utilizes the base size of the numeral system to convert the byte index into a numeral.
        /// </remarks>
        public Numeral this[byte index]
        {
            get
            {
                var integral = ULong.ToIndicesOfBase((ulong)index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), new List<int>(), true);
            }
        }

        /// Represents a numeral system, which allows conversion and parsing of numbers based on custom sets of numeral identities.
        public Numeral this[IEnumerable<byte> index] => new(this, index.Select(x => (int)x).ToList(), positive: true);

        /// Represents a numeral system used for parsing and handling numbers with a given size.
        public Numeral this[IEnumerable<char> index] => new(this, index.Select(x => (int)x).ToList(), positive: true);

        /// Represents a numerical system that provides operations for parsing
        /// and converting numeral representations utilizing a specified identity and delimiter settings.
        public Numeral this[List<int> index] => new(this, index, positive: true);

        /// <summary>
        /// Represents a numeral system with customizable settings for numeral parsing and conversion.
        /// </summary>
        public Numeral this[IList<char> index] => new(this, index.Select(x => (int)x).ToList(), positive: true);

        /// <summary>
        /// Accesses a numeral in the numeral system using an integer index.
        /// </summary>
        /// <remarks>
        /// The index must be within the bounds of the numeral system.
        /// </remarks>
        /// <param name="index">The integer index to access the numeral.</param>
        /// <returns>The numeral corresponding to the specified index.</returns>
        public Numeral this[IList<byte> index] => new(this, index.Select(x => (int)x).ToList(), positive: true);

        /// Provides a means to work with different numeral systems, allowing for parsing, indexing, and conversion operations
        /// within a specified numeral system size.
        public Numeral this[IEnumerable<int> index] => new(this, index.ToList(), positive: true);

        /// <summary>
        /// Attempts to convert a list of indices representing the digits of a number
        /// within this numeral system to its corresponding integer value.
        /// </summary>
        /// <param name="indices">A list of integers representing the indices of the digits.</param>
        /// <param name="result">The integer value obtained from the indices if conversion is successful; otherwise, zero.</param>
        /// <param name="positive">Determines whether the resulting integer should be positive; defaults to true.</param>
        /// <returns>True if the conversion is successful and no adjustments were necessary; otherwise, false if adjusted to the nearest possible value.</returns>
        public bool TryIntegerOf(IList<int> indices, out int result, bool positive = true)
        {
            result = 0;
            var success = true;
            var ind = indices;
            if (null == indices || indices.Count == 0)
            {
                ind = new List<int> { 0 };
            }
            else if (!Contains(indices))
            {
                ind = indices.Select(x => x < Size && x >= 0 ? x : 0).ToList();
                success = false;
            }

            result = ind.Select((t, i) => t * Convert.ToInt32(Math.Pow(Size, (ind.Count() - 1 - i)))).Sum();
            result = positive ? result : -result;
            return success;
        }

        /// <summary>
        /// Attempts to convert the given indices of numeric digits into a character representation.
        /// </summary>
        /// <param name="indices">A list containing the indices of the digits in the numeral system.</param>
        /// <param name="result">The resulting character representation of the indices if the conversion is successful.</param>
        /// <param name="positive">A boolean indicating whether the number represented by the indices is positive.</param>
        /// <returns>True if the conversion is successful; otherwise, false if it was only approximated to the nearest possible char value.</returns>
        public bool TryCharOf(IList<int> indices, out char result, bool positive = true)
        {
            result = char.MinValue;
            var success = TryIntegerOf(indices, out var integer, positive);
            if (success)
            {
                result = (char)integer;
            }

            return success;
        }

        /// <summary>
        /// Tries to convert a list of string indices into their corresponding integer value,
        /// using specified numeral system configuration settings such as identity mappings
        /// and separators.
        /// </summary>
        /// <param name="indices">A list of string representations of numeral indices.</param>
        /// <param name="identity">A list of strings that represent the numeral mappings in the system.</param>
        /// <param name="separator">The character used to separate numerals in the input list.</param>
        /// <param name="negativeSign">The symbol that denotes a negative number.</param>
        /// <param name="numberDecimalSeparator">The character used to separate the integer and fractional parts of a number.</param>
        /// <param name="result">Outputs the integer value derived from the provided indices.</param>
        /// <param name="positive">A boolean flag indicating whether the result should be positive;
        /// defaults to true.</param>
        /// <returns>True if the indices successfully convert to an integer; false if the conversion fails or
        /// has to be approximated to the nearest zero value.</returns>
        public bool TryIntegerOf(IList<string> indices, IList<string> identity, string separator, string negativeSign,
            string numberDecimalSeparator, out int result, bool positive = true)
        {
            result = 0;
            IList<string> ind;
            if (indices.Count == 0)
            {
                ind = new List<string> { identity[0] };
            }
            else
            {
                if (indices[0] == negativeSign)
                {
                    positive = false;
                }

                ind = indices
                    .SkipWhile(x => x.Equals(negativeSign))
                    .TakeWhile(x => !x.Equals(numberDecimalSeparator))
                    .Where(x => !x.Equals(separator))
                    .ToList();
            }

            return TryIntegerOf(ind.Select(identity.IndexOf).ToList(), out result, positive);
        }

        /// <summary>
        /// Attempts to convert a string representation of a number in a given numeral system to its integer equivalent.
        /// </summary>
        /// <param name="value">The string representation of the number to convert.</param>
        /// <param name="identity">The list of string representations of each digit in the numeral system.</param>
        /// <param name="separator">The separator used between digits in the string representation.</param>
        /// <param name="negativeSign">The character representing a negative sign in the numeral system.</param>
        /// <param name="numberDecimalSeparator">The character used as the decimal separator in the string representation.</param>
        /// <param name="integral">If the conversion is successful, this will contain the integer equivalent of the number.</param>
        /// <param name="positive">Indicates whether the number should be treated as positive if no negative sign is found.</param>
        /// <returns>True if the string was converted successfully; otherwise, false.</returns>
        public bool TryIntegerOf(string value, IList<string> identity, string separator, string negativeSign,
            string numberDecimalSeparator, out int integral, bool positive = true)
        {
            integral = 0;
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            //var integral = string.Join(string.Empty, value.Split(NumberDecimalSeparator).First());
            //var test = integral.SplitAndKeep(Identity.ToArray());
            var success = TrySplitNumberIndices(value, identity, separator, negativeSign, numberDecimalSeparator,
                out var result);
            var test = TryIntegerOf(result.integralIndices, out integral, result.positive | positive);
            return success && test;
        }

        /// The `SerializationInfo` class contains information necessary for the serialization
        /// and deserialization of numerals in a given numeral system. It maintains the identity
        /// components and formatting details such as separators and signs used in numerical representations.
        public class SerializationInfo
        {
            /// Gets or sets the identity of the numeral system used for serialization.
            /// The identity represents a list of characters or strings that uniquely define
            /// the numerals in the system.
            public List<string> Identity { get; set; } = new();

            /// <summary>
            /// Gets or sets the separator string used in the numeral system.
            /// </summary>
            public string Separator { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the string used to denote negative numbers in the numeral system.
            /// </summary>
            public string NegativeSign { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the string that represents the decimal separator in a numeral system.
            /// </summary>
            public string NumberDecimalSeparator { get; set; } = string.Empty;

            /// <summary>
            /// Generates the serialization information for a numeral system of the given size.
            /// </summary>
            /// <param name="size">The size of the numeral system.</param>
            /// <returns>The serialization information for the numeral system.</returns>
            public static SerializationInfo OfBase(int size)
            {
                var printableIdentity = Numeral.System.Characters.Printable.ToList();
                var cultureInfo = CultureInfo.CurrentCulture;
                var negativeSign = cultureInfo.NumberFormat.NegativeSign;
                var numberDecimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;
                var separator = size < printableIdentity.Count ? string.Empty : printableIdentity[^1].ToString();
                var identity = printableIdentity
                    .Take(size)
                    .Where(x =>
                        !negativeSign.Contains(x) &&
                        !numberDecimalSeparator.Contains(x) &&
                        !separator.Contains(x)
                    )
                    .Select(c => c.ToString(cultureInfo)).ToList();
                if (identity.Count() < size)
                {
                    identity = identity.Concat(Enumerable.Range(identity.Count(), size - identity.Count())
                        .Select(i => i.ToString(cultureInfo))).ToList();
                }

                return new SerializationInfo
                {
                    Identity = identity,
                    Separator = separator,
                    NegativeSign = negativeSign,
                    NumberDecimalSeparator = numberDecimalSeparator
                };
            }
        }

        /// <summary>
        /// Parses a string representation of a number into a Numeral object.
        /// </summary>
        /// <param name="val">The string representation of the number to parse.</param>
        /// <param name="identity">The list of strings representing the identity of the numeral system.</param>
        /// <param name="separator">The separator used to distinguish separate digits in the number string.</param>
        /// <param name="negativeSign">The string representation of the negative sign.</param>
        /// <param name="numberDecimalSeparator">The decimal separator used in the number string.</param>
        /// <returns>A Numeral object representing the parsed number.</returns>
        public Numeral Parse(string toString, SerializationInfo serializationInfo) => Parse(toString,
            serializationInfo.Identity, serializationInfo.Separator, serializationInfo.NegativeSign,
            serializationInfo.NumberDecimalSeparator);

        /// <summary>
        /// Parses a string representation of a numeral in the current numeral system.
        /// </summary>
        /// <param name="toString">The string representation of the numeral.</param>
        /// <returns>A Numeral object representing the parsed numeral.</returns>
        public Numeral Parse(string toString)
        {
            var serializationInfo = SerializationInfo.OfBase(Size);
            return Parse(toString, serializationInfo);
        }
    }
}