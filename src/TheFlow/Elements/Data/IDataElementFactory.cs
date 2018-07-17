namespace TheFlow.Elements.Data
{
    public interface IDataElementFactory : IElement
    {
        object CreateInstance();
    }
}