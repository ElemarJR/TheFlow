namespace TheFlow.Elements.Data
{
    public interface IDataProducerElement : IElement
    {
        void AddDataOutput<T>(DataOutput<T> dataOutput);
    }
}