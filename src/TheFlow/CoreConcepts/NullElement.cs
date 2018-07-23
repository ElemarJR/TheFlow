using System.Collections.Generic;
using System.Linq;
using TheFlow.Elements;
using TheFlow.Elements.Data;

namespace TheFlow.CoreConcepts
{
    public class NullElement 
        : IElement, IDataProducer, IDataConsumer
    {
        private readonly List<DataOutput> _outputs = new List<DataOutput>();
        // TODO: Check duplicities
        public void AddDataOutput(DataOutput output)
        {
            _outputs.Add(output);
        }
        
        private readonly List<DataInput> _inputs = new List<DataInput>();

        public void AddDataInput(DataInput input)
        {
            _inputs.Add(input);
        }
        
        public DataOutput GetDataOutputByName(string name)
        {
            return _outputs.FirstOrDefault(o => o.Name == name);
        }

        public DataInput GetDataInputByName(string name)
        {
            return _inputs.FirstOrDefault(i => i.Name == name);
        }
    }
}