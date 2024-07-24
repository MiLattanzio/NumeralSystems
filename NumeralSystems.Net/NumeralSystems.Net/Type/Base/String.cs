using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NumeralSystems.Net.Type.Base
{
    public partial class String: IList<Char>
    {
        public String()
        {
            
        }
        
        public String(string value)
        {
            foreach (var c in value)
            {
                Add(new Char() { Value = c });
            }
        }
        
        private readonly List<Char> _chars = new();
        public IEnumerator<Char> GetEnumerator() => _chars.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Char item)
        {
            if (item == null) return;
            _chars.Add(item);
        }

        public void Clear() => _chars.Clear();

        public bool Contains(Char item) => _chars.Any(x => item != null && x.Value == item.Value);

        public void CopyTo(Char[] array, int arrayIndex) => _chars.CopyTo(array, arrayIndex);

        public bool Remove(Char item)
        {
            if (item == null) return false;
            var index = _chars.FindIndex(x => x.Value == item.Value);
            if (index == -1) return false;
            _chars.RemoveAt(index);
            return true;
        }

        public int Count => _chars.Count;
        public bool IsReadOnly => false;
        public int IndexOf(Char item)
        {
            return _chars.FindIndex(x => item != null && x.Value == item.Value);
        }

        public void Insert(int index, Char item)
        {
            if (item == null) return;
            _chars.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _chars.RemoveAt(index);
        }

        public Char this[int index]
        {
            get => _chars[index];
            set
            {
                if (value == null) return;
                _chars[index] = value;
            }
        }
        
        public override string ToString() => string.Join(string.Empty, _chars.Select(x => x.ToString()));
        public string ToString(string format) => string.Join(string.Empty, _chars.Select(x => x.ToString(format)));
    }
}