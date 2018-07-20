using System;

namespace TheFlow.Elements.Data
{
    public class DataAssociation
    {
        public DataInput Input { get; }
        public DataOutput Output { get; }
        
        private DataAssociation(
            DataInput input,
            DataOutput output
            )
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Output = output ?? throw new ArgumentNullException(nameof(output));
            output.Subscribe((data) => Input.Update(data));
        }

        public static DataAssociation Create(
            DataInput input,
            DataOutput output
        )
        {
            return new DataAssociation(input, output);
        }
    }
}