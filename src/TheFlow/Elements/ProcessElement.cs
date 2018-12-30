namespace TheFlow.Elements
{
    public interface IProcessElement<out T>
        where T : IElement
    {
        T Element { get; }
    }

    public abstract class ProcessElement<T> : IProcessElement<T>
        where T : IElement
    {
        public T Element { get; }

        protected ProcessElement(
            T element
            )
        {
            Element = element;
        }

        
    }

    public static class ProcessElement 
    {
        public static NamedProcessElement<T> Create<T>(
            string name,
            T element
        ) where T : IElement => NamedProcessElement<T>.Create(name, element);

        public static UnnamedProcessElement<T> Create<T>(
            T element
        ) where T : IElement => UnnamedProcessElement<T>.Create(element);
    }
}