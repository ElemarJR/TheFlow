namespace TheFlow.Elements.Connections
{
    public interface IConnectionElement : IElement
    {
        string From { get; }
        string To { get; }
        
        object FilterValue { get; }
        bool HasFilterValue { get; }

    }
}