using TheFlow.CoreConcepts;

namespace TheFlow.Elements.Data
{
    public class EmbeddedDataStore<TData> : IDataStore<TData>
    {
        private readonly string _key;

        public EmbeddedDataStore(string key)
        {
            _key = key;
        }

        public void SetValue(
            ExecutionContext context,
            TData value
        )
        {
            context.Instance.EmbeddedDataStoresValues[_key] = value;
        }

        public TData GetValue(
            ExecutionContext context
        )
        {
            return (TData) context.Instance.EmbeddedDataStoresValues[_key];
        }
    }
}
