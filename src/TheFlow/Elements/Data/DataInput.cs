using System;
using TheFlow.CoreConcepts;

namespace TheFlow.Elements.Data
{
    public class DataInput : IDataConsumer
    {
        public string Name { get; }

        public DataInput(string name)
        {
            Name = name;
        }
        
        public void Update(
            ExecutionContext context,
            string parentElementName,
            object data)
        {
            context.Instance.SetDataInputValue(parentElementName, Name, data);
        }

        public object GetCurrentValue(
            ExecutionContext context,
            string parentElementName
        )
        {
            return context.Instance.GetDataInputValue(parentElementName, Name);
        }

        public DataInput GetDataInputByName(string name) 
            => name == Name ? this : null;
        
        public static implicit operator DataInput(string name) =>
            new DataInput(name);
    }
}