using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NumeralSystems.Net.Type.Base
{
    /// <summary>
    /// Represents a custom string type that implements the IList interface for Char objects.
    /// </summary>
    public partial class String: IList<Char>
    {
        /// <summary>
        /// Initializes a new instance of the String class.
        /// </summary>
        public String()
        {

        }

        /// <summary>
        /// Initializes a new instance of the String class with the specified string value.
        /// </summary>
        /// <param name="value">The string value to initialize the String instance with.</param>
        public String(string value)
        {
            foreach (var c in value)
            {
                Add(new Char() { Value = c });
            }
        }

        private readonly List<Char> _chars = new();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        public IEnumerator<Char> GetEnumerator() => _chars.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds a Char object to the end of the String.
        /// </summary>
        /// <param name="item">The Char object to add.</param>
        public void Add(Char item)
        {
            if (item == null) return;
            _chars.Add(item);
        }

        /// <summary>
        /// Removes all elements from the String.
        /// </summary>
        public void Clear() => _chars.Clear();

        /// <summary>
        /// Determines whether the String contains a specific Char object.
        /// </summary>
        /// <param name="item">The Char object to locate in the String.</param>
        /// <returns>True if the Char object is found; otherwise, false.</returns>
        public bool Contains(Char item) => _chars.Any(x => item != null && x.Value == item.Value);

        /// <summary>
        /// Copies the elements of the String to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The array to copy elements to.</param>
        /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
        public void CopyTo(Char[] array, int arrayIndex) => _chars.CopyTo(array, arrayIndex);

        /// <summary>
        /// Removes the first occurrence of a specific Char object from the String.
        /// </summary>
        /// <param name="item">The Char object to remove.</param>
        /// <returns>True if the Char object was successfully removed; otherwise, false.</returns>
        public bool Remove(Char item)
        {
            if (item == null) return false;
            var index = _chars.FindIndex(x => x.Value == item.Value);
            if (index == -1) return false;
            _chars.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Gets the number of elements contained in the String.
        /// </summary>
        public int Count => _chars.Count;

        /// <summary>
        /// Gets a value indicating whether the String is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Determines the index of a specific Char object in the String.
        /// </summary>
        /// <param name="item">The Char object to locate in the String.</param>
        /// <returns>The index of the Char object if found; otherwise, -1.</returns>
        public int IndexOf(Char item)
        {
            return _chars.FindIndex(x => item != null && x.Value == item.Value);
        }

        /// <summary>
        /// Inserts a Char object at the specified index in the String.
        /// </summary>
        /// <param name="index">The zero-based index at which the Char object should be inserted.</param>
        /// <param name="item">The Char object to insert.</param>
        public void Insert(int index, Char item)
        {
            if (item == null) return;
            _chars.Insert(index, item);
        }

        /// <summary>
        /// Removes the Char object at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the Char object to remove.</param>
        public void RemoveAt(int index)
        {
            _chars.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets the Char object at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the Char object to get or set.</param>
        /// <returns>The Char object at the specified index.</returns>
        public Char this[int index]
        {
            get => _chars[index];
            set
            {
                if (value == null) return;
                _chars[index] = value;
            }
        }

        /// <summary>
        /// Returns the string representation of the String.
        /// </summary>
        /// <returns>The string representation of the String.</returns>
        public override string ToString() => string.Join(string.Empty, _chars.Select(x => x.ToString()));

        /// <summary>
        /// Converts the String to a string using the specified format.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <returns>The formatted string representation of the String.</returns>
        public string ToString(string format) => string.Join(string.Empty, _chars.Select(x => x.ToString(format)));
    }
}
