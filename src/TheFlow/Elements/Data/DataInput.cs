using System;

namespace TheFlow.Elements.Data
{
    public class DataInput<T>
    {
        public string Name { get; }

        public DataInput(string name)
        {
            Name = name;
        }
        
        public T CurrentValue { get; private set; }
        public void Update(T data)
        {
            CurrentValue = data;
        }


    }
}