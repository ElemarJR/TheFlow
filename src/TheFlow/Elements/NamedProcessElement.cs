using System;

namespace TheFlow.Elements
{
    public class NamedProcessElement<T>
        : ProcessElement<T>, INamedProcessElement<T> 
        where T : IElement
    {
        public string Name { get; }
        
       
        private NamedProcessElement(
            string name,
            T element
        ) : base(element)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public new static NamedProcessElement<T> Create(
            string name, T element
        ) => new NamedProcessElement<T>(name, element);
    }
}