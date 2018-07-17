namespace TheFlow.Elements
{
    public class UnnamedProcessElement<T>
        : ProcessElement<T> where T : IElement
    {
        private UnnamedProcessElement(T element) : base(element)
        {
        }
        
        public new static UnnamedProcessElement<T> Create(T element) =>
            new UnnamedProcessElement<T>(element);
    }
}