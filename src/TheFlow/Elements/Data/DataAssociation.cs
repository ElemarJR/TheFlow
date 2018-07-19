using System;

namespace TheFlow.Elements.Data
{
    public class DataAssociation<T>
    {
        public DataInput<T> Input { get; }
        public DataOutput<T> Output { get; }
        
        private DataAssociation(
            DataInput<T> input,
            DataOutput<T> output
            )
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Output = output ?? throw new ArgumentNullException(nameof(output));
            output.Subscribe((data) => Input.Update(data));
        }

        // TODO: Incompatible types?
        public static DataAssociation<T> Create(
            DataInput<T> input,
            DataOutput<T> output
        )
        {
            return new DataAssociation<T>(input, output);
        }
    }
}