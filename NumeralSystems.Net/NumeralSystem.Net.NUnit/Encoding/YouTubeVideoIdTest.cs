using System.Collections.Generic;
using System.Linq;
using NumeralSystems.Net;
using NUnit.Framework;

namespace NumeralSystem.Net.NUnit.Encoding
{
    [TestFixture]
    public class YouTubeVideoIdTest
    {
        private static readonly char[] YoutubeIdChars = {
            '-', '_',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' 
        };
    
        private static readonly char[] CleanYoutubeIdChars = {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' 
        };
        
        private static readonly NumeralSystems.Net.NumeralSystem YoutubeNumeralSystem = Numeral.System.OfBase(YoutubeIdChars.Length);
        private static readonly NumeralSystems.Net.NumeralSystem NumeralSystem = Numeral.System.OfBase(CleanYoutubeIdChars.Length);
        private static readonly NumeralAlphabet YoutubeAlphabet =
            new(YoutubeIdChars.Select(character => character.ToString()));
        private static readonly NumeralAlphabet CleanYoutubeAlphabet =
            new(CleanYoutubeIdChars.Select(character => character.ToString()));

        private static string EncodeIdNumeral(string youtubeId)
        {
            var number = YoutubeNumeralSystem.Parse(youtubeId, YoutubeAlphabet, string.Empty, "#", "^");
            var cleanNumber = number.To(NumeralSystem, NumeralConversionOptions.Default);
            var result = cleanNumber.ToString(CleanYoutubeAlphabet, string.Empty, "#", "^");
            return result;
        }

        private static string DecodeIdNumeral(string cleanId)
        {
            var cleanNumber = NumeralSystem.Parse(cleanId, CleanYoutubeAlphabet, string.Empty, "#", "^");
            var youtubeNumber = cleanNumber.To(YoutubeNumeralSystem, NumeralConversionOptions.Default);
            var youtubeId = youtubeNumber.ToString(YoutubeAlphabet, string.Empty, "#", "^");
            return youtubeId;
        }
        

        private static string EncodeId(string youtubeId)
        {
        
            var number = Value.FromString(youtubeId, YoutubeAlphabet);
            var cleanNumber = number.ToBase(CleanYoutubeIdChars.Length);
            var result = string.Concat(cleanNumber.Indices.Select(x => CleanYoutubeIdChars[x].ToString()));
            return result;
        }

        private static string DecodeId(string cleanId)
        {
            var cleanNumber = Value.FromString(cleanId, CleanYoutubeAlphabet);
            var youtubeNumber = cleanNumber.ToBase(YoutubeIdChars.Length);
            var youtubeId = string.Concat(youtubeNumber.Indices.Select(x => YoutubeIdChars[x].ToString()));
            return youtubeId;
        }
        
        [Test]
        public void TestVideoIdEncoding()
        {
            var id = "0SerEuqAlAA";
            var encoded = EncodeId(id);
            var decoded = DecodeId(encoded);
            Assert.AreEqual(id, decoded);
            var encodedNumeral  = EncodeIdNumeral(id);
            var decodedNumeral = DecodeIdNumeral(encodedNumeral);
            Assert.AreEqual(id, decodedNumeral);
        }
        
        [Test]
        public void TestVideoIdEval()
        {
            var id = "0SerEuqAlAA";
            var value = Value.FromString(id, YoutubeAlphabet);
            var numeralValue = NumeralValue.FromValue(value);
            var valueFromNumeral = numeralValue.ToValue().ToBase(YoutubeIdChars.Length);
            var youtubeId = string.Concat(valueFromNumeral.Indices.Select(x => YoutubeIdChars[x].ToString()));
            Assert.AreEqual(id, youtubeId);
            var bigInt = numeralValue.ToBigInteger();
            var fromBigInteger = NumeralValue.FromBigInteger(bigInt);
            Assert.AreEqual(bigInt, fromBigInteger.ToBigInteger());
            var decim = numeralValue.ToDecimal();
            var fromDecimal = NumeralValue.FromDecimal(decim);
            Assert.AreEqual(decim, fromDecimal.ToDecimal());
        }
        
    }
}
