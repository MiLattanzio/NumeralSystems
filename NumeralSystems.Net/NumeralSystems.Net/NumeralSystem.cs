using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using NumeralSystems.Net.Utils;
using NumeralSystems.Net.Utils.Encode;
using Math = System.Math;
using Convert = System.Convert;
using Decimal = NumeralSystems.Net.Utils.Encode.Decimal;
using Double = System.Double;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable HeapView.ObjectAllocation.Evident
// ReSharper disable MemberCanBePrivate.Global
namespace NumeralSystems.Net
{
    [SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NumeralSystem
    {
       
        /// <summary>
        /// Size of the numeral system.
        /// <para>For example, for base 10 it is 10</para>
        /// </summary>
        public int Size { get; }
        
        
        /// <summary>
        /// If true, the parsing will skip the unknown values during the parsing, otherwise they will be replaced with zeros.
        /// Example: for base 10, the value 1;1 will be parsed as 11 if true, otherwhise it will be parsed as 101.
        /// </summary>
        public bool SkipUnknownValues { get; set; }
        
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        public NumeralSystem(int size)
        {
            if (size <= 0) throw new Exception("Size cannot be less than 1");
           Size = size;
        }

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
        
        
        public Numeral Parse(string val, IList<string> identity, string separator, string negativeSign, string numberDecimalSeparator)
        {
            if (!TrySplitNumberIndices(val, identity, separator, negativeSign, numberDecimalSeparator,  out var result)) throw new InvalidOperationException($"'{val}' is not a valid numeral");
            var (positive, integralIndices, fractionalIndices, _, _) = result;
            return new Numeral(this, integralIndices, fractionalIndices, positive);
        }
        
        public bool TryParse(string value, IList<string> identity, string separator, string negativeSign, string numberDecimalSeparator, out Numeral result)
        {
            var success = TrySplitNumberIndices(value, identity, separator, negativeSign, numberDecimalSeparator, out var r);
            var (positive, integralIndices, fractionalIndices, _, _) = r;
            result = new Numeral(this, integralIndices, fractionalIndices, positive);
            return success;
        }

        public bool Contains(IList<int> value) => (null != value && value.ToList().All(x => x >= 0 && x < Size));
        private List<int> IntegralIndicesOf(ulong value) => ULong.ToIndicesOfBase(value, Size).Select(x => (int)x).ToList();
        private List<int> IntegralIndicesOf(double value) => Utils.Encode.Double.ToIndicesOfBase(value, Size).Integral.Select(x => (int)x).ToList();
        private List<int> IntegralIndicesOf(decimal value) => Utils.Encode.Decimal.ToIndicesOfBase(value, Size).Integral.Select(x => (int)x).ToList();
        private List<int> FractionalIndicesOf(double value) => Utils.Encode.Double.ToIndicesOfBase(value, Size).Fractional.Select(x => (int)x).ToList();
        private List<int> FractionalIndicesOf(decimal value) => Utils.Encode.Decimal.ToIndicesOfBase(value, Size).Fractional.Select(x => (int)x).ToList();
        
        public Numeral this[int index] => new (this, IntegralIndicesOf((ulong) Math.Abs(index)), new List<int>(), index>=0);
        public Numeral this[double index] => new (this, IntegralIndicesOf(index), FractionalIndicesOf(index), index > 0);

        public Numeral this[decimal index] => new (this, IntegralIndicesOf(index), FractionalIndicesOf(index), index > 0);

        public Numeral this[long index] => new(this, IntegralIndicesOf((ulong) Math.Abs(index)), positive: index > 0);

        public Numeral this[ulong index] => new(this, IntegralIndicesOf(index), positive: true);

        public Numeral this[uint index] => new(this, IntegralIndicesOf(index), positive: true);

        public Numeral this[short index] => new(this, IntegralIndicesOf((ulong) Math.Abs(index)), positive: index > 0);

        public Numeral this[ushort index] => new(this, IntegralIndicesOf(index), positive: true);

        public Numeral this[sbyte index] => new(this, IntegralIndicesOf((ulong) Math.Abs(index)), positive: index > 0);

        public Numeral this[byte index] => new(this, IntegralIndicesOf(index), positive: true);

        public Numeral this[IEnumerable<byte> index] => new(this, index.Select(x => (int)x).ToList(), positive: true);

        public Numeral this[IEnumerable<char> index] => new(this, index.Select(x => (int)x).ToList(), positive: true);

        public Numeral this[List<int> index] => new(this, index, positive: true);

        public Numeral this[IList<char> index] => new(this, index.Select(x => (int)x).ToList(), positive: true);

        public Numeral this[IList<byte> index] => new(this, index.Select(x => (int)x).ToList(), positive: true);

        public Numeral this[IEnumerable<int> index] => new(this, index.ToList(), positive: true);

        /// <summary>
        /// Try to get the integer value of the indices
        /// </summary>
        /// <param name="indices">Indices of the digits of the number</param>
        /// <param name="result">Integer value representing the indices of the digits</param>
        /// <param name="positive"></param>
        /// <returns>True If the conversion succeds, False if it was approssimated to the nearest 0 possible value</returns>
        public bool TryIntegerOf(IList<int> indices, out int result, bool positive = true)
        {
            result = 0; 
            var success = true;
            var ind = indices;
            if (null == indices || indices.Count == 0)
            {
                ind = new List<int> {0};
            } else if (!Contains(indices))
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
        /// <param name="positive"></param>
        /// <returns>True If the conversion succeds, False if it was approssimated to the nearest 0 possible value</returns>
        public bool TryCharOf(IList<int> indices, out char result, bool positive = true)
        {
            result = char.MinValue;
            var success = TryIntegerOf(indices, out var integer, positive);
            if (success)
            {
                result = (char) integer;
            }
            return success;
        }

        /// <summary>
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
        public bool TryIntegerOf(IList<string> indices, IList<string> identity, string separator, string negativeSign, string numberDecimalSeparator,  out int result, bool positive = true)
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
            public string NumberDecimalSeparator { get; set; } = string.Empty;

            public static SerializationInfo OfBase(int size)
            {
                const int numberCount = 10;
                const int lettersLowerCount = 26;
                var cultureInfo = CultureInfo.CurrentCulture;
                var negativeSign = cultureInfo.NumberFormat.NegativeSign;
                var numberDecimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;
                var separator = size switch
                {
                    < numberCount + lettersLowerCount * 2 => string.Empty,
                    _ => cultureInfo.NumberFormat.NumberGroupSeparator
                };
                var identity = size switch
                {
                    < 10 => Enumerable.Range(0, size).Select(i => i.ToString(cultureInfo)).ToList(),
                    _ => Numeral.System.Characters.All
                        .Take(size)
                        .Where(x =>
                            !negativeSign.Contains(x) &&
                            !numberDecimalSeparator.Contains(x) &&
                            !separator.Contains(x)
                        )
                        .Select(c => c.ToString(cultureInfo)).ToList(),
                };
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
        
        public Numeral Parse(string toString, SerializationInfo serializationInfo) => Parse(toString, serializationInfo.Identity, serializationInfo.Separator, serializationInfo.NegativeSign, serializationInfo.NumberDecimalSeparator);
    }
}