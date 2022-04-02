using System;
using System.Collections.Generic;

namespace NumeralSystems.Net.Utils
{
    public static class StringExtensions
    {
        public static List<string> SplitAndKeep(this string s, params string[] delims)
        {
            if (string.IsNullOrEmpty(s)) return new();
            var rows = new List<string> { s };
            foreach (var delim in delims)
            {
                for (var i = 0; i < rows.Count; i++)
                {
                    var index = rows[i].IndexOf(delim, StringComparison.Ordinal);
                    if (index <= -1 || rows[i].Length <= index + 1) continue;
                    var leftPart = rows[i][..(index + delim.Length)];
                    var rightPart = rows[i][(index + delim.Length)..];
                    rows[i] = leftPart;
                    rows.Insert(i + 1, rightPart);
                }
            }
            return rows;
        }
    }
}