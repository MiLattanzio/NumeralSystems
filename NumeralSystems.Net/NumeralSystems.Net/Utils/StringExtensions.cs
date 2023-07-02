using System;
using System.Collections.Generic;

namespace NumeralSystems.Net.Utils
{
    public static class StringExtensions
    {
        public static IEnumerable<string> SplitAndKeep(this string input, params string[] delimiters)
        {
            var result = new List<string>();
        
            var startIndex = 0;
            int delimiterIndex;
        
            while ((delimiterIndex = FindDelimiterIndex(input, startIndex, delimiters)) != -1)
            {
                if (delimiterIndex > startIndex)
                {
                    var substring = input.Substring(startIndex, delimiterIndex - startIndex);
                    result.Add(substring);
                }
            
                var delimiter = input.Substring(delimiterIndex, 1);
                result.Add(delimiter);
            
                startIndex = delimiterIndex + 1;
            }

            if (startIndex >= input.Length) return result.ToArray();
            var remainingSubstring = input[startIndex..];
            result.Add(remainingSubstring);

            return result.ToArray();
        }
    
        private static int FindDelimiterIndex(string input, int startIndex, IEnumerable<string> delimiters)
        {
            var minIndex = -1;
        
            foreach (var delimiter in delimiters)
            {
                var index = input.IndexOf(delimiter, startIndex, StringComparison.Ordinal);
            
                if (index != -1 && (minIndex == -1 || index < minIndex))
                {
                    minIndex = index;
                }
            }
        
            return minIndex;
        }
    }
}