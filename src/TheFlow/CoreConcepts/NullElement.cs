using System.Collections.Generic;
using System.Linq;
using TheFlow.Elements;
using TheFlow.Elements.Data;

namespace TheFlow.CoreConcepts
{
    public class NullElement 
        : IElement, IDataProducer, IDataConsumer
    {
        public  readonly DataOutputCollection Outputs = new DataOutputCollection();
        public readonly DataInputCollection Inputs = new DataInputCollection();

        
        public DataOutput GetDataOutputByName(string name)
        {
            return Outputs.FirstOrDefault(o => o.Name == name);
        }

        public DataInput GetDataInputByName(string name)
        {
            return Inputs.FirstOrDefault(i => i.Name == name);
        }
    }
}