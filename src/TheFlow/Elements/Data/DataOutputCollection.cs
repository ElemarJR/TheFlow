using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TheFlow.Elements.Data
{
    public class DataOutputCollection : ICollection<DataOutput>
    {
        private readonly List<DataOutput> _inner = new List<DataOutput> { new DataOutput("default")};
        
        public IEnumerator<DataOutput> GetEnumerator() => 
            _inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => _inner.GetEnumerator();
        
        public DataOutput this[string name]
        {
            get { return _inner.First(i => i.Name == name); }
        }

        
        public void Add(DataOutput item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (_inner.Any(i => item.Name == i.Name))
            {
                throw new InvalidOperationException("Trying to add two DataOutputs with same key.");
            }

            _inner.Add(item);
        }
       

        public void Clear()
        {
            _inner.Clear();
        }

        public bool Contains(DataOutput item)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(DataOutput[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool Remove(DataOutput item)
        {
            throw new NotSupportedException();
        }

        public int Count => _inner.Count;
        public bool IsReadOnly => false;


    }
}