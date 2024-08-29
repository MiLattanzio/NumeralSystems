using System;
using System.Collections.Generic;
using System.Linq;

namespace NumeralSystems.Net.Utils
{
    public static class StringExtensions
    {
        /// <summary>
        /// Provides extension methods for string manipulation.
        /// </summary>
        public static IEnumerable<string> TakeOnly(this string input, params string[] delimiters) => input.SplitAndKeep(delimiters).Where(delimiters.Contains);

        /// <summary>
        /// Removes the specified delimiters from the input string and returns the resulting substrings.
        /// </summary>
        /// <param name="input">The input string to remove delimiters from.</param>
        /// <param name="delimiters">The delimiters to remove from the input string.</param>
        /// <returns>An enumerable collection of substrings obtained by removing the specified delimiters from the input string.</returns>
        public static IEnumerable<string> Remove(this string input, params string[] delimiters) => input.SplitAndKeep(delimiters).Where(x => !delimiters.Contains(x));

        /// <summary>
        /// Splits a string based on the given delimiters and keeps the delimiters in the result.
        /// </summary>
        /// <param name="input">The input string to be split.</param>
        /// <param name="delimiters">The delimiters to split the input string by.</param>
        /// <returns>An array of strings containing the split parts of the input string.</returns>
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

        /// <summary>
        /// Finds the index of the first occurrence of any delimiter in a given string starting from a specified index.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="startIndex">The index to start searching from.</param>
        /// <param name="delimiters">The array of delimiters to search for.</param>
        /// <returns>
        /// The index of the first occurrence of any delimiter in the input string starting from the specified index, or -1 if no delimiter is found.
        /// </returns>
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