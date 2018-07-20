using System;

namespace TheFlow.Elements.Data
{
    public class DataInput
    {
        public string Name { get; }

        public DataInput(string name)
        {
            Name = name;
        }
        
        public object CurrentValue { get; private set; }
        public void Update(object data)
        {
            CurrentValue = data;
        }


    }
}