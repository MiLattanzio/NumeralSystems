using System.Collections.Generic;
using System.Linq;

namespace NumeralSystems.Net.Ecoding
{
    public class String
    {
        /// <summary>
        /// Extracts the distinct characters from the string ordered by count.
        /// </summary>
        /// <param name="value">Value to extract the identity from</param>
        /// <returns>The identity of the value</returns>
        public IList<char> GetIdentity(string value)
        {
            var identity = new List<char>();
            foreach (var c in value.Where(c => !identity.Contains(c)))
            {
                identity.Add(c);
            }
            identity.Sort((a, b) => value.Count(x => x == b).CompareTo(value.Count(x => x == a)));
            return identity;
        }
    }
}