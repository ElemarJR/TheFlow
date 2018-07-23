namespace TheFlow.Elements.Data
{
    public interface IDataConsumer
    {
        DataInput GetDataInputByName(string name);
    }

    public interface IDataProducer
    {
        DataOutput GetDataOutputByName(string name);
    }
    
}