using System;
using System.Linq;
using TheFlow.CoreConcepts;

namespace TheFlow.Elements.Data
{
    public class DataOutput : IDataProducer
    {
        public string Name { get; }

        public DataOutput(string name)

        {
            Name = name;
        }
        
        
        public void Update(IServiceProvider sp,
            Guid processInstanceId,
            string parentElementName,
            object newValue
            )
        {
            var pip = sp.GetService<IProcessInstanceProvider>();
            var pim = sp.GetService<IProcessModelProvider>();

            var instance = pip.GetProcessInstance(processInstanceId);
            // TODO: Provide an overload that accepts string
            var model = pim.GetProcessModel(Guid.Parse(instance.ProcessModelId));

            var allAssociations = model.Elements
                .OfType<INamedProcessElement<DataAssociation>>();
            
            var associations = allAssociations
                .Select(namedAssociation => namedAssociation.Element)
                .Where(association =>
                    association.DataProducerName == parentElementName &&
                    association.OutputName == Name
                );

            foreach (var association in associations)
            {
                // TODO: Target not found?
                var consumerElement = model.GetElementByName(association.DataConsumerName)
                    ?.Element as IDataConsumer;
                var input = consumerElement.GetDataInputByName(association.InputName);
                
                input.Update(sp, processInstanceId, association.DataConsumerName, newValue);
            }
            //_subscriptions.ForEach(s => s(newValue));
        }

        public DataOutput GetDataOutputByName(string name) 
            => name == Name ? this : null;
    }
}