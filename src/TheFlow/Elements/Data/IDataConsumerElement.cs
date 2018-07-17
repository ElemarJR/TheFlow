namespace TheFlow.Elements.Data
{
    public interface IDataConsumerElement : IElement
    {
        void AddDataInput<T>(DataInput<T> dataInput);
    }
}
