namespace TheFlow.Elements
{
    public class NamedProcessElement<T>
        : ProcessElement<T>, INamedProcessElement<T> 
        where T : IElement
    {
        public string Name { get; }
        
        // TODO: Validate Name

        private NamedProcessElement(
            string name,
            T element
        ) : base(element)
        {
            Name = name;
        }

        public new static NamedProcessElement<T> Create(
            string name, T element
        ) => new NamedProcessElement<T>(name, element);
    }
}