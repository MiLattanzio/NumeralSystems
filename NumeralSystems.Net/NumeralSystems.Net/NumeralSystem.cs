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
    /// The `NumeralSystem` class represents a numeral system with a specified size.
    /// It provides various methods to parse and manipulate numbers in the numeral system.
    /// /
    [SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NumeralSystem
    {
        /// Gets the size of the numeral system.
        /// </summary>
        public int Size { get; }


        /// Gets or sets a value indicating whether unknown values should be skipped when splitting or converting numbers.
        /// </summary>
        public bool SkipUnknownValues { get; set; }

        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        /// <summary>
        /// The <c>NumeralSystem</c> class represents a numeral system with a specific size. It provides functionality to convert numbers to and from the indices of the digits in the numeral system.
        /// </summary>
        public NumeralSystem(int size)
        {
            if (size <= 0) throw new Exception("Size cannot be less than 1");
           Size = size;
        }

        /// Tries to split the number string into its integral and fractional parts using given parameters
        /// </summary>
        /// <param name="val">The number string to split</param>
        /// <param name="identity">The list of strings representing the numeral system identity</param>
        /// <param name="separator">The separator string to split the integral and fractional parts. If null or empty, the identity strings will be used instead</param>
        /// <param name="negativeSign">The string representing the negative sign</param>
        /// <param name="numberDecimalSeparator">The string representing the decimal separator</param>
        /// <param name="result">Tuple containing the splitting results:
        /// - positive: boolean representing the sign of the number
        /// - integralIndices: list of integral indices representing the digits
        /// - fractionalIndices: list of fractional indices representing the digits
        /// - integral: string representing the integral part of the number
        /// - fractional: string representing the fractional part of the number</param>
        /// <returns>True if the splitting was successful, otherwise false</returns>
        public bool TrySplitNumberIndices(string val, IList<string> identity, string separator, string negativeSign, string numberDecimalSeparator, out (bool positive, List<int> integralIndices, List<int> fractionalIndices, string integral, string fractional) result)
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

        /// Tries to convert a list of integral and fractional indices to a string representation in a given numeral system.
        /// </summary>
        /// <param name="integralIndices">A list of integral indices representing the digits of the number</param>
        /// <param name="fractionalIndices">A list of fractional indices representing the digits after the decimal separator</param>
        /// <param name="identity">A list of strings representing the symbols or digits in the numeral system</param>
        /// <param name="separator">The separator used between integral and fractional parts of the number</param>
        /// <param name="negativeSign">The string representing the negative sign</param>
        /// <param name="numberDecimalSeparator">The string representing the decimal separator</param>
        /// <param name="result">The string representation of the number in the given numeral system</param>
        /// <param name="positive">A boolean value indicating whether the number is positive or negative (default is true)</param>
        /// <returns>True if the conversion succeeds, False if the conversion was approximate</returns>
        public bool TryFromIndices(List<int> integralIndices, List<int> fractionalIndices, IList<string> identity, string separator, string negativeSign, string numberDecimalSeparator, out string result, bool positive = true)
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
        public Numeral Parse(string val, IList<string> identity, string separator, string negativeSign, string numberDecimalSeparator)
        {
            if (!TrySplitNumberIndices(val, identity, separator, negativeSign, numberDecimalSeparator,  out var result)) throw new InvalidOperationException($"'{val}' is not a valid numeral");
            var (positive, integralIndices, fractionalIndices, _, _) = result;
            return new Numeral(this, integralIndices, fractionalIndices, positive);
        }

        /// Tries to parse a string representation of a number using the given numeral system.
        /// </summary>
        /// <param name="value">The string value to parse.</param>
        /// <param name="identity">The list of identities of the digits in the numeral system.</param>
        /// <param name="separator">The separator used in the string value.</param>
        /// <param name="negativeSign">The negative sign used in the string value.</param>
        /// <param name="numberDecimalSeparator">The decimal separator used in the string value.</param>
        /// <param name="result">The parsed Numeral object if successful, otherwise null.</param>
        /// <returns>True if the parsing was successful, otherwise false.</returns>
        public bool TryParse(string value, IList<string> identity, string separator, string negativeSign, string numberDecimalSeparator, out Numeral result)
        {
            var success = TrySplitNumberIndices(value, identity, separator, negativeSign, numberDecimalSeparator, out var r);
            var (positive, integralIndices, fractionalIndices, _, _) = r;
            result = new Numeral(this, integralIndices, fractionalIndices, positive);
            return success;
        }

        /// Determines whether the given list of indices is a valid representation in the numeral system.
        /// </summary>
        /// <param name="value">The list of indices</param>
        /// <returns>True if the list of indices represents a valid value in the numeral system, false otherwise</returns>
        public bool Contains(IList<int> value) => (null != value && value.ToList().All(x => x >= 0 && x < Size));

        public Numeral this[int index]
        {
            get
            {
                var integral = UInt.ToIndicesOfBase(index, Size, out var positive);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), new List<int>(), index >= 0);
            }
        }

        /// <summary>
        /// Represents a numeral system with a specified size.
        /// </summary>
        /// <remarks>
        /// The <c>NumeralSystem</c> class allows operations on numbers in a specific numeral system.
        /// It provides methods to retrieve numerals at different indices in different data types.
        /// </remarks>
        public Numeral this[double index]
        {
            get
            {
                var (integral, fractional, positive) = Type.Base.Double.ToIndicesOfBase(index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), fractional.Select(x => (int)x).ToList(), positive);
            }
        }

        /// <summary>
        /// Represents a numeral system.
        /// </summary>
        public Numeral this[decimal index]
        {
            get
            {
                var (integral, fractional, positive) = Type.Base.Decimal.ToIndicesOfBase(index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), fractional.Select(x => (int)x).ToList(), positive);
            }
        }

        /// <summary>
        /// Represents a numeral system with a specified size.
        /// </summary>
        /// <remarks>
        /// The NumeralSystem class provides functionality for converting numbers to and from a specified numeral system.
        /// </remarks>
        public Numeral this[long index]
        {
            get
            {
                var integral = ULong.ToIndicesOfBase((ulong)index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), new List<int>(), index >= 0);
            }
        }

        /// <summary>
        /// Gets the size of the numeral system.
        /// </summary>
        public Numeral this[ulong index]
        {
            get
            {
                var integral = ULong.ToIndicesOfBase(index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), new List<int>(), true);
            }
        }

        /// <summary>
        /// Gets the size of the numeral system.
        /// </summary>
        public Numeral this[uint index]
        {
            get
            {
                var integral = ULong.ToIndicesOfBase(index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), new List<int>(), true);
            }
        }

        /// <summary>
        /// Gets the size of the numeral system.
        /// </summary>
        public Numeral this[short index]
        {
            get
            {
                var integral = ULong.ToIndicesOfBase((ulong)index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), new List<int>(), index >= 0);
            }
        }

        /// The `NumeralSystem` class represents a numeral system with a specified size.
        /// It is used to parse and format numbers in the specified numeral system.
        /// /
        public Numeral this[ushort index]
        {
            get
            {
                var integral = ULong.ToIndicesOfBase((ulong)index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), new List<int>(), true);
            }
        }

        /// The `NumeralSystem` class represents a numeral system used for converting numbers to and from different bases.
        /// It provides methods for splitting numbers into integral and fractional parts, parsing numbers from string representations,
        /// and accessing numerals using different index types.
        public Numeral this[sbyte index]
        {
            get 
            {
                var integral = ULong.ToIndicesOfBase((ulong)index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), new List<int>(), index >= 0);
            }
        }

        /// *Properties:**
        public Numeral this[byte index]
        {
            get
            {
                var integral = ULong.ToIndicesOfBase((ulong)index, Size);
                return new Numeral(this, integral.Select(x => (int)x).ToList(), new List<int>(), true);
            }
        }

        /// <summary>
        /// Represents a numeral system.
        /// </summary>
        /// <remarks>
        /// This class provides functionality to define and manipulate numeral systems.
        /// </remarks>
        public Numeral this[IEnumerable<byte> index] => new(this, index.Select(x => (int)x).ToList(), positive: true);

        /// The `NumeralSystem` class represents a numeral system with a specified base size.
        /// @remarks
        /// This class provides methods to perform operations on numbers in the specified numeral system.
        /// @example
        /// ```csharp
        /// var numeralSystem = new NumeralSystem(16);
        /// var numeral = numeralSystem.Parse("1A");
        /// ```
        /// /
        public Numeral this[IEnumerable<char> index] => new(this, index.Select(x => (int)x).ToList(), positive: true);

        /// The `NumeralSystem` class represents a numeral system that can be used to parse and manipulate numbers in different bases.
        /// @since 1.0.0
        /// /
        public Numeral this[List<int> index] => new(this, index, positive: true);

        /// The `NumeralSystem` class represents a numeral system with a specified size.
        /// It provides methods to parse and manipulate numbers in the given numeral system.
        /// @constructor NumeralSystem
        /// @param {number} size - The size of the numeral system.
        /// @property {number} Size - The size of the numeral system.
        /// @property {boolean} SkipUnknownValues - A flag indicating whether to skip unknown values.
        /// @method TrySplitNumberIndices - Tries to split a number string into its integral and fractional parts and returns the result.
        /// @param {string} val - The number string to split.
        /// @param {Array<string>} identity - The identity list of the numeral system.
        /// @param {string} separator - The separator character or string.
        /// @param {string} negativeSign - The negative sign character or string.
        /// @param {string} numberDecimalSeparator - The decimal separator character or string.
        /// @returns {Object} - An object containing the split result.
        /// @method TryFromIndices - Tries to construct a number string from its integral and fractional parts using the given indices and returns the result.
        /// @param {Array<number>} integralIndices - The indices of the integral part.
        /// @param {Array<number>} fractionalIndices - The indices of the fractional part.
        /// @param {Array<string>} identity - The identity list of the numeral system.
        /// @param {string} separator - The separator character or string.
        /// @param {string} negativeSign - The negative sign character or string.
        /// @param {string} numberDecimalSeparator - The decimal separator character or string.
        /// @param {boolean} positive - A flag indicating whether the constructed number is positive. Default is `true`.
        /// @returns {string} - The constructed number string.
        /// @method Parse - Parses a number string in the given numeral system and returns a `Numeral` object.
        /// @param {string} val - The number string to parse.
        /// @param {Array<string>} identity - The identity list of the numeral system.
        /// @param {string} separator - The separator character or string.
        /// @param {string} negativeSign - The negative sign character or string.
        /// @param {string} numberDecimalSeparator - The decimal separator character or string.
        /// @returns {Numeral} - The parsed `Numeral` object.
        /// @method TryParse - Tries to parse a number string in the given numeral system and returns a boolean indicating whether the parsing was successful.
        /// @param {string} value - The number string to parse.
        /// @param {Array<string>} identity - The identity list of the numeral system.
        /// @param {string} separator - The separator character or string.
        /// @param {string} negativeSign - The negative sign character or string.
        /// @param {string} numberDecimalSeparator - The decimal separator character or string.
        /// @param {Numeral} result - The parsed `Numeral` object.
        /// @returns {boolean} - A boolean indicating whether the parsing was successful.
        /// @method Contains - Checks if the given list of indices is valid for the numeral system.
        /// @param {Array<number>} value - The list of indices to check.
        /// @returns {boolean} - A boolean indicating whether the indices are valid.
        /// @method this[int index] - Gets the `Numeral` object at the specified index.
        /// @param {number} index - The index of the `Numeral` object to get.
        /// @returns {Numeral} - The `Numeral` object at the specified index.
        /// @method this[double index] - Gets the `Numeral` object at the specified index.
        /// @param {number} index - The index of the `Numeral` object to get.
        /// @returns {Numeral} - The `Numeral` object at the specified index.
        /// @method this[decimal index] - Gets the `Numeral` object at the specified index.
        /// @param {number} index - The index of the `Numeral` object to get.
        /// @returns {Numeral} - The `Numeral` object at the specified index.
        /// @method this[long index] - Gets the `Numeral` object at the specified index.
        /// @param {number} index - The index of the `Numeral` object to get.
        /// @returns {Numeral} - The `Numeral` object at the specified index.
        /// @method this[ulong index] - Gets the `Numeral` object at the specified index.
        /// @param {number} index - The index of the `Numeral` object to get.
        /// @returns {Numeral} - The `Numeral` object at the specified index.
        /// @method this[uint index] - Gets the `Numeral` object at the specified index.
        /// @param {number} index - The index of the `Numeral` object to get.
        /// @returns {Numeral} - The `Numeral` object at the specified index.
        /// @method this[short index] - Gets the `Numeral` object at the specified index.
        /// @param {number} index - The index of the `Numeral` object to get.
        /// @returns {Numeral} - The `Numeral` object at the specified index.
        /// @method this[ushort index] - Gets the `Numeral` object at the specified index.
        /// @param {number} index - The index of the `Numeral` object to get.
        /// @returns {Numeral} - The `Numeral` object at the specified index.
        /// @method this[sbyte index] - Gets the `Numeral` object at the specified index.
        /// @param {number} index - The index of the `Numeral` object to get.
        /// @returns {Numeral} - The `Numeral` object at the specified index.
        /// @method this[byte index] - Gets the `Numeral` object at the specified index.
        /// @param {number} index - The index of the `Numeral` object to get.
        /// @returns {Numeral} - The `Numeral` object at the specified index.
        /// @method this[IEnumerable<byte> index] - Gets a new `Numeral` object using the specified indices.
        /// @param {Iterable<number>} index - The indices of the `Numeral` object to get.
        /// @returns {Numeral} - A new `Numeral` object.
        /// @method this[IEnumerable<char> index] - Gets a new `Numeral` object using the specified indices.
        /// @param {Iterable<number>} index - The indices of the `Numeral` object to get.
        /// @returns {Numeral} - A new `Numeral` object.
        /// @method this[List<int> index] - Gets a new `Numeral` object using the specified indices.
        /// @param {Array<number>} index - The indices of the `Numeral` object to get.
        /// @returns {Numeral} - A new `Numeral` object.
        /// @method this[IList<char> index] - Gets a new `Numeral` object using the specified indices.
        /// @param {Array<number>} index - The indices of the `Numeral` object to get.
        /// @returns {Numeral} - A new `Numeral` object.
        /// @method this[IList<byte> index] - Gets a new `Numeral` object using the specified indices.
        /// @param {Array<number>} index - The indices of the `Numeral` object to get.
        /// @returns {Numeral} - A new `Numeral` object.
        /// @method this[IEnumerable<int> index] - Gets a new `Numeral` object using the specified indices.
        /// @param {Iterable<number>} index - The indices of the `Numeral` object to get.
        /// @returns {Numeral} - A new `Numeral` object.
        /// @method TryIntegerOf - Tries to convert a list of indices to an integer value and returns the result.
        /// @param {Array<number>} indices - The indices to convert.
        /// @param {number} result - The result of the conversion.
        /// @param {boolean} positive - A flag indicating whether the resulting integer value should be positive. Default is `true`.
        /// @returns {boolean} - A boolean indicating whether the conversion was successful.
        /// @method TryCharOf - Tries to convert a list of indices to a character value and returns the result.
        /// @param {Array<number>} indices - The indices to convert.
        /// @param {string} result - The result of the conversion.
        /// @param {boolean} positive - A flag indicating whether the resulting character value should be positive. Default is `true`.
        /// @returns {boolean} - A boolean indicating whether the conversion was successful.
        /// @method TryIntegerOf - Tries to convert a list of indices to an integer value and returns the result.
        /// @param {Array<string>} indices - The indices to convert.
        /// @param {Array<string>} identity - The identity list of the numeral system.
        /// @param {string} separator - The separator character or string.
        /// @param {string} negativeSign - The negative sign character or string.
        /// @param {string} numberDecimalSeparator - The decimal separator character or string.
        /// @param {number} result - The result of the conversion.
        /// @param {boolean} positive - A flag indicating whether the resulting integer value should be positive. Default is `true`.
        /// @returns {boolean} - A boolean indicating whether the conversion was successful.
        /// @method TryIntegerOf - Tries to convert a number string to an integer value and returns the result.
        /// @param {string} value - The number string to convert.
        /// @param {Array<string>} identity - The identity list of the numeral system.
        /// @param {string} separator - The separator character or string.
        /// @param {string} negativeSign - The negative sign character or string.
        /// @param {string} numberDecimalSeparator - The decimal separator character or string.
        /// @param {number} integral - The integral part of the converted value.
        /// @param {boolean} positive - A flag indicating whether the resulting integer value should be positive. Default is `true`.
        /// @returns {boolean} - A boolean indicating whether the conversion was successful.
        /// /
        public Numeral this[IList<char> index] => new(this, index.Select(x => (int)x).ToList(), positive: true);

        /// *NumeralSystems.Net.NumeralSystem**
        public Numeral this[IList<byte> index] => new(this, index.Select(x => (int)x).ToList(), positive: true);

        public Numeral this[IEnumerable<int> index] => new(this, index.ToList(), positive: true);

        /// <summary>
        /// Try to get the integer value of the indices.
        /// </summary>
        /// <param name="indices">Indices of the digits of the number.</param>
        /// <param name="result">Integer value representing the indices of the digits.</param>
        /// <param name="positive">Flag indicating if the resulting integer should be positive (default is true).</param>
        /// <returns>True if the conversion succeeds, false if it was approximated to the nearest possible 0 value.</returns>
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
        /// Try to get the char value of the indices
        /// </summary>
        /// <param name="indices">Indices of the digits of the number</param>
        /// <param name="result">Char value representing the indices of the digits</param>
        /// <param name="positive">Boolean indicating if it is a positive number or not</param>
        /// <returns>True if the conversion succeeds, False if it was approximated to the nearest possible value</returns>
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

        /// Try to get the integer value of the indices
        /// </summary>
        /// <param name="indices">Indices of the digits of the number</param>
        /// <param name="result">Integer value representing the indices of the digits</param>
        /// <param name="positive">If the number is greater than 0</param>
        /// <param name="identity">The identity that represents the number at each index</param>
        /// <param name="separator">The separator of each number</param>
        /// <param name="negativeSign">The negative sign symbol</param>
        /// <param name="numberDecimalSeparator">The separator for integral and fractional part in a float</param>
        /// <returns>True If the conversion succeds, False if it was approssimated to the nearest 0 possible value</returns>
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
        
        public bool TryIntegerOf(string value, IList<string> identity, string separator, string negativeSign, string numberDecimalSeparator, out int integral, bool positive = true)
        {
            integral = 0;
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            
            //var integral = string.Join(string.Empty, value.Split(NumberDecimalSeparator).First());
            //var test = integral.SplitAndKeep(Identity.ToArray());
            var success = TrySplitNumberIndices(value, identity, separator, negativeSign, numberDecimalSeparator, out var result);
            var test = TryIntegerOf(result.integralIndices, out integral, result.positive | positive);
            return success && test;
        }
        
        public class SerializationInfo
        {
            public List<string> Identity { get; set; } = new ();
            public string Separator { get; set; } = string.Empty;
            public string NegativeSign { get; set; } = string.Empty;

            /// The string used to separate the whole number part from the fractional part of a decimal number.
            /// <para>For example, in the number 3.14, the decimal separator is ".".</para>
            /// </summary>
            public string NumberDecimalSeparator { get; set; } = string.Empty;

            /// Generates the serialization information for a numeral system of the given size.
            /// </summary>
            /// <param name="size">The size of the numeral system</param>
            /// <returns>The serialization information for the numeral system</returns>
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
                    identity = identity.Concat(Enumerable.Range(identity.Count(), size - identity.Count()).Select(i => i.ToString(cultureInfo))).ToList();
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

        /// Parses a string representation of a number into a Numeral object.
        /// </summary>
        /// <param name="val">The string representation of the number to parse.</param>
        /// <param name="identity">The list of strings representing the identity of the numeral system.</param>
        /// <param name="separator">The separator used to distinguish separate digits in the number string.</param>
        /// <param name="negativeSign">The string representation of the negative sign.</param>
        /// <param name="numberDecimalSeparator">The decimal separator used in the number string.</param>
        /// <returns>A Numeral object representing the parsed number.</returns>
        public Numeral Parse(string toString, SerializationInfo serializationInfo) => Parse(toString, serializationInfo.Identity, serializationInfo.Separator, serializationInfo.NegativeSign, serializationInfo.NumberDecimalSeparator);

        /// Parses a string representation of a numeral in the current numeral system.
        /// </summary>
        /// <param name="toString">String representation of the numeral</param>
        /// <returns>A Numeral object representing the parsed numeral</returns>
        public Numeral Parse(string toString)
        {
            var serializationInfo = SerializationInfo.OfBase(Size);
            return Parse(toString, serializationInfo);
        }
    }
}