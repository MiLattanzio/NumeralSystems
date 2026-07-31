using System;
using System.Globalization;

namespace NumeralSystems.Net
{
    /// <summary>
    /// Supplies an ordered alphabet and formatting tokens to numeral parsing and
    /// formatting APIs through the standard <see cref="IFormatProvider"/> model.
    /// </summary>
    public sealed class NumeralFormatInfo : IFormatProvider
    {
        private readonly NumberFormatInfo _numberFormat;

        /// <summary>Creates immutable formatting information.</summary>
        public NumeralFormatInfo(
            NumeralAlphabet alphabet,
            string digitSeparator = "",
            string negativeSign = "-",
            string decimalSeparator = ".")
        {
            Alphabet = alphabet ?? throw new ArgumentNullException(nameof(alphabet));
            DigitSeparator = digitSeparator ?? throw new ArgumentNullException(nameof(digitSeparator));
            NegativeSign = negativeSign ?? throw new ArgumentNullException(nameof(negativeSign));
            DecimalSeparator = decimalSeparator ?? throw new ArgumentNullException(nameof(decimalSeparator));
            Alphabet.ValidateFormat(DigitSeparator, NegativeSign, DecimalSeparator);

            var numberFormat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            numberFormat.NegativeSign = NegativeSign;
            numberFormat.NumberDecimalSeparator = DecimalSeparator;
            if (DigitSeparator.Length > 0)
                numberFormat.NumberGroupSeparator = DigitSeparator;
            _numberFormat = NumberFormatInfo.ReadOnly(numberFormat);
        }

        /// <summary>Gets the ordered alphabet.</summary>
        public NumeralAlphabet Alphabet { get; }

        /// <summary>Gets the token inserted between individual digit symbols.</summary>
        public string DigitSeparator { get; }

        /// <summary>Gets the negative-sign token.</summary>
        public string NegativeSign { get; }

        /// <summary>Gets the radix-point token.</summary>
        public string DecimalSeparator { get; }

        /// <summary>
        /// Creates formatting information for a base using sign and decimal
        /// tokens from another provider.
        /// </summary>
        public static NumeralFormatInfo ForBase(
            int baseValue,
            IFormatProvider provider = null,
            string digitSeparator = "") =>
            ForAlphabet(NumeralAlphabet.CreateDefault(baseValue), provider, digitSeparator);

        /// <summary>
        /// Creates formatting information for an alphabet using sign and decimal
        /// tokens from another provider.
        /// </summary>
        public static NumeralFormatInfo ForAlphabet(
            NumeralAlphabet alphabet,
            IFormatProvider provider = null,
            string digitSeparator = "")
        {
            if (alphabet == null) throw new ArgumentNullException(nameof(alphabet));
            if (digitSeparator == null) throw new ArgumentNullException(nameof(digitSeparator));
            if (provider is NumeralFormatInfo existing)
            {
                if (existing.Alphabet.Count != alphabet.Count)
                    throw new ArgumentException(
                        "The supplied numeral format uses a different base.",
                        nameof(provider));
                return new NumeralFormatInfo(
                    alphabet,
                    digitSeparator.Length == 0 ? existing.DigitSeparator : digitSeparator,
                    existing.NegativeSign,
                    existing.DecimalSeparator);
            }

            var numberFormat = NumberFormatInfo.GetInstance(provider ?? CultureInfo.CurrentCulture);
            return new NumeralFormatInfo(
                alphabet,
                digitSeparator,
                numberFormat.NegativeSign,
                numberFormat.NumberDecimalSeparator);
        }

        /// <inheritdoc />
        public object GetFormat(System.Type formatType)
        {
            if (formatType == typeof(NumeralFormatInfo)) return this;
            if (formatType == typeof(NumberFormatInfo)) return _numberFormat;
            return null;
        }

        internal static NumeralFormatInfo Resolve(int baseValue, IFormatProvider provider)
        {
            if (provider?.GetFormat(typeof(NumeralFormatInfo)) is NumeralFormatInfo numeralFormat)
            {
                if (numeralFormat.Alphabet.Count != baseValue)
                    throw new FormatException(
                        $"The format alphabet has base {numeralFormat.Alphabet.Count}, expected {baseValue}.");
                return numeralFormat;
            }
            return ForBase(baseValue, provider);
        }
    }
}
