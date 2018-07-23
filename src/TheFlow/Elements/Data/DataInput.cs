using System;

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
            IServiceProvider sp,
            Guid processInstanceId,
            string parentElementName,
            object data)
        {
            var pip = sp.GetService<IProcessInstanceProvider>();
            var instance = pip.GetProcessInstance(processInstanceId);
            instance.SetDataInputValue(parentElementName, Name, data);
        }

        public object GetCurrentValue(
            IServiceProvider sp,
            Guid processInstanceId,
            string parentElementName
        )
        {
            var pip = sp.GetService<IProcessInstanceProvider>();
            var instance = pip.GetProcessInstance(processInstanceId);
            return instance.GetDataInputValue(parentElementName, Name);
        }

        public DataInput GetDataInputByName(string name) 
            => name == Name ? this : null;
    }
}