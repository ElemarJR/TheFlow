using TheFlow.CoreConcepts;

namespace TheFlow.Elements.Data
{
    public interface IDataStore<TData> : IElement
    {
        void SetValue(
            ExecutionContext context,
            TData value
        );

        TData GetValue(
            ExecutionContext context
        );
    }
}