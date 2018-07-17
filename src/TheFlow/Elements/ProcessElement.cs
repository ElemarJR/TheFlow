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

        public static NamedProcessElement<T> Create(
            string name,
            T element
        ) => NamedProcessElement<T>.Create(name, element);

        public static UnnamedProcessElement<T> Create(
            T element
        ) => UnnamedProcessElement<T>.Create(element);
    }
}