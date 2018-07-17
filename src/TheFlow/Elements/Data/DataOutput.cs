using System;
using System.Collections.Generic;

namespace TheFlow.Elements.Data
{

    public class DataOutput<T>
    {
        public string Name { get; }

        public DataOutput(string name)

        {
            Name = name;
        }
        
        
        List<Action<T>> _subscriptions = new List<Action<T>>();
        public void Subscribe(Action<T> action)
        {
            _subscriptions.Add(action);
        }
        
        public void Update(T newValue)
        {
            _subscriptions.ForEach(s => s(newValue));
        }

    }
}