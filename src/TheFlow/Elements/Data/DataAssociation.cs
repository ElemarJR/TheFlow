using System;

namespace TheFlow.Elements.Data
{
    public class DataAssociation : IElement
    {
        public string DataProducerName { get; }
        public string OutputName { get; }
        public string DataConsumerName { get; }
        public string InputName { get; }

        private DataAssociation(
            string dataProducerName,
            string outputName,
            string dataConsumerName,
            string inputName
            )
        {
            DataProducerName = dataProducerName;
            OutputName = outputName;
            DataConsumerName = dataConsumerName;
            InputName = inputName;
        }

        public static DataAssociation Create(
            string dataProducerName,
            string outputName,
            string dataConsumerName,
            string inputName

            )
        {
            return new DataAssociation(
                dataProducerName,
                outputName,
                dataConsumerName,
                inputName
                );
            
        }
    }
}