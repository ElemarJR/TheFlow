namespace TheFlow.Elements
{
    public interface INamedProcessElement<out T> :
        IProcessElement<T> where T : IElement
    {
        string Name { get; }
    }
}