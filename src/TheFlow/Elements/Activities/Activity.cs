using System;
using System.Linq;
using TheFlow.CoreConcepts;
using TheFlow.Elements.Data;

namespace TheFlow.Elements.Activities
{
    public abstract class Activity : IElement,
        IDataConsumer, IDataProducer
    {
        public abstract void Run(
            ExecutionContext context 
            );

        public DataOutputCollection Outputs { get; } = new DataOutputCollection();
        public DataInputCollection Inputs { get; } = new DataInputCollection();

        public DataOutput GetDataOutputByName(string name) 
            => Outputs.FirstOrDefault(o => o.Name == name);

        public DataInput GetDataInputByName(string name) 
            => Inputs.FirstOrDefault(i => i.Name == name);
    }
}