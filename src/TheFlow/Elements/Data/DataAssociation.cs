namespace TheFlow.Elements.Data
{
    public class DataAssociation<T>
    {
        public DataInput<T> Input { get; }
        public DataOutput<T> Output { get; }
        
        // TODO: Null?
        private DataAssociation(
            DataInput<T> input,
            DataOutput<T> output
            )
        {
            Input = input;
            Output = output;
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