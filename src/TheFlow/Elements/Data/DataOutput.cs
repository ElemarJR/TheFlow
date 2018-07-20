using System;
using System.Collections.Generic;

namespace TheFlow.Elements.Data
{
    public class DataOutput
    {
        public string Name { get; }

        public DataOutput(string name)

        {
            Name = name;
        }


        readonly List<Action<object>> _subscriptions = new List<Action<object>>();
        internal void Subscribe(Action<object> action)
        {
            _subscriptions.Add(action);
        }
        
        public void Update(object newValue)
        {
            _subscriptions.ForEach(s => s(newValue));
        }

    }
}