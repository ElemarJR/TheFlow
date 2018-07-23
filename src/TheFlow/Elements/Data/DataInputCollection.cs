using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TheFlow.Elements.Data
{
    public class DataInputCollection : ICollection<DataInput>
    {
        private readonly List<DataInput> _inner = new List<DataInput>();
        
        public IEnumerator<DataInput> GetEnumerator() => 
            _inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => _inner.GetEnumerator();

        public void Add(string item)
        {
            Add(new DataInput(item));
        }
        
        public void Add(DataInput item)
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
            throw new System.NotImplementedException();
        }

        public bool Contains(DataInput item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(DataInput[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(DataInput item)
        {
            throw new System.NotImplementedException();
        }

        public int Count => _inner.Count;
        public bool IsReadOnly => false;
    }
}